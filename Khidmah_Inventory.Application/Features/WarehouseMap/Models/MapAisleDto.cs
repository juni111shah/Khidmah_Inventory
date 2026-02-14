namespace Khidmah_Inventory.Application.Features.WarehouseMap.Models;

public class MapAisleDto
{
    public Guid Id { get; set; }
    public Guid MapZoneId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Code { get; set; }
    public int DisplayOrder { get; set; }
    public int RackCount { get; set; }
}
