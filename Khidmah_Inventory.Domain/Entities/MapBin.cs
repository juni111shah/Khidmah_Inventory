using Khidmah_Inventory.Domain.Common;

namespace Khidmah_Inventory.Domain.Entities;

/// <summary>
/// Bin with (x,y) coordinates for routing algorithms. Smallest unit in the digital map.
/// </summary>
public class MapBin : Entity
{
    public Guid MapRackId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string? Code { get; private set; }
    /// <summary>X coordinate for routing (e.g. meters or grid units).</summary>
    public decimal X { get; private set; }
    /// <summary>Y coordinate for routing.</summary>
    public decimal Y { get; private set; }
    public int DisplayOrder { get; private set; }
    /// <summary>Optional link to inventory Bin for stock levels.</summary>
    public Guid? BinId { get; private set; }

    public virtual MapRack Rack { get; private set; } = null!;

    private MapBin() { }

    public MapBin(Guid companyId, Guid mapRackId, string name, decimal x, decimal y, string? code = null, Guid? binId = null, int displayOrder = 0, Guid? createdBy = null)
        : base(companyId, createdBy)
    {
        MapRackId = mapRackId;
        Name = name;
        Code = code;
        X = x;
        Y = y;
        BinId = binId;
        DisplayOrder = displayOrder;
    }

    public void Update(string name, decimal x, decimal y, string? code, Guid? binId, int displayOrder, Guid? updatedBy = null)
    {
        Name = name;
        X = x;
        Y = y;
        Code = code;
        BinId = binId;
        DisplayOrder = displayOrder;
        UpdateAuditInfo(updatedBy);
    }
}
