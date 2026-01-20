using Khidmah_Inventory.Domain.Common;

namespace Khidmah_Inventory.Domain.Entities;

public class Bin : Entity
{
    public Guid WarehouseZoneId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string? Code { get; private set; }
    public string? Description { get; private set; }
    public int DisplayOrder { get; private set; } = 0;
    public bool IsActive { get; private set; } = true;

    // Navigation properties
    public virtual WarehouseZone WarehouseZone { get; private set; } = null!;

    private Bin() { }

    public Bin(
        Guid companyId,
        Guid warehouseZoneId,
        string name,
        string? code = null,
        Guid? createdBy = null) : base(companyId, createdBy)
    {
        WarehouseZoneId = warehouseZoneId;
        Name = name;
        Code = code;
    }

    public void Update(
        string name,
        string? code,
        string? description,
        int displayOrder,
        Guid? updatedBy = null)
    {
        Name = name;
        Code = code;
        Description = description;
        DisplayOrder = displayOrder;
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
}

