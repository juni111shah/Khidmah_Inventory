using MediatR;
using Microsoft.EntityFrameworkCore;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Analytics.Models;

namespace Khidmah_Inventory.Application.Features.Analytics.Queries.GetInventoryAnalytics;

public class GetInventoryAnalyticsQueryHandler : IRequestHandler<GetInventoryAnalyticsQuery, Result<InventoryAnalyticsDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetInventoryAnalyticsQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<InventoryAnalyticsDto>> Handle(GetInventoryAnalyticsQuery request, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId;
        if (!companyId.HasValue)
            return Result<InventoryAnalyticsDto>.Failure("Company context is required");

        var query = _context.StockLevels
            .Include(sl => sl.Product)
                .ThenInclude(p => p.Category)
            .Include(sl => sl.Warehouse)
            .Where(sl => sl.CompanyId == companyId.Value);

        if (request.WarehouseId.HasValue)
            query = query.Where(sl => sl.WarehouseId == request.WarehouseId.Value);

        if (request.CategoryId.HasValue)
            query = query.Where(sl => sl.Product.CategoryId == request.CategoryId.Value);

        var stockLevels = await query.ToListAsync(cancellationToken);

        var analytics = new InventoryAnalyticsDto
        {
            TotalStockValue = stockLevels.Sum(sl => sl.Quantity * (sl.AverageCost ?? sl.Product.PurchasePrice)),
            TotalProducts = stockLevels.Select(sl => sl.ProductId).Distinct().Count(),
            LowStockItems = stockLevels.Count(sl => sl.Product.MinStockLevel.HasValue && 
                sl.Quantity <= sl.Product.MinStockLevel.Value && sl.Quantity > 0),
            OutOfStockItems = stockLevels.Count(sl => sl.Quantity == 0)
        };

        analytics.AverageStockValue = analytics.TotalProducts > 0 
            ? analytics.TotalStockValue / analytics.TotalProducts 
            : 0;

        // Category stock values
        var totalStockValue = analytics.TotalStockValue;
        analytics.CategoryStockValues = stockLevels
            .GroupBy(sl => sl.Product.Category != null ? sl.Product.Category.Name : "Uncategorized")
            .Select(g => new CategoryStockValueDto
            {
                CategoryName = g.Key,
                StockValue = g.Sum(sl => sl.Quantity * (sl.AverageCost ?? sl.Product.PurchasePrice)),
                ProductCount = g.Select(sl => sl.ProductId).Distinct().Count()
            })
            .ToList();

        foreach (var category in analytics.CategoryStockValues)
        {
            category.Percentage = totalStockValue > 0 
                ? (category.StockValue / totalStockValue) * 100 
                : 0;
        }

        // Warehouse stock values
        analytics.WarehouseStockValues = stockLevels
            .GroupBy(sl => sl.Warehouse.Name)
            .Select(g => new WarehouseStockValueDto
            {
                WarehouseName = g.Key,
                StockValue = g.Sum(sl => sl.Quantity * (sl.AverageCost ?? sl.Product.PurchasePrice)),
                ProductCount = g.Select(sl => sl.ProductId).Distinct().Count()
            })
            .ToList();

        foreach (var warehouse in analytics.WarehouseStockValues)
        {
            warehouse.Percentage = totalStockValue > 0 
                ? (warehouse.StockValue / totalStockValue) * 100 
                : 0;
        }

        // Fast moving products (last 30 days)
        var thirtyDaysAgo = DateTime.UtcNow.AddDays(-30);
        var fastMoving = await _context.SalesOrderItems
            .Include(item => item.Product)
            .Include(item => item.SalesOrder)
            .Where(item => item.SalesOrder.CompanyId == companyId.Value &&
                !item.SalesOrder.IsDeleted &&
                item.SalesOrder.OrderDate >= thirtyDaysAgo)
            .GroupBy(item => new { item.ProductId, item.Product.Name, item.Product.SKU })
            .Select(g => new FastMovingProductDto
            {
                ProductId = g.Key.ProductId,
                ProductName = g.Key.Name,
                ProductSKU = g.Key.SKU,
                QuantitySold = g.Sum(item => item.Quantity),
                SalesValue = g.Sum(item => item.LineTotal),
                DaysSinceLastSale = (int)(DateTime.UtcNow.Date - g.Max(item => item.SalesOrder.OrderDate).Date).TotalDays
            })
            .OrderByDescending(p => p.QuantitySold)
            .Take(10)
            .ToListAsync(cancellationToken);

        analytics.FastMovingProducts = fastMoving;

        // Slow moving products
        var slowMoving = await _context.Products
            .Where(p => p.CompanyId == companyId.Value && !p.IsDeleted)
            .Select(p => new
            {
                Product = p,
                LastSale = _context.SalesOrderItems
                    .Include(item => item.SalesOrder)
                    .Where(item => item.ProductId == p.Id &&
                        item.SalesOrder.CompanyId == companyId.Value &&
                        !item.SalesOrder.IsDeleted)
                    .OrderByDescending(item => item.SalesOrder.OrderDate)
                    .Select(item => (DateTime?)item.SalesOrder.OrderDate)
                    .FirstOrDefault(),
                TotalSales = _context.SalesOrderItems
                    .Include(item => item.SalesOrder)
                    .Where(item => item.ProductId == p.Id &&
                        item.SalesOrder.CompanyId == companyId.Value &&
                        !item.SalesOrder.IsDeleted &&
                        item.SalesOrder.OrderDate >= thirtyDaysAgo)
                    .Sum(item => (decimal?)item.Quantity) ?? 0,
                StockValue = _context.StockLevels
                    .Where(sl => sl.ProductId == p.Id && sl.CompanyId == companyId.Value)
                    .Sum(sl => (decimal?)(sl.Quantity * (sl.AverageCost ?? p.PurchasePrice))) ?? 0
            })
            .Where(x => x.TotalSales < 10 && x.StockValue > 0) // Less than 10 units sold in 30 days
            .OrderBy(x => x.TotalSales)
            .Take(10)
            .ToListAsync(cancellationToken);

        analytics.SlowMovingProducts = slowMoving.Select(x => new SlowMovingProductDto
        {
            ProductId = x.Product.Id,
            ProductName = x.Product.Name,
            ProductSKU = x.Product.SKU,
            QuantitySold = x.TotalSales,
            SalesValue = 0, // Can be calculated if needed
            DaysSinceLastSale = x.LastSale.HasValue 
                ? (int)(DateTime.UtcNow.Date - x.LastSale.Value.Date).TotalDays 
                : 999,
            StockValue = x.StockValue
        }).ToList();

        return Result<InventoryAnalyticsDto>.Success(analytics);
    }
}

