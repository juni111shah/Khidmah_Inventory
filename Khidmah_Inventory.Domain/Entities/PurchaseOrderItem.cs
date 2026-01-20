using Khidmah_Inventory.Domain.Common;

namespace Khidmah_Inventory.Domain.Entities;

public class PurchaseOrderItem : BaseEntity
{
    public Guid PurchaseOrderId { get; private set; }
    public Guid ProductId { get; private set; }
    public decimal Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public decimal DiscountPercent { get; private set; }
    public decimal DiscountAmount { get; private set; }
    public decimal TaxPercent { get; private set; }
    public decimal TaxAmount { get; private set; }
    public decimal LineTotal { get; private set; }
    public decimal? ReceivedQuantity { get; private set; } = 0;
    public string? Notes { get; private set; }

    // Navigation properties
    public virtual PurchaseOrder PurchaseOrder { get; private set; } = null!;
    public virtual Product Product { get; private set; } = null!;

    private PurchaseOrderItem() { }

    public PurchaseOrderItem(
        Guid companyId,
        Guid purchaseOrderId,
        Guid productId,
        decimal quantity,
        decimal unitPrice,
        Guid? createdBy = null) : base(companyId, createdBy)
    {
        PurchaseOrderId = purchaseOrderId;
        ProductId = productId;
        Quantity = quantity;
        UnitPrice = unitPrice;
        CalculateLineTotal();
    }

    public void Update(
        decimal quantity,
        decimal unitPrice,
        decimal discountPercent,
        decimal taxPercent,
        string? notes,
        Guid? updatedBy = null)
    {
        Quantity = quantity;
        UnitPrice = unitPrice;
        DiscountPercent = discountPercent;
        TaxPercent = taxPercent;
        Notes = notes;
        CalculateLineTotal();
        UpdateAuditInfo(updatedBy);
    }

    public void RecordReceivedQuantity(decimal quantity)
    {
        ReceivedQuantity = (ReceivedQuantity ?? 0) + quantity;
    }

    private void CalculateLineTotal()
    {
        var subtotal = Quantity * UnitPrice;
        DiscountAmount = subtotal * (DiscountPercent / 100);
        var afterDiscount = subtotal - DiscountAmount;
        TaxAmount = afterDiscount * (TaxPercent / 100);
        LineTotal = afterDiscount + TaxAmount;
    }
}

