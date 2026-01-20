namespace Khidmah_Inventory.Application.Features.Dashboard.Models;

public class DashboardDto
{
    public DashboardSummaryDto Summary { get; set; } = new();
    public List<SalesChartDataDto> SalesChartData { get; set; } = new();
    public List<InventoryChartDataDto> InventoryChartData { get; set; } = new();
    public List<TopProductDto> TopProducts { get; set; } = new();
    public List<LowStockProductDto> LowStockProducts { get; set; } = new();
    public List<RecentOrderDto> RecentOrders { get; set; } = new();
}

public class DashboardSummaryDto
{
    public int TotalProducts { get; set; }
    public int TotalCategories { get; set; }
    public int TotalWarehouses { get; set; }
    public int TotalSuppliers { get; set; }
    public int TotalCustomers { get; set; }
    public decimal TotalStockValue { get; set; }
    public int LowStockItems { get; set; }
    public int PendingPurchaseOrders { get; set; }
    public int PendingSalesOrders { get; set; }
    public decimal TodaySales { get; set; }
    public decimal TodayPurchases { get; set; }
    public decimal MonthlySales { get; set; }
    public decimal MonthlyPurchases { get; set; }
}

public class SalesChartDataDto
{
    public string Date { get; set; } = string.Empty;
    public decimal Sales { get; set; }
    public decimal Purchases { get; set; }
}

public class InventoryChartDataDto
{
    public string CategoryName { get; set; } = string.Empty;
    public decimal StockValue { get; set; }
    public int ProductCount { get; set; }
}

public class TopProductDto
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ProductSKU { get; set; } = string.Empty;
    public decimal TotalSales { get; set; }
    public decimal QuantitySold { get; set; }
}

public class LowStockProductDto
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ProductSKU { get; set; } = string.Empty;
    public string WarehouseName { get; set; } = string.Empty;
    public decimal CurrentStock { get; set; }
    public decimal MinStockLevel { get; set; }
}

public class RecentOrderDto
{
    public Guid Id { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty; // Purchase or Sales
    public string CustomerOrSupplierName { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
}

