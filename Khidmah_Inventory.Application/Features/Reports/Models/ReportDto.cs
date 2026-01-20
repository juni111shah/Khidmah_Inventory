namespace Khidmah_Inventory.Application.Features.Reports.Models;

public class SalesReportDto
{
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public decimal TotalSales { get; set; }
    public decimal TotalCost { get; set; }
    public decimal TotalProfit { get; set; }
    public decimal ProfitMargin { get; set; }
    public int TotalOrders { get; set; }
    public List<SalesReportItemDto> Items { get; set; } = new();
}

public class SalesReportItemDto
{
    public DateTime Date { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public decimal Cost { get; set; }
    public decimal Profit { get; set; }
}

public class InventoryReportDto
{
    public decimal TotalStockValue { get; set; }
    public int TotalProducts { get; set; }
    public int LowStockItems { get; set; }
    public int OutOfStockItems { get; set; }
    public List<InventoryReportItemDto> Items { get; set; } = new();
}

public class InventoryReportItemDto
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ProductSKU { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public string WarehouseName { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal AverageCost { get; set; }
    public decimal StockValue { get; set; }
    public decimal? MinStockLevel { get; set; }
    public decimal? MaxStockLevel { get; set; }
    public string Status { get; set; } = string.Empty; // Normal, Low, Out
}

public class PurchaseReportDto
{
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public decimal TotalPurchases { get; set; }
    public int TotalOrders { get; set; }
    public List<PurchaseReportItemDto> Items { get; set; } = new();
}

public class PurchaseReportItemDto
{
    public DateTime Date { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public string SupplierName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Status { get; set; } = string.Empty;
}

