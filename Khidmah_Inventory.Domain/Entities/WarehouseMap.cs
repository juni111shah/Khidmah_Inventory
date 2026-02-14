using Khidmah_Inventory.Domain.Common;

namespace Khidmah_Inventory.Domain.Entities;

/// <summary>
/// Digital warehouse map for spatial routing. One per warehouse (or multiple layouts).
/// </summary>
public class WarehouseMap : Entity
{
    public Guid WarehouseId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public bool IsActive { get; private set; } = true;

    public virtual Warehouse Warehouse { get; private set; } = null!;
    public virtual ICollection<MapZone> Zones { get; private set; } = new List<MapZone>();

    private WarehouseMap() { }

    public WarehouseMap(Guid companyId, Guid warehouseId, string name, string? description = null, Guid? createdBy = null)
        : base(companyId, createdBy)
    {
        WarehouseId = warehouseId;
        Name = name;
        Description = description;
    }

    public void Update(string name, string? description, Guid? updatedBy = null)
    {
        Name = name;
        Description = description;
        UpdateAuditInfo(updatedBy);
    }

    public void SetActive(bool active, Guid? updatedBy = null)
    {
        IsActive = active;
        UpdateAuditInfo(updatedBy);
    }
}
