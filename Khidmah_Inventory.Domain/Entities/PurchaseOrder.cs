using System.Linq;
using Khidmah_Inventory.Domain.Common;

namespace Khidmah_Inventory.Domain.Entities;

public class PurchaseOrder : Entity
{
    public string OrderNumber { get; private set; } = string.Empty;
    public Guid SupplierId { get; private set; }
    public DateTime OrderDate { get; private set; } = DateTime.UtcNow;
    public DateTime? ExpectedDeliveryDate { get; private set; }
    public string Status { get; private set; } = "Draft"; // Draft, Sent, PartiallyReceived, Completed, Cancelled
    public decimal SubTotal { get; private set; }
    public decimal TaxAmount { get; private set; }
    public decimal DiscountAmount { get; private set; }
    public decimal TotalAmount { get; private set; }
    public string? Notes { get; private set; }
    public string? TermsAndConditions { get; private set; }

    // Navigation properties
    public virtual Supplier Supplier { get; private set; } = null!;
    public virtual ICollection<PurchaseOrderItem> Items { get; private set; } = new List<PurchaseOrderItem>();

    private PurchaseOrder() { }

    public PurchaseOrder(
        Guid companyId,
        string orderNumber,
        Guid supplierId,
        DateTime orderDate,
        Guid? createdBy = null) : base(companyId, createdBy)
    {
        OrderNumber = orderNumber;
        SupplierId = supplierId;
        OrderDate = orderDate;
    }

    public void Update(
        Guid supplierId,
        DateTime orderDate,
        DateTime? expectedDeliveryDate,
        string? notes,
        string? termsAndConditions,
        Guid? updatedBy = null)
    {
        SupplierId = supplierId;
        OrderDate = orderDate;
        ExpectedDeliveryDate = expectedDeliveryDate;
        Notes = notes;
        TermsAndConditions = termsAndConditions;
        UpdateAuditInfo(updatedBy);
    }

    public void CalculateTotals()
    {
        SubTotal = Items.Sum(x => x.Quantity * x.UnitPrice);
        DiscountAmount = Items.Sum(x => x.DiscountAmount);
        TaxAmount = Items.Sum(x => x.TaxAmount);
        TotalAmount = SubTotal - DiscountAmount + TaxAmount;
    }

    public void UpdateStatus(string status, Guid? updatedBy = null)
    {
        Status = status;
        UpdateAuditInfo(updatedBy);
    }

    public void SetStatus(string status, Guid? updatedBy = null)
    {
        UpdateStatus(status, updatedBy);
    }

    public void SetExpectedDeliveryDate(DateTime expectedDeliveryDate, Guid? updatedBy = null)
    {
        ExpectedDeliveryDate = expectedDeliveryDate;
        UpdateAuditInfo(updatedBy);
    }

    public void AddItem(PurchaseOrderItem item)
    {
        Items.Add(item);
    }
}

