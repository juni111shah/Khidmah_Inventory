using Khidmah_Inventory.Domain.Common;

namespace Khidmah_Inventory.Domain.Entities;

public class StockLevel : BaseEntity
{
    public Guid ProductId { get; private set; }
    public Guid WarehouseId { get; private set; }
    public decimal Quantity { get; private set; }
    public decimal? ReservedQuantity { get; private set; } = 0;
    public decimal AvailableQuantity => Quantity - (ReservedQuantity ?? 0);
    public decimal? AverageCost { get; private set; }
    public DateTime LastUpdated { get; private set; } = DateTime.UtcNow;

    // Navigation properties
    public virtual Product Product { get; private set; } = null!;
    public virtual Warehouse Warehouse { get; private set; } = null!;

    private StockLevel() { }

    public StockLevel(
        Guid companyId,
        Guid productId,
        Guid warehouseId,
        decimal quantity,
        Guid? createdBy = null) : base(companyId, createdBy)
    {
        ProductId = productId;
        WarehouseId = warehouseId;
        Quantity = quantity;
    }

    public void AdjustQuantity(decimal quantity, decimal? averageCost = null)
    {
        Quantity += quantity;
        if (averageCost.HasValue)
        {
            AverageCost = averageCost.Value;
        }
        LastUpdated = DateTime.UtcNow;
    }

    public void ReserveQuantity(decimal quantity)
    {
        ReservedQuantity = (ReservedQuantity ?? 0) + quantity;
        LastUpdated = DateTime.UtcNow;
    }

    public void ReleaseReservedQuantity(decimal quantity)
    {
        ReservedQuantity = Math.Max(0, (ReservedQuantity ?? 0) - quantity);
        LastUpdated = DateTime.UtcNow;
    }

    public void SetQuantity(decimal quantity, decimal? averageCost = null)
    {
        Quantity = quantity;
        if (averageCost.HasValue)
        {
            AverageCost = averageCost.Value;
        }
        LastUpdated = DateTime.UtcNow;
    }
}

