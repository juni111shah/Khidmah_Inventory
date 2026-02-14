namespace Khidmah_Inventory.Application.Features.WarehouseMap.Models;

public class MapRackDto
{
    public Guid Id { get; set; }
    public Guid MapAisleId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Code { get; set; }
    public int DisplayOrder { get; set; }
    public int BinCount { get; set; }
}
