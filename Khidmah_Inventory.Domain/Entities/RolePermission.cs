using Khidmah_Inventory.Domain.Common;

namespace Khidmah_Inventory.Domain.Entities;

public class RolePermission : BaseEntity
{
    public Guid RoleId { get; private set; }
    public Guid PermissionId { get; private set; }

    // Navigation properties
    public virtual Role Role { get; private set; } = null!;
    public virtual Permission Permission { get; private set; } = null!;

    private RolePermission() { }

    public RolePermission(
        Guid companyId,
        Guid roleId,
        Guid permissionId,
        Guid? createdBy = null) : base(companyId, createdBy)
    {
        RoleId = roleId;
        PermissionId = permissionId;
    }
}

