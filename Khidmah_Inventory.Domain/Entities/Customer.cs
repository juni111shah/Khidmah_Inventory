using Khidmah_Inventory.Domain.Common;

namespace Khidmah_Inventory.Domain.Entities;

public class Customer : Entity
{
    public string Name { get; private set; } = string.Empty;
    public string? Code { get; private set; }
    public string? ContactPerson { get; private set; }
    public string? Email { get; private set; }
    public string? PhoneNumber { get; private set; }
    public string? Address { get; private set; }
    public string? City { get; private set; }
    public string? State { get; private set; }
    public string? Country { get; private set; }
    public string? PostalCode { get; private set; }
    public string? TaxId { get; private set; }
    public string? PaymentTerms { get; private set; }
    public decimal? CreditLimit { get; private set; }
    public decimal? Balance { get; private set; } = 0;
    public bool IsActive { get; private set; } = true;
    public string? ImageUrl { get; private set; }

    // Navigation properties
    public virtual ICollection<SalesOrder> SalesOrders { get; private set; } = new List<SalesOrder>();

    private Customer() { }

    public Customer(
        Guid companyId,
        string name,
        string? code = null,
        Guid? createdBy = null) : base(companyId, createdBy)
    {
        Name = name;
        Code = code;
    }

    public void Update(
        string name,
        string? code,
        string? contactPerson,
        string? email,
        string? phoneNumber,
        string? address,
        string? city,
        string? state,
        string? country,
        string? postalCode,
        string? taxId,
        string? paymentTerms,
        decimal? creditLimit,
        Guid? updatedBy = null)
    {
        Name = name;
        Code = code;
        ContactPerson = contactPerson;
        Email = email;
        PhoneNumber = phoneNumber;
        Address = address;
        City = city;
        State = state;
        Country = country;
        PostalCode = postalCode;
        TaxId = taxId;
        PaymentTerms = paymentTerms;
        CreditLimit = creditLimit;
        UpdateAuditInfo(updatedBy);
    }

    public void Activate(Guid? updatedBy = null)
    {
        IsActive = true;
        UpdateAuditInfo(updatedBy);
    }

    public void Deactivate(Guid? updatedBy = null)
    {
        IsActive = false;
        UpdateAuditInfo(updatedBy);
    }

    public void UpdateImage(string? imageUrl, Guid? updatedBy = null)
    {
        ImageUrl = imageUrl;
        UpdateAuditInfo(updatedBy);
    }
}

