using Khidmah_Inventory.Domain.Common;

namespace Khidmah_Inventory.Domain.Entities;

public class Batch : Entity
{
    public Guid ProductId { get; private set; }
    public Guid WarehouseId { get; private set; }
    public string BatchNumber { get; private set; } = string.Empty;
    public string? LotNumber { get; private set; }
    public DateTime? ManufactureDate { get; private set; }
    public DateTime? ExpiryDate { get; private set; }
    public decimal Quantity { get; private set; }
    public decimal? UnitCost { get; private set; }
    public string? SupplierName { get; private set; }
    public string? SupplierBatchNumber { get; private set; }
    public string? Notes { get; private set; }
    public bool IsRecalled { get; private set; } = false;
    public DateTime? RecallDate { get; private set; }
    public string? RecallReason { get; private set; }

    // Navigation properties
    public virtual Product Product { get; private set; } = null!;
    public virtual Warehouse Warehouse { get; private set; } = null!;

    private Batch() { }

    public Batch(
        Guid companyId,
        Guid productId,
        Guid warehouseId,
        string batchNumber,
        decimal quantity,
        Guid? createdBy = null) : base(companyId, createdBy)
    {
        ProductId = productId;
        WarehouseId = warehouseId;
        BatchNumber = batchNumber;
        Quantity = quantity;
    }

    public void UpdateQuantity(decimal quantity)
    {
        Quantity = quantity;
        UpdateAuditInfo();
    }

    public void SetExpiryDate(DateTime? expiryDate)
    {
        ExpiryDate = expiryDate;
        UpdateAuditInfo();
    }

    public void SetManufactureDate(DateTime? manufactureDate)
    {
        ManufactureDate = manufactureDate;
        UpdateAuditInfo();
    }

    public void SetLotNumber(string? lotNumber)
    {
        LotNumber = lotNumber;
        UpdateAuditInfo();
    }

    public void SetSupplierInfo(string? supplierName, string? supplierBatchNumber)
    {
        SupplierName = supplierName;
        SupplierBatchNumber = supplierBatchNumber;
        UpdateAuditInfo();
    }

    public void SetCost(decimal? unitCost)
    {
        UnitCost = unitCost;
        UpdateAuditInfo();
    }

    public void Recall(string reason, Guid? recalledBy = null)
    {
        IsRecalled = true;
        RecallDate = DateTime.UtcNow;
        RecallReason = reason;
        UpdateAuditInfo(recalledBy);
    }

    public void ClearRecall()
    {
        IsRecalled = false;
        RecallDate = null;
        RecallReason = null;
        UpdateAuditInfo();
    }

    public bool IsExpired => ExpiryDate.HasValue && ExpiryDate.Value < DateTime.UtcNow;
    public bool IsExpiringSoon => ExpiryDate.HasValue && ExpiryDate.Value <= DateTime.UtcNow.AddDays(30) && ExpiryDate.Value > DateTime.UtcNow;
    public int? DaysUntilExpiry => ExpiryDate.HasValue ? (int?)(ExpiryDate.Value - DateTime.UtcNow).TotalDays : null;
}

