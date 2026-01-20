using MediatR;
using Microsoft.EntityFrameworkCore;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Analytics.Models;

namespace Khidmah_Inventory.Application.Features.Analytics.Queries.GetProfitAnalytics;

public class GetProfitAnalyticsQueryHandler : IRequestHandler<GetProfitAnalyticsQuery, Result<ProfitAnalyticsDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetProfitAnalyticsQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<ProfitAnalyticsDto>> Handle(GetProfitAnalyticsQuery request, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId;
        if (!companyId.HasValue)
            return Result<ProfitAnalyticsDto>.Failure("Company context is required");

        var (fromDate, toDate) = CalculateDateRange(request.Request);

        var analytics = new ProfitAnalyticsDto
        {
            FromDate = fromDate,
            ToDate = toDate
        };

        // Get sales orders
        var salesOrders = await _context.SalesOrders
            .Include(so => so.Items)
                .ThenInclude(item => item.Product)
                    .ThenInclude(p => p.Category)
            .Where(so => so.CompanyId == companyId.Value &&
                !so.IsDeleted &&
                so.OrderDate >= fromDate &&
                so.OrderDate <= toDate)
            .ToListAsync(cancellationToken);

        // Calculate revenue and cost
        decimal totalRevenue = salesOrders.Sum(so => so.TotalAmount);
        decimal totalCost = 0;

        foreach (var order in salesOrders)
        {
            foreach (var item in order.Items)
            {
                var stockLevel = await _context.StockLevels
                    .FirstOrDefaultAsync(sl => sl.ProductId == item.ProductId && 
                        sl.CompanyId == companyId.Value, cancellationToken);

                var cost = stockLevel?.AverageCost ?? item.Product.PurchasePrice;
                totalCost += item.Quantity * cost;
            }
        }

        analytics.TotalRevenue = totalRevenue;
        analytics.TotalCost = totalCost;
        analytics.TotalProfit = totalRevenue - totalCost;
        analytics.ProfitMargin = totalRevenue > 0 
            ? (analytics.TotalProfit / totalRevenue) * 100 
            : 0;
        analytics.GrossProfitMargin = totalRevenue > 0 
            ? ((totalRevenue - totalCost) / totalRevenue) * 100 
            : 0;

        // Profit trend (daily)
        var currentDate = fromDate.Date;
        var profitTrend = new List<TimeSeriesDataDto>();
        while (currentDate <= toDate.Date)
        {
            var dayOrders = salesOrders.Where(o => o.OrderDate.Date == currentDate).ToList();
            decimal dayRevenue = dayOrders.Sum(o => o.TotalAmount);
            decimal dayCost = 0;

            foreach (var order in dayOrders)
            {
                foreach (var item in order.Items)
                {
                    var stockLevel = _context.StockLevels
                        .FirstOrDefault(sl => sl.ProductId == item.ProductId && sl.CompanyId == companyId.Value);
                    var cost = stockLevel?.AverageCost ?? item.Product.PurchasePrice;
                    dayCost += item.Quantity * cost;
                }
            }

            profitTrend.Add(new TimeSeriesDataDto
            {
                Label = currentDate.ToString("MMM dd"),
                Date = currentDate,
                Value = dayRevenue - dayCost,
                SecondaryValue = dayRevenue
            });

            currentDate = currentDate.AddDays(1);
        }

        analytics.ProfitTrend = profitTrend;

        // Category profits
        analytics.CategoryProfits = salesOrders
            .SelectMany(so => so.Items)
            .GroupBy(item => item.Product.Category != null ? item.Product.Category.Name : "Uncategorized")
            .Select(g =>
            {
                var items = g.ToList();
                decimal revenue = items.Sum(item => item.LineTotal);
                decimal cost = items.Sum(item =>
                {
                    var stockLevel = _context.StockLevels
                        .FirstOrDefault(sl => sl.ProductId == item.ProductId && sl.CompanyId == companyId.Value);
                    return item.Quantity * (stockLevel?.AverageCost ?? item.Product.PurchasePrice);
                });

                return new CategoryProfitDto
                {
                    CategoryName = g.Key,
                    Revenue = revenue,
                    Cost = cost,
                    Profit = revenue - cost,
                    ProfitMargin = revenue > 0 ? ((revenue - cost) / revenue) * 100 : 0
                };
            })
            .ToList();

        var totalCategoryRevenue = analytics.CategoryProfits.Sum(c => c.Revenue);
        foreach (var category in analytics.CategoryProfits)
        {
            category.Percentage = totalCategoryRevenue > 0 
                ? (category.Revenue / totalCategoryRevenue) * 100 
                : 0;
        }

        return Result<ProfitAnalyticsDto>.Success(analytics);
    }

    private (DateTime fromDate, DateTime toDate) CalculateDateRange(AnalyticsRequestDto request)
    {
        var now = DateTime.UtcNow;
        var today = now.Date;
        var startOfMonth = new DateTime(today.Year, today.Month, 1);
        var startOfYear = new DateTime(today.Year, 1, 1);

        return request.TimeRange switch
        {
            TimeRangeType.Today => (today, today.AddDays(1).AddTicks(-1)),
            TimeRangeType.Yesterday => (today.AddDays(-1), today.AddTicks(-1)),
            TimeRangeType.Last7Days => (today.AddDays(-7), today.AddDays(1).AddTicks(-1)),
            TimeRangeType.Last30Days => (today.AddDays(-30), today.AddDays(1).AddTicks(-1)),
            TimeRangeType.ThisMonth => (startOfMonth, today.AddDays(1).AddTicks(-1)),
            TimeRangeType.LastMonth => (startOfMonth.AddMonths(-1), startOfMonth.AddTicks(-1)),
            TimeRangeType.ThisQuarter => (new DateTime(today.Year, ((today.Month - 1) / 3) * 3 + 1, 1), today.AddDays(1).AddTicks(-1)),
            TimeRangeType.LastQuarter => (new DateTime(today.Year, ((today.Month - 1) / 3) * 3 + 1, 1).AddMonths(-3), new DateTime(today.Year, ((today.Month - 1) / 3) * 3 + 1, 1).AddTicks(-1)),
            TimeRangeType.ThisYear => (startOfYear, today.AddDays(1).AddTicks(-1)),
            TimeRangeType.LastYear => (startOfYear.AddYears(-1), startOfYear.AddTicks(-1)),
            TimeRangeType.Custom => (request.CustomFromDate ?? today, request.CustomToDate ?? today.AddDays(1).AddTicks(-1)),
            _ => (today.AddDays(-30), today.AddDays(1).AddTicks(-1))
        };
    }
}

