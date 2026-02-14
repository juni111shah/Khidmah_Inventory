using Khidmah_Inventory.Domain.Common;

namespace Khidmah_Inventory.Domain.Entities;

/// <summary>
/// Aisle within a zone.
/// </summary>
public class MapAisle : Entity
{
    public Guid MapZoneId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string? Code { get; private set; }
    public int DisplayOrder { get; private set; }

    public virtual MapZone Zone { get; private set; } = null!;
    public virtual ICollection<MapRack> Racks { get; private set; } = new List<MapRack>();

    private MapAisle() { }

    public MapAisle(Guid companyId, Guid mapZoneId, string name, string? code = null, int displayOrder = 0, Guid? createdBy = null)
        : base(companyId, createdBy)
    {
        MapZoneId = mapZoneId;
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
