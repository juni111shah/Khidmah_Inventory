using Khidmah_Inventory.Domain.Common;

namespace Khidmah_Inventory.Domain.Entities;

public class StockTransaction : Entity
{
    public Guid ProductId { get; private set; }
    public Guid WarehouseId { get; private set; }
    public string TransactionType { get; private set; } = string.Empty; // StockIn, StockOut, Adjustment, Transfer
    public decimal Quantity { get; private set; }
    public decimal? UnitCost { get; private set; }
    public decimal? TotalCost { get; private set; }
    public string? ReferenceNumber { get; private set; }
    public string? ReferenceType { get; private set; } // PO, SO, GRN, etc.
    public Guid? ReferenceId { get; private set; }
    public string? BatchNumber { get; private set; }
    public DateTime? ExpiryDate { get; private set; }
    public string? Notes { get; private set; }
    public DateTime TransactionDate { get; private set; } = DateTime.UtcNow;
    public decimal BalanceAfter { get; private set; } // Stock balance after this transaction

    // Navigation properties
    public virtual Product Product { get; private set; } = null!;
    public virtual Warehouse Warehouse { get; private set; } = null!;

    private StockTransaction() { }

    public StockTransaction(
        Guid companyId,
        Guid productId,
        Guid warehouseId,
        string transactionType,
        decimal quantity,
        decimal? unitCost = null,
        Guid? createdBy = null) : base(companyId, createdBy)
    {
        ProductId = productId;
        WarehouseId = warehouseId;
        TransactionType = transactionType;
        Quantity = quantity;
        UnitCost = unitCost;
        TotalCost = unitCost.HasValue ? quantity * unitCost.Value : null;
    }

    public void SetReference(string referenceType, Guid? referenceId, string? referenceNumber = null)
    {
        ReferenceType = referenceType;
        ReferenceId = referenceId;
        ReferenceNumber = referenceNumber;
    }

    public void SetBatchInfo(string? batchNumber, DateTime? expiryDate)
    {
        BatchNumber = batchNumber;
        ExpiryDate = expiryDate;
    }

    public void SetBalanceAfter(decimal balance)
    {
        BalanceAfter = balance;
    }
}

