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

        if (request.FromDate >= request.ToDate)
            return Result<SalesReportDto>.Failure("From date must be before To date");

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

