using Khidmah_Inventory.Domain.Common;

namespace Khidmah_Inventory.Domain.Entities;

public class UserCompany : BaseEntity
{
    public Guid UserId { get; private set; }
    public new Guid CompanyId { get; private set; }
    public bool IsDefault { get; private set; } = false;
    public bool IsActive { get; private set; } = true;

    // Navigation properties
    public virtual User User { get; private set; } = null!;
    public virtual Company Company { get; private set; } = null!;

    private UserCompany() { }

    public UserCompany(
        Guid companyId,
        Guid userId,
        bool isDefault = false,
        Guid? createdBy = null) : base(companyId, createdBy)
    {
        UserId = userId;
        CompanyId = companyId;
        IsDefault = isDefault;
    }

    public void SetAsDefault()
    {
        IsDefault = true;
    }

    public void RemoveDefault()
    {
        IsDefault = false;
    }

    public void Activate()
    {
        IsActive = true;
    }

    public void Deactivate()
    {
        IsActive = false;
    }
}

