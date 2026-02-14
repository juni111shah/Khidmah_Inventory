namespace Khidmah_Inventory.Application.Features.Search.Models;

public class GlobalSearchResultDto
{
    public List<GlobalSearchItemDto> Products { get; set; } = new();
    public List<GlobalSearchItemDto> Customers { get; set; } = new();
    public List<GlobalSearchItemDto> Suppliers { get; set; } = new();
    public List<GlobalSearchItemDto> PurchaseOrders { get; set; } = new();
    public List<GlobalSearchItemDto> SalesOrders { get; set; } = new();
}
