namespace Khidmah_Inventory.Application.Features.Inventory.Models;

public class BatchDto
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ProductSKU { get; set; } = string.Empty;
    public Guid WarehouseId { get; set; }
    public string WarehouseName { get; set; } = string.Empty;
    public string BatchNumber { get; set; } = string.Empty;
    public string? LotNumber { get; set; }
    public DateTime? ManufactureDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public decimal Quantity { get; set; }
    public decimal? UnitCost { get; set; }
    public string? SupplierName { get; set; }
    public string? SupplierBatchNumber { get; set; }
    public string? Notes { get; set; }
    public bool IsRecalled { get; set; }
    public DateTime? RecallDate { get; set; }
    public string? RecallReason { get; set; }
    public bool IsExpired { get; set; }
    public bool IsExpiringSoon { get; set; }
    public int? DaysUntilExpiry { get; set; }
    public DateTime CreatedAt { get; set; }
}

