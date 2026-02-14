namespace Khidmah_Inventory.Application.Features.WarehouseMap.Models;

/// <summary>
/// Full map tree for routing: map → zones → aisles → racks → bins (with x,y).
/// </summary>
public class WarehouseMapTreeDto
{
    public WarehouseMapDto Map { get; set; } = null!;
    public List<MapZoneTreeDto> Zones { get; set; } = new();
}

public class MapZoneTreeDto
{
    public MapZoneDto Zone { get; set; } = null!;
    public List<MapAisleTreeDto> Aisles { get; set; } = new();
}

public class MapAisleTreeDto
{
    public MapAisleDto Aisle { get; set; } = null!;
    public List<MapRackTreeDto> Racks { get; set; } = new();
}

public class MapRackTreeDto
{
    public MapRackDto Rack { get; set; } = null!;
    public List<MapBinDto> Bins { get; set; } = new();
}
