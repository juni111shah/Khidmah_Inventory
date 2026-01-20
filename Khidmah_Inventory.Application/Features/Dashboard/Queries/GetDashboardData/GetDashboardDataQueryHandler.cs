using MediatR;
using Microsoft.EntityFrameworkCore;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Dashboard.Models;

namespace Khidmah_Inventory.Application.Features.Dashboard.Queries.GetDashboardData;

public class GetDashboardDataQueryHandler : IRequestHandler<GetDashboardDataQuery, Result<DashboardDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetDashboardDataQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<DashboardDto>> Handle(GetDashboardDataQuery request, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId;
        if (!companyId.HasValue)
            return Result<DashboardDto>.Failure("Company context is required");

        var today = DateTime.UtcNow.Date;
        var monthStart = new DateTime(today.Year, today.Month, 1);
        var monthEnd = monthStart.AddMonths(1).AddDays(-1);

        var fromDate = request.FromDate ?? monthStart;
        var toDate = request.ToDate ?? monthEnd;

        var dashboard = new DashboardDto();

        // Summary Statistics
        dashboard.Summary.TotalProducts = await _context.Products
            .CountAsync(p => p.CompanyId == companyId.Value && !p.IsDeleted, cancellationToken);

        dashboard.Summary.TotalCategories = await _context.Categories
            .CountAsync(c => c.CompanyId == companyId.Value && !c.IsDeleted, cancellationToken);

        dashboard.Summary.TotalWarehouses = await _context.Warehouses
            .CountAsync(w => w.CompanyId == companyId.Value && !w.IsDeleted, cancellationToken);

        dashboard.Summary.TotalSuppliers = await _context.Suppliers
            .CountAsync(s => s.CompanyId == companyId.Value && !s.IsDeleted, cancellationToken);

        dashboard.Summary.TotalCustomers = await _context.Customers
            .CountAsync(c => c.CompanyId == companyId.Value && !c.IsDeleted, cancellationToken);

        // Stock Value
        dashboard.Summary.TotalStockValue = await _context.StockLevels
            .Include(sl => sl.Product)
            .Where(sl => sl.CompanyId == companyId.Value)
            .SumAsync(sl => sl.Quantity * (sl.AverageCost ?? sl.Product.PurchasePrice), cancellationToken);

        // Low Stock Items
        dashboard.Summary.LowStockItems = await _context.StockLevels
            .Include(sl => sl.Product)
            .Where(sl => sl.CompanyId == companyId.Value &&
                sl.Product.MinStockLevel.HasValue &&
                sl.Quantity <= sl.Product.MinStockLevel.Value)
            .CountAsync(cancellationToken);

        // Pending Orders
        dashboard.Summary.PendingPurchaseOrders = await _context.PurchaseOrders
            .CountAsync(po => po.CompanyId == companyId.Value && 
                !po.IsDeleted && 
                po.Status != "Completed" && 
                po.Status != "Cancelled", cancellationToken);

        dashboard.Summary.PendingSalesOrders = await _context.SalesOrders
            .CountAsync(so => so.CompanyId == companyId.Value && 
                !so.IsDeleted && 
                so.Status != "Delivered" && 
                so.Status != "Invoiced" && 
                so.Status != "Cancelled", cancellationToken);

        // Today's Sales and Purchases
        dashboard.Summary.TodaySales = await _context.SalesOrders
            .Where(so => so.CompanyId == companyId.Value && 
                !so.IsDeleted && 
                so.OrderDate.Date == today)
            .SumAsync(so => so.TotalAmount, cancellationToken);

        dashboard.Summary.TodayPurchases = await _context.PurchaseOrders
            .Where(po => po.CompanyId == companyId.Value && 
                !po.IsDeleted && 
                po.OrderDate.Date == today)
            .SumAsync(po => po.TotalAmount, cancellationToken);

        // Monthly Sales and Purchases
        dashboard.Summary.MonthlySales = await _context.SalesOrders
            .Where(so => so.CompanyId == companyId.Value && 
                !so.IsDeleted && 
                so.OrderDate >= monthStart && 
                so.OrderDate <= monthEnd)
            .SumAsync(so => so.TotalAmount, cancellationToken);

        dashboard.Summary.MonthlyPurchases = await _context.PurchaseOrders
            .Where(po => po.CompanyId == companyId.Value && 
                !po.IsDeleted && 
                po.OrderDate >= monthStart && 
                po.OrderDate <= monthEnd)
            .SumAsync(po => po.TotalAmount, cancellationToken);

        // Sales Chart Data (Last 30 days)
        var chartStartDate = DateTime.UtcNow.AddDays(-30).Date;
        var salesByDate = await _context.SalesOrders
            .Where(so => so.CompanyId == companyId.Value && 
                !so.IsDeleted && 
                so.OrderDate >= chartStartDate)
            .GroupBy(so => so.OrderDate.Date)
            .Select(g => new { Date = g.Key, Sales = g.Sum(so => so.TotalAmount) })
            .ToListAsync(cancellationToken);

        var purchasesByDate = await _context.PurchaseOrders
            .Where(po => po.CompanyId == companyId.Value && 
                !po.IsDeleted && 
                po.OrderDate >= chartStartDate)
            .GroupBy(po => po.OrderDate.Date)
            .Select(g => new { Date = g.Key, Purchases = g.Sum(po => po.TotalAmount) })
            .ToListAsync(cancellationToken);

        var allDates = salesByDate.Select(s => s.Date)
            .Union(purchasesByDate.Select(p => p.Date))
            .OrderBy(d => d)
            .ToList();

        dashboard.SalesChartData = allDates.Select(date => new SalesChartDataDto
        {
            Date = date.ToString("yyyy-MM-dd"),
            Sales = salesByDate.FirstOrDefault(s => s.Date == date)?.Sales ?? 0,
            Purchases = purchasesByDate.FirstOrDefault(p => p.Date == date)?.Purchases ?? 0
        }).ToList();

        // Inventory Chart Data by Category
        dashboard.InventoryChartData = await _context.StockLevels
            .Include(sl => sl.Product)
                .ThenInclude(p => p.Category)
            .Where(sl => sl.CompanyId == companyId.Value)
            .GroupBy(sl => sl.Product.Category != null ? sl.Product.Category.Name : "Uncategorized")
            .Select(g => new InventoryChartDataDto
            {
                CategoryName = g.Key,
                StockValue = g.Sum(sl => sl.Quantity * (sl.AverageCost ?? sl.Product.PurchasePrice)),
                ProductCount = g.Select(sl => sl.ProductId).Distinct().Count()
            })
            .ToListAsync(cancellationToken);

        // Top Products (by sales quantity) - Use last 30 days like the chart
        var topProductsStartDate = DateTime.UtcNow.AddDays(-30).Date;
        dashboard.TopProducts = await _context.SalesOrderItems
            .Include(item => item.Product)
            .Include(item => item.SalesOrder)
            .Where(item => item.SalesOrder.CompanyId == companyId.Value && 
                !item.SalesOrder.IsDeleted &&
                item.SalesOrder.OrderDate >= topProductsStartDate)
            .GroupBy(item => new { item.ProductId, item.Product.Name, item.Product.SKU })
            .Select(g => new TopProductDto
            {
                ProductId = g.Key.ProductId,
                ProductName = g.Key.Name,
                ProductSKU = g.Key.SKU,
                TotalSales = g.Sum(item => item.LineTotal),
                QuantitySold = g.Sum(item => item.Quantity)
            })
            .OrderByDescending(p => p.QuantitySold)
            .Take(10)
            .ToListAsync(cancellationToken);

        // Low Stock Products
        dashboard.LowStockProducts = await _context.StockLevels
            .Include(sl => sl.Product)
            .Include(sl => sl.Warehouse)
            .Where(sl => sl.CompanyId == companyId.Value &&
                sl.Product.MinStockLevel.HasValue &&
                sl.Quantity <= sl.Product.MinStockLevel.Value)
            .Select(sl => new LowStockProductDto
            {
                ProductId = sl.ProductId,
                ProductName = sl.Product.Name,
                ProductSKU = sl.Product.SKU,
                WarehouseName = sl.Warehouse.Name,
                CurrentStock = sl.Quantity,
                MinStockLevel = sl.Product.MinStockLevel!.Value
            })
            .Take(10)
            .ToListAsync(cancellationToken);

        // Recent Orders
        var recentPurchaseOrders = await _context.PurchaseOrders
            .Include(po => po.Supplier)
            .Where(po => po.CompanyId == companyId.Value && !po.IsDeleted)
            .OrderByDescending(po => po.OrderDate)
            .Take(5)
            .Select(po => new RecentOrderDto
            {
                Id = po.Id,
                OrderNumber = po.OrderNumber,
                Type = "Purchase",
                CustomerOrSupplierName = po.Supplier.Name,
                TotalAmount = po.TotalAmount,
                Status = po.Status,
                OrderDate = po.OrderDate
            })
            .ToListAsync(cancellationToken);

        var recentSalesOrders = await _context.SalesOrders
            .Include(so => so.Customer)
            .Where(so => so.CompanyId == companyId.Value && !so.IsDeleted)
            .OrderByDescending(so => so.OrderDate)
            .Take(5)
            .Select(so => new RecentOrderDto
            {
                Id = so.Id,
                OrderNumber = so.OrderNumber,
                Type = "Sales",
                CustomerOrSupplierName = so.Customer.Name,
                TotalAmount = so.TotalAmount,
                Status = so.Status,
                OrderDate = so.OrderDate
            })
            .ToListAsync(cancellationToken);

        dashboard.RecentOrders = recentPurchaseOrders
            .Concat(recentSalesOrders)
            .OrderByDescending(o => o.OrderDate)
            .Take(10)
            .ToList();

        return Result<DashboardDto>.Success(dashboard);
    }
}

