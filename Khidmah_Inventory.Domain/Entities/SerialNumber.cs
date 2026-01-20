using Khidmah_Inventory.Domain.Common;

namespace Khidmah_Inventory.Domain.Entities;

public class SerialNumber : Entity
{
    public Guid ProductId { get; private set; }
    public Guid WarehouseId { get; private set; }
    public string SerialNumberValue { get; private set; } = string.Empty;
    public string? BatchNumber { get; private set; }
    public Guid? BatchId { get; private set; }
    public DateTime? ManufactureDate { get; private set; }
    public DateTime? ExpiryDate { get; private set; }
    public string Status { get; private set; } = "InStock"; // InStock, Sold, Returned, Damaged, Recalled
    public Guid? SalesOrderId { get; private set; }
    public Guid? SalesOrderItemId { get; private set; }
    public Guid? CustomerId { get; private set; }
    public DateTime? SoldDate { get; private set; }
    public string? WarrantyExpiryDate { get; private set; }
    public string? Notes { get; private set; }
    public bool IsRecalled { get; private set; } = false;
    public DateTime? RecallDate { get; private set; }
    public string? RecallReason { get; private set; }

    // Navigation properties
    public virtual Product Product { get; private set; } = null!;
    public virtual Warehouse Warehouse { get; private set; } = null!;
    public virtual Batch? Batch { get; private set; }
    public virtual SalesOrder? SalesOrder { get; private set; }
    public virtual SalesOrderItem? SalesOrderItem { get; private set; }
    public virtual Customer? Customer { get; private set; }

    private SerialNumber() { }

    public SerialNumber(
        Guid companyId,
        Guid productId,
        Guid warehouseId,
        string serialNumberValue,
        Guid? createdBy = null) : base(companyId, createdBy)
    {
        ProductId = productId;
        WarehouseId = warehouseId;
        SerialNumberValue = serialNumberValue;
    }

    public void AssignToBatch(Guid? batchId, string? batchNumber)
    {
        BatchId = batchId;
        BatchNumber = batchNumber;
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

    public void Sell(Guid salesOrderId, Guid salesOrderItemId, Guid customerId, DateTime soldDate)
    {
        Status = "Sold";
        SalesOrderId = salesOrderId;
        SalesOrderItemId = salesOrderItemId;
        CustomerId = customerId;
        SoldDate = soldDate;
        UpdateAuditInfo();
    }

    public void Return()
    {
        Status = "Returned";
        SalesOrderId = null;
        SalesOrderItemId = null;
        CustomerId = null;
        SoldDate = null;
        UpdateAuditInfo();
    }

    public void MarkAsDamaged(string? notes = null)
    {
        Status = "Damaged";
        Notes = notes;
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

    public void SetWarrantyExpiry(string? warrantyExpiryDate)
    {
        WarrantyExpiryDate = warrantyExpiryDate;
        UpdateAuditInfo();
    }

    public bool IsExpired => ExpiryDate.HasValue && ExpiryDate.Value < DateTime.UtcNow;
    public bool IsExpiringSoon => ExpiryDate.HasValue && ExpiryDate.Value <= DateTime.UtcNow.AddDays(30) && ExpiryDate.Value > DateTime.UtcNow;
}

