using MediatR;
using Microsoft.EntityFrameworkCore;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Reports.Models;

namespace Khidmah_Inventory.Application.Features.Reports.Queries.GetSalesReport;

public class GetSalesReportQueryHandler : IRequestHandler<GetSalesReportQuery, Result<SalesReportDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetSalesReportQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<SalesReportDto>> Handle(GetSalesReportQuery request, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId;
        if (!companyId.HasValue)
            return Result<SalesReportDto>.Failure("Company context is required");

        // Validate date range
        if (request.FromDate == default || request.ToDate == default)
            return Result<SalesReportDto>.Failure("Invalid date parameters");

        if (request.FromDate > request.ToDate)
            return Result<SalesReportDto>.Failure("From date cannot be after To date");

        if (request.ToDate > DateTime.UtcNow.AddDays(1))
            return Result<SalesReportDto>.Failure("To date cannot be in the future");

        var query = _context.SalesOrders
            .Include(so => so.Customer)
            .Include(so => so.Items)
                .ThenInclude(item => item.Product)
            .Where(so => so.CompanyId == companyId.Value && 
                !so.IsDeleted &&
                so.OrderDate >= request.FromDate &&
                so.OrderDate <= request.ToDate);

        if (request.CustomerId.HasValue)
            query = query.Where(so => so.CustomerId == request.CustomerId.Value);

        var orders = await query.ToListAsync(cancellationToken);

        var report = new SalesReportDto
        {
            FromDate = request.FromDate,
            ToDate = request.ToDate,
            TotalSales = orders.Sum(o => o.TotalAmount),
            TotalOrders = orders.Count
        };

        // If no orders found, return empty report
        if (!orders.Any())
        {
            report.TotalCost = 0;
            report.TotalProfit = 0;
            report.ProfitMargin = 0;
            report.Items = new List<SalesReportItemDto>();
            return Result<SalesReportDto>.Success(report);
        }

        // Calculate cost and profit
        decimal totalCost = 0;
        foreach (var order in orders)
        {
            foreach (var item in order.Items)
            {
                if (item.Product == null)
                    continue; // Skip items with missing product data

                var stockLevel = await _context.StockLevels
                    .FirstOrDefaultAsync(sl => sl.ProductId == item.ProductId &&
                        sl.CompanyId == companyId.Value, cancellationToken);

                var cost = stockLevel?.AverageCost ?? item.Product.PurchasePrice;
                totalCost += item.Quantity * cost;
            }
        }

        report.TotalCost = totalCost;
        report.TotalProfit = report.TotalSales - report.TotalCost;
        report.ProfitMargin = report.TotalSales > 0 
            ? (report.TotalProfit / report.TotalSales) * 100 
            : 0;

        // Previous period comparison (same length, immediately before FromDate)
        var periodDays = (request.ToDate - request.FromDate).Days + 1;
        var prevEnd = request.FromDate.AddDays(-1);
        var prevStart = prevEnd.AddDays(-periodDays + 1);
        var prevOrders = await _context.SalesOrders
            .Include(so => so.Items).ThenInclude(i => i.Product)
            .Where(so => so.CompanyId == companyId.Value && !so.IsDeleted
                && so.OrderDate >= prevStart && so.OrderDate <= prevEnd)
            .ToListAsync(cancellationToken);
        if (request.CustomerId.HasValue)
            prevOrders = prevOrders.Where(o => o.CustomerId == request.CustomerId.Value).ToList();
        decimal prevSales = prevOrders.Sum(o => o.TotalAmount);
        decimal prevCost = 0;
        foreach (var order in prevOrders)
        {
            foreach (var item in order.Items)
            {
                if (item.Product == null) continue;
                var sl = await _context.StockLevels.FirstOrDefaultAsync(sl => sl.ProductId == item.ProductId && sl.CompanyId == companyId.Value, cancellationToken);
                prevCost += item.Quantity * (sl?.AverageCost ?? item.Product.PurchasePrice);
            }
        }
        decimal prevProfit = prevSales - prevCost;
        report.PreviousPeriodFromDate = prevStart;
        report.PreviousPeriodToDate = prevEnd;
        report.PreviousPeriodTotalSales = prevSales;
        report.PreviousPeriodTotalCost = prevCost;
        report.PreviousPeriodTotalProfit = prevProfit;
        if (prevSales > 0)
        {
            report.VarianceSalesPercent = Math.Round((report.TotalSales - prevSales) / prevSales * 100, 2);
            report.TrendDirection = report.VarianceSalesPercent > 1 ? "Up" : report.VarianceSalesPercent < -1 ? "Down" : "Stable";
        }
        if (prevProfit != 0)
            report.VarianceProfitPercent = Math.Round((report.TotalProfit - prevProfit) / Math.Abs(prevProfit) * 100, 2);

        // Get all product IDs from orders to fetch stock levels in one query
        var productIds = orders.SelectMany(o => o.Items.Select(i => i.ProductId)).Distinct().ToList();
        var stockLevels = new Dictionary<Guid, decimal?>();
        if (productIds.Any())
        {
            var stockLevelList = await _context.StockLevels
                .Where(sl => productIds.Contains(sl.ProductId) && sl.CompanyId == companyId.Value)
                .ToListAsync(cancellationToken);

            foreach (var sl in stockLevelList)
            {
                stockLevels[sl.ProductId] = sl.AverageCost;
            }
        }

        // Report items
        report.Items = orders.Select(order =>
        {
            decimal orderCost = 0;
            foreach (var item in order.Items)
            {
                if (item.Product == null)
                    continue; // Skip items with missing product data

                var cost = stockLevels.TryGetValue(item.ProductId, out var averageCost) && averageCost.HasValue
                    ? averageCost.Value
                    : item.Product.PurchasePrice;
                orderCost += item.Quantity * cost;
            }

            return new SalesReportItemDto
            {
                Date = order.OrderDate,
                OrderNumber = order.OrderNumber,
                CustomerName = order.Customer?.Name ?? "Unknown Customer",
                Amount = order.TotalAmount,
                Cost = orderCost,
                Profit = order.TotalAmount - orderCost
            };
        }).ToList();

        return Result<SalesReportDto>.Success(report);
    }
}

