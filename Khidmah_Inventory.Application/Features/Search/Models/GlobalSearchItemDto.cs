namespace Khidmah_Inventory.Application.Features.Search.Models;

public class GlobalSearchItemDto
{
    public Guid Id { get; set; }
    public string NameOrNumber { get; set; } = string.Empty;
    public string Route { get; set; } = string.Empty;
    public string? ExtraInfo { get; set; }
}
