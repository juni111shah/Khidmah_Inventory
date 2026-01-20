namespace Khidmah_Inventory.Application.Features.Reordering.Models;

public class ReorderSuggestionDto
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ProductSKU { get; set; } = string.Empty;
    public Guid WarehouseId { get; set; }
    public string WarehouseName { get; set; } = string.Empty;
    public decimal CurrentStock { get; set; }
    public decimal? MinStockLevel { get; set; }
    public decimal? ReorderPoint { get; set; }
    public decimal? MaxStockLevel { get; set; }
    public decimal SuggestedQuantity { get; set; }
    public decimal? AverageDailySales { get; set; }
    public int DaysOfStockRemaining { get; set; }
    public string Priority { get; set; } = string.Empty; // Critical, High, Medium, Low
    public List<SupplierSuggestionDto> SupplierSuggestions { get; set; } = new();
}

public class SupplierSuggestionDto
{
    public Guid SupplierId { get; set; }
    public string SupplierName { get; set; } = string.Empty;
    public decimal? LastPurchasePrice { get; set; }
    public int? AverageDeliveryDays { get; set; }
    public int PurchaseCount { get; set; }
    public DateTime? LastPurchaseDate { get; set; }
    public decimal? RecommendedPrice { get; set; }
    public int Score { get; set; } // Higher is better
}

