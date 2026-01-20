using MediatR;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.PurchaseOrders.Models;

namespace Khidmah_Inventory.Application.Features.Reordering.Commands.GeneratePurchaseOrderFromSuggestions;

public class GeneratePurchaseOrderFromSuggestionsCommand : IRequest<Result<PurchaseOrderDto>>
{
    public List<ReorderItemDto> Items { get; set; } = new();
    public Guid SupplierId { get; set; }
    public DateTime? ExpectedDeliveryDate { get; set; }
    public string? Notes { get; set; }
}

public class ReorderItemDto
{
    public Guid ProductId { get; set; }
    public Guid WarehouseId { get; set; }
    public decimal Quantity { get; set; }
    public decimal? UnitPrice { get; set; }
}

