namespace Khidmah_Inventory.Application.Features.WarehouseMap.Models;

public class MapZoneDto
{
    public Guid Id { get; set; }
    public Guid WarehouseMapId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Code { get; set; }
    public int DisplayOrder { get; set; }
    public int AisleCount { get; set; }
}
