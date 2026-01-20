using Khidmah_Inventory.Domain.Common;

namespace Khidmah_Inventory.Domain.Entities;

public class UserRole : BaseEntity
{
    public Guid UserId { get; private set; }
    public Guid RoleId { get; private set; }

    // Navigation properties
    public virtual User User { get; private set; } = null!;
    public virtual Role Role { get; private set; } = null!;

    private UserRole() { }

    public UserRole(
        Guid companyId,
        Guid userId,
        Guid roleId,
        Guid? createdBy = null) : base(companyId, createdBy)
    {
        UserId = userId;
        RoleId = roleId;
    }
}

