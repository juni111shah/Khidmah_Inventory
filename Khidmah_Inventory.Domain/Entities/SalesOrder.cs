using System.Linq;
using Khidmah_Inventory.Domain.Common;

namespace Khidmah_Inventory.Domain.Entities;

public class SalesOrder : Entity
{
    public string OrderNumber { get; private set; } = string.Empty;
    public Guid CustomerId { get; private set; }
    public DateTime OrderDate { get; private set; } = DateTime.UtcNow;
    public DateTime? ExpectedDeliveryDate { get; private set; }
    public string Status { get; private set; } = "Draft"; // Draft, Confirmed, PartiallyDelivered, Delivered, Invoiced, Cancelled
    public decimal SubTotal { get; private set; }
    public decimal TaxAmount { get; private set; }
    public decimal DiscountAmount { get; private set; }
    public decimal TotalAmount { get; private set; }
    public string? Notes { get; private set; }
    public string? TermsAndConditions { get; private set; }

    // Navigation properties
    public virtual Customer Customer { get; private set; } = null!;
    public virtual ICollection<SalesOrderItem> Items { get; private set; } = new List<SalesOrderItem>();

    private SalesOrder() { }

    public SalesOrder(
        Guid companyId,
        string orderNumber,
        Guid customerId,
        DateTime orderDate,
        Guid? createdBy = null) : base(companyId, createdBy)
    {
        OrderNumber = orderNumber;
        CustomerId = customerId;
        OrderDate = orderDate;
    }

    public void Update(
        Guid customerId,
        DateTime orderDate,
        DateTime? expectedDeliveryDate,
        string? notes,
        string? termsAndConditions,
        Guid? updatedBy = null)
    {
        CustomerId = customerId;
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
}

