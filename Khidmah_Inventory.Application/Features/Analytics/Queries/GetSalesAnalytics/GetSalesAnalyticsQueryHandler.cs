using MediatR;
using Microsoft.EntityFrameworkCore;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Analytics.Models;

namespace Khidmah_Inventory.Application.Features.Analytics.Queries.GetSalesAnalytics;

public class GetSalesAnalyticsQueryHandler : IRequestHandler<GetSalesAnalyticsQuery, Result<SalesAnalyticsDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetSalesAnalyticsQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<SalesAnalyticsDto>> Handle(GetSalesAnalyticsQuery request, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId;
        if (!companyId.HasValue)
            return Result<SalesAnalyticsDto>.Failure("Company context is required");

        var (fromDate, toDate) = CalculateDateRange(request.Request);
        var groupBy = request.Request.GroupBy ?? "Day";

        var analytics = new SalesAnalyticsDto
        {
            FromDate = fromDate,
            ToDate = toDate
        };

        // Get sales orders in date range
        var salesOrders = await _context.SalesOrders
            .Include(so => so.Customer)
            .Include(so => so.Items)
                .ThenInclude(item => item.Product)
                    .ThenInclude(p => p.Category)
            .Where(so => so.CompanyId == companyId.Value &&
                !so.IsDeleted &&
                so.OrderDate >= fromDate &&
                so.OrderDate <= toDate)
            .ToListAsync(cancellationToken);

        // Calculate totals
        analytics.TotalSales = salesOrders.Sum(so => so.TotalAmount);
        analytics.TotalOrders = salesOrders.Count;
        analytics.AverageOrderValue = analytics.TotalOrders > 0 
            ? analytics.TotalSales / analytics.TotalOrders 
            : 0;

        // Calculate cost and profit
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

        analytics.TotalCost = totalCost;
        analytics.TotalProfit = analytics.TotalSales - analytics.TotalCost;
        analytics.ProfitMargin = analytics.TotalSales > 0 
            ? (analytics.TotalProfit / analytics.TotalSales) * 100 
            : 0;

        // Time series data
        analytics.TimeSeriesData = GenerateTimeSeriesData(salesOrders, fromDate, toDate, groupBy);

        // Category breakdown
        analytics.CategoryBreakdown = salesOrders
            .SelectMany(so => so.Items)
            .GroupBy(item => item.Product.Category != null ? item.Product.Category.Name : "Uncategorized")
            .Select(g => new CategoryAnalyticsDto
            {
                CategoryName = g.Key,
                TotalSales = g.Sum(item => item.LineTotal),
                TotalCost = g.Sum(item =>
                {
                    var stockLevel = _context.StockLevels
                        .FirstOrDefault(sl => sl.ProductId == item.ProductId && sl.CompanyId == companyId.Value);
                    return item.Quantity * (stockLevel?.AverageCost ?? item.Product.PurchasePrice);
                }),
                OrderCount = g.Select(item => item.SalesOrderId).Distinct().Count()
            })
            .ToList();

        var totalCategorySales = analytics.CategoryBreakdown.Sum(c => c.TotalSales);
        foreach (var category in analytics.CategoryBreakdown)
        {
            category.TotalProfit = category.TotalSales - category.TotalCost;
            category.Percentage = totalCategorySales > 0 
                ? (category.TotalSales / totalCategorySales) * 100 
                : 0;
        }

        // Top products
        analytics.TopProducts = salesOrders
            .SelectMany(so => so.Items)
            .GroupBy(item => new { item.ProductId, item.Product.Name, item.Product.SKU })
            .Select(g => new ProductAnalyticsDto
            {
                ProductId = g.Key.ProductId,
                ProductName = g.Key.Name,
                ProductSKU = g.Key.SKU,
                TotalSales = g.Sum(item => item.LineTotal),
                QuantitySold = g.Sum(item => item.Quantity),
                AveragePrice = g.Average(item => item.UnitPrice)
            })
            .OrderByDescending(p => p.QuantitySold)
            .Take(10)
            .ToList();

        foreach (var product in analytics.TopProducts)
        {
            var items = salesOrders
                .SelectMany(so => so.Items)
                .Where(item => item.ProductId == product.ProductId)
                .ToList();

            product.TotalCost = items.Sum(item =>
            {
                var stockLevel = _context.StockLevels
                    .FirstOrDefault(sl => sl.ProductId == item.ProductId && sl.CompanyId == companyId.Value);
                return item.Quantity * (stockLevel?.AverageCost ?? item.Product.PurchasePrice);
            });
            product.TotalProfit = product.TotalSales - product.TotalCost;
        }

        // Top customers
        analytics.TopCustomers = salesOrders
            .GroupBy(so => new { so.CustomerId, so.Customer.Name })
            .Select(g => new CustomerAnalyticsDto
            {
                CustomerId = g.Key.CustomerId,
                CustomerName = g.Key.Name,
                TotalSales = g.Sum(so => so.TotalAmount),
                OrderCount = g.Count(),
                AverageOrderValue = g.Average(so => so.TotalAmount)
            })
            .OrderByDescending(c => c.TotalSales)
            .Take(10)
            .ToList();

        var totalCustomerSales = analytics.TopCustomers.Sum(c => c.TotalSales);
        foreach (var customer in analytics.TopCustomers)
        {
            customer.Percentage = totalCustomerSales > 0 
                ? (customer.TotalSales / totalCustomerSales) * 100 
                : 0;
        }

        return Result<SalesAnalyticsDto>.Success(analytics);
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

    private List<TimeSeriesDataDto> GenerateTimeSeriesData(
        List<Domain.Entities.SalesOrder> orders,
        DateTime fromDate,
        DateTime toDate,
        string groupBy)
    {
        var timeSeries = new List<TimeSeriesDataDto>();

        if (groupBy == "Day")
        {
            var currentDate = fromDate.Date;
            while (currentDate <= toDate.Date)
            {
                var dayOrders = orders.Where(o => o.OrderDate.Date == currentDate).ToList();
                timeSeries.Add(new TimeSeriesDataDto
                {
                    Label = currentDate.ToString("MMM dd"),
                    Date = currentDate,
                    Value = dayOrders.Sum(o => o.TotalAmount)
                });
                currentDate = currentDate.AddDays(1);
            }
        }
        else if (groupBy == "Week")
        {
            var currentDate = fromDate.Date;
            while (currentDate <= toDate.Date)
            {
                var weekEnd = currentDate.AddDays(6);
                var weekOrders = orders.Where(o => o.OrderDate.Date >= currentDate && o.OrderDate.Date <= weekEnd).ToList();
                timeSeries.Add(new TimeSeriesDataDto
                {
                    Label = $"Week {GetWeekOfYear(currentDate)}",
                    Date = currentDate,
                    Value = weekOrders.Sum(o => o.TotalAmount)
                });
                currentDate = currentDate.AddDays(7);
            }
        }
        else if (groupBy == "Month")
        {
            var currentDate = new DateTime(fromDate.Year, fromDate.Month, 1);
            while (currentDate <= toDate.Date)
            {
                var monthEnd = currentDate.AddMonths(1).AddDays(-1);
                var monthOrders = orders.Where(o => o.OrderDate.Date >= currentDate && o.OrderDate.Date <= monthEnd).ToList();
                timeSeries.Add(new TimeSeriesDataDto
                {
                    Label = currentDate.ToString("MMM yyyy"),
                    Date = currentDate,
                    Value = monthOrders.Sum(o => o.TotalAmount)
                });
                currentDate = currentDate.AddMonths(1);
            }
        }

        return timeSeries;
    }

    private int GetWeekOfYear(DateTime date)
    {
        var culture = System.Globalization.CultureInfo.CurrentCulture;
        var calendar = culture.Calendar;
        return calendar.GetWeekOfYear(date, culture.DateTimeFormat.CalendarWeekRule, culture.DateTimeFormat.FirstDayOfWeek);
    }
}

