using Khidmah_Inventory.Domain.Common;

namespace Khidmah_Inventory.Domain.Entities;

public class Brand : Entity
{
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public string? LogoUrl { get; private set; }
    public string? Website { get; private set; }

    // Navigation properties
    public virtual ICollection<Product> Products { get; private set; } = new List<Product>();

    private Brand() { }

    public Brand(
        Guid companyId,
        string name,
        Guid? createdBy = null) : base(companyId, createdBy)
    {
        Name = name;
    }

    public void Update(
        string name,
        string? description,
        string? website,
        Guid? updatedBy = null)
    {
        Name = name;
        Description = description;
        Website = website;
        UpdateAuditInfo(updatedBy);
    }

    public void UpdateLogo(string? logoUrl, Guid? updatedBy = null)
    {
        LogoUrl = logoUrl;
        UpdateAuditInfo(updatedBy);
    }
}

