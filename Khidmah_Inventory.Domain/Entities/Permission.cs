using Khidmah_Inventory.Domain.Common;

namespace Khidmah_Inventory.Domain.Entities;

public class Permission : BaseEntity
{
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public string Module { get; private set; } = string.Empty;
    public string Action { get; private set; } = string.Empty; // Create, Read, Update, Delete, etc.

    // Navigation properties
    public virtual ICollection<RolePermission> RolePermissions { get; private set; } = new List<RolePermission>();

    private Permission() { }

    public Permission(
        Guid companyId,
        string name,
        string module,
        string action,
        string? description = null,
        Guid? createdBy = null) : base(companyId, createdBy)
    {
        Name = name;
        Module = module;
        Action = action;
        Description = description;
    }
}

