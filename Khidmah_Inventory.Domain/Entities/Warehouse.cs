using Khidmah_Inventory.Domain.Common;

namespace Khidmah_Inventory.Domain.Entities;

public class Warehouse : Entity
{
    public string Name { get; private set; } = string.Empty;
    public string? Code { get; private set; }
    public string? Description { get; private set; }
    public string? Address { get; private set; }
    public string? City { get; private set; }
    public string? State { get; private set; }
    public string? Country { get; private set; }
    public string? PostalCode { get; private set; }
    public string? PhoneNumber { get; private set; }
    public string? Email { get; private set; }
    public bool IsDefault { get; private set; } = false;
    public bool IsActive { get; private set; } = true;

    // Navigation properties
    public virtual ICollection<WarehouseZone> Zones { get; private set; } = new List<WarehouseZone>();
    public virtual ICollection<StockTransaction> StockTransactions { get; private set; } = new List<StockTransaction>();

    private Warehouse() { }

    public Warehouse(
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
        string? description,
        string? address,
        string? city,
        string? state,
        string? country,
        string? postalCode,
        string? phoneNumber,
        string? email,
        Guid? updatedBy = null)
    {
        Name = name;
        Code = code;
        Description = description;
        Address = address;
        City = city;
        State = state;
        Country = country;
        PostalCode = postalCode;
        PhoneNumber = phoneNumber;
        Email = email;
        UpdateAuditInfo(updatedBy);
    }

    public void SetAsDefault()
    {
        IsDefault = true;
    }

    public void RemoveDefault()
    {
        IsDefault = false;
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
}

