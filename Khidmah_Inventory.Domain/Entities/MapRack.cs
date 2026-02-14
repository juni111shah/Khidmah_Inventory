using Khidmah_Inventory.Domain.Common;

namespace Khidmah_Inventory.Domain.Entities;

/// <summary>
/// Rack within an aisle.
/// </summary>
public class MapRack : Entity
{
    public Guid MapAisleId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string? Code { get; private set; }
    public int DisplayOrder { get; private set; }

    public virtual MapAisle Aisle { get; private set; } = null!;
    public virtual ICollection<MapBin> Bins { get; private set; } = new List<MapBin>();

    private MapRack() { }

    public MapRack(Guid companyId, Guid mapAisleId, string name, string? code = null, int displayOrder = 0, Guid? createdBy = null)
        : base(companyId, createdBy)
    {
        MapAisleId = mapAisleId;
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
