namespace Khidmah_Inventory.Application.Features.Analytics.Models;

public class AnalyticsRequestDto
{
    public TimeRangeType TimeRange { get; set; } = TimeRangeType.Last30Days;
    public DateTime? CustomFromDate { get; set; }
    public DateTime? CustomToDate { get; set; }
    public AnalyticsType AnalyticsType { get; set; } = AnalyticsType.Sales;
    public string? GroupBy { get; set; } // Day, Week, Month, Category, Product, etc.
    public List<string>? Metrics { get; set; } // Sales, Profit, Quantity, etc.
}

public enum TimeRangeType
{
    Today,
    Yesterday,
    Last7Days,
    Last30Days,
    ThisMonth,
    LastMonth,
    ThisQuarter,
    LastQuarter,
    ThisYear,
    LastYear,
    Custom
}

public enum AnalyticsType
{
    Sales,
    Purchase,
    Inventory,
    Profit,
    Products,
    Customers,
    Suppliers
}

public class SalesAnalyticsDto
{
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public decimal TotalSales { get; set; }
    public decimal TotalCost { get; set; }
    public decimal TotalProfit { get; set; }
    public decimal ProfitMargin { get; set; }
    public int TotalOrders { get; set; }
    public decimal AverageOrderValue { get; set; }
    public List<TimeSeriesDataDto> TimeSeriesData { get; set; } = new();
    public List<CategoryAnalyticsDto> CategoryBreakdown { get; set; } = new();
    public List<ProductAnalyticsDto> TopProducts { get; set; } = new();
    public List<CustomerAnalyticsDto> TopCustomers { get; set; } = new();
}

public class PurchaseAnalyticsDto
{
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public decimal TotalPurchases { get; set; }
    public int TotalOrders { get; set; }
    public decimal AverageOrderValue { get; set; }
    public List<TimeSeriesDataDto> TimeSeriesData { get; set; } = new();
    public List<SupplierAnalyticsDto> TopSuppliers { get; set; } = new();
    public List<ProductAnalyticsDto> TopProducts { get; set; } = new();
}

public class InventoryAnalyticsDto
{
    public decimal TotalStockValue { get; set; }
    public int TotalProducts { get; set; }
    public int LowStockItems { get; set; }
    public int OutOfStockItems { get; set; }
    public decimal AverageStockValue { get; set; }
    public List<CategoryStockValueDto> CategoryStockValues { get; set; } = new();
    public List<WarehouseStockValueDto> WarehouseStockValues { get; set; } = new();
    public List<FastMovingProductDto> FastMovingProducts { get; set; } = new();
    public List<SlowMovingProductDto> SlowMovingProducts { get; set; } = new();
}

public class ProfitAnalyticsDto
{
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal TotalCost { get; set; }
    public decimal TotalProfit { get; set; }
    public decimal ProfitMargin { get; set; }
    public decimal GrossProfitMargin { get; set; }
    public List<TimeSeriesDataDto> ProfitTrend { get; set; } = new();
    public List<CategoryProfitDto> CategoryProfits { get; set; } = new();
}

public class TimeSeriesDataDto
{
    public string Label { get; set; } = string.Empty; // Date or period label
    public DateTime Date { get; set; }
    public decimal Value { get; set; }
    public decimal? SecondaryValue { get; set; } // For comparison (e.g., Sales vs Purchases)
}

public class CategoryAnalyticsDto
{
    public string CategoryName { get; set; } = string.Empty;
    public decimal TotalSales { get; set; }
    public decimal TotalCost { get; set; }
    public decimal TotalProfit { get; set; }
    public int OrderCount { get; set; }
    public decimal Percentage { get; set; }
}

public class ProductAnalyticsDto
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ProductSKU { get; set; } = string.Empty;
    public decimal TotalSales { get; set; }
    public decimal TotalCost { get; set; }
    public decimal TotalProfit { get; set; }
    public decimal QuantitySold { get; set; }
    public decimal AveragePrice { get; set; }
}

public class CustomerAnalyticsDto
{
    public Guid CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public decimal TotalSales { get; set; }
    public int OrderCount { get; set; }
    public decimal AverageOrderValue { get; set; }
    public decimal Percentage { get; set; }
}

public class SupplierAnalyticsDto
{
    public Guid SupplierId { get; set; }
    public string SupplierName { get; set; } = string.Empty;
    public decimal TotalPurchases { get; set; }
    public int OrderCount { get; set; }
    public decimal AverageOrderValue { get; set; }
    public decimal Percentage { get; set; }
}

public class CategoryStockValueDto
{
    public string CategoryName { get; set; } = string.Empty;
    public decimal StockValue { get; set; }
    public int ProductCount { get; set; }
    public decimal Percentage { get; set; }
}

public class WarehouseStockValueDto
{
    public string WarehouseName { get; set; } = string.Empty;
    public decimal StockValue { get; set; }
    public int ProductCount { get; set; }
    public decimal Percentage { get; set; }
}

public class FastMovingProductDto
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ProductSKU { get; set; } = string.Empty;
    public decimal QuantitySold { get; set; }
    public decimal SalesValue { get; set; }
    public int DaysSinceLastSale { get; set; }
}

public class SlowMovingProductDto
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ProductSKU { get; set; } = string.Empty;
    public decimal QuantitySold { get; set; }
    public decimal SalesValue { get; set; }
    public int DaysSinceLastSale { get; set; }
    public decimal StockValue { get; set; }
}

public class CategoryProfitDto
{
    public string CategoryName { get; set; } = string.Empty;
    public decimal Revenue { get; set; }
    public decimal Cost { get; set; }
    public decimal Profit { get; set; }
    public decimal ProfitMargin { get; set; }
    public decimal Percentage { get; set; }
}

