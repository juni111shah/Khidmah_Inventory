using Khidmah_Inventory.Domain.Common;

namespace Khidmah_Inventory.Domain.Entities;

public class Role : Entity
{
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public bool IsSystemRole { get; private set; } = false;

    // Navigation properties
    public virtual ICollection<UserRole> UserRoles { get; private set; } = new List<UserRole>();
    public virtual ICollection<RolePermission> RolePermissions { get; private set; } = new List<RolePermission>();

    private Role() { }

    public Role(
        Guid companyId,
        string name,
        string? description = null,
        bool isSystemRole = false,
        Guid? createdBy = null) : base(companyId, createdBy)
    {
        Name = name;
        Description = description;
        IsSystemRole = isSystemRole;
    }

    public void Update(string name, string? description, Guid? updatedBy = null)
    {
        if (IsSystemRole)
            throw new InvalidOperationException("Cannot update system role");

        Name = name;
        Description = description;
        UpdateAuditInfo(updatedBy);
    }
}

