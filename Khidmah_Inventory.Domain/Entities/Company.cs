using Khidmah_Inventory.Domain.Common;

namespace Khidmah_Inventory.Domain.Entities;

public class Company : Entity
{
    public string Name { get; private set; } = string.Empty;
    public string? LegalName { get; private set; }
    public string? TaxId { get; private set; }
    public string? RegistrationNumber { get; private set; }
    public string? Email { get; private set; }
    public string? PhoneNumber { get; private set; }
    public string? Address { get; private set; }
    public string? City { get; private set; }
    public string? State { get; private set; }
    public string? Country { get; private set; }
    public string? PostalCode { get; private set; }
    public string? LogoUrl { get; private set; }
    public string? Currency { get; private set; } = "USD";
    public string? TimeZone { get; private set; }
    public bool IsActive { get; private set; } = true;
    public DateTime? SubscriptionExpiresAt { get; private set; }
    public string? SubscriptionPlan { get; private set; }

    // Navigation properties
    public virtual ICollection<UserCompany> UserCompanies { get; private set; } = new List<UserCompany>();

    private Company() { }

    public Company(
        Guid companyId,
        string name,
        string? email = null,
        Guid? createdBy = null) : base(companyId, createdBy)
    {
        Name = name;
        Email = email;
    }

    public void Update(
        string name,
        string? legalName,
        string? taxId,
        string? registrationNumber,
        string? email,
        string? phoneNumber,
        string? address,
        string? city,
        string? state,
        string? country,
        string? postalCode,
        string? currency,
        string? timeZone,
        Guid? updatedBy = null)
    {
        Name = name;
        LegalName = legalName;
        TaxId = taxId;
        RegistrationNumber = registrationNumber;
        Email = email;
        PhoneNumber = phoneNumber;
        Address = address;
        City = city;
        State = state;
        Country = country;
        PostalCode = postalCode;
        Currency = currency;
        TimeZone = timeZone;
        UpdateAuditInfo(updatedBy);
    }

    public void UpdateLogo(string? logoUrl, Guid? updatedBy = null)
    {
        LogoUrl = logoUrl;
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

    public void UpdateSubscription(string? plan, DateTime? expiresAt, Guid? updatedBy = null)
    {
        SubscriptionPlan = plan;
        SubscriptionExpiresAt = expiresAt;
        UpdateAuditInfo(updatedBy);
    }
}

