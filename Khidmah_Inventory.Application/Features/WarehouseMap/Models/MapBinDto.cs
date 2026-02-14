namespace Khidmah_Inventory.Application.Features.WarehouseMap.Models;

public class MapBinDto
{
    public Guid Id { get; set; }
    public Guid MapRackId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Code { get; set; }
    public decimal X { get; set; }
    public decimal Y { get; set; }
    public int DisplayOrder { get; set; }
    public Guid? BinId { get; set; }
}
