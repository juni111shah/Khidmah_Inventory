using Khidmah_Inventory.Domain.Common;

namespace Khidmah_Inventory.Domain.Entities;

/// <summary>
/// Zone within a warehouse map (e.g. receiving, storage, shipping).
/// </summary>
public class MapZone : Entity
{
    public Guid WarehouseMapId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string? Code { get; private set; }
    public int DisplayOrder { get; private set; }

    public virtual WarehouseMap WarehouseMap { get; private set; } = null!;
    public virtual ICollection<MapAisle> Aisles { get; private set; } = new List<MapAisle>();

    private MapZone() { }

    public MapZone(Guid companyId, Guid warehouseMapId, string name, string? code = null, int displayOrder = 0, Guid? createdBy = null)
        : base(companyId, createdBy)
    {
        WarehouseMapId = warehouseMapId;
        Name = name;
        Code = code;
        DisplayOrder = displayOrder;
    }

    public void Update(string name, string? code, int displayOrder, Guid? updatedBy = null)
    {
        Name = name;
        Code = code;
        DisplayOrder = displayOrder;
        UpdateAuditInfo(updatedBy);
    }
}
