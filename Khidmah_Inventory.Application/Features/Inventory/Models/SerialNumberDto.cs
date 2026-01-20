namespace Khidmah_Inventory.Application.Features.Inventory.Models;

public class SerialNumberDto
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ProductSKU { get; set; } = string.Empty;
    public Guid WarehouseId { get; set; }
    public string WarehouseName { get; set; } = string.Empty;
    public string SerialNumberValue { get; set; } = string.Empty;
    public string? BatchNumber { get; set; }
    public Guid? BatchId { get; set; }
    public DateTime? ManufactureDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public string Status { get; set; } = string.Empty; // InStock, Sold, Returned, Damaged, Recalled
    public Guid? SalesOrderId { get; set; }
    public string? SalesOrderNumber { get; set; }
    public Guid? CustomerId { get; set; }
    public string? CustomerName { get; set; }
    public DateTime? SoldDate { get; set; }
    public string? WarrantyExpiryDate { get; set; }
    public string? Notes { get; set; }
    public bool IsRecalled { get; set; }
    public DateTime? RecallDate { get; set; }
    public string? RecallReason { get; set; }
    public bool IsExpired { get; set; }
    public bool IsExpiringSoon { get; set; }
    public DateTime CreatedAt { get; set; }
}

