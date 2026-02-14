using MediatR;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.PurchaseOrders.Models;

namespace Khidmah_Inventory.Application.Features.PurchaseOrders.Commands.UpdatePurchaseOrder;

public class UpdatePurchaseOrderCommand : IRequest<Result<PurchaseOrderDto>>
{
    public Guid Id { get; set; }
    public Guid SupplierId { get; set; }
    public DateTime OrderDate { get; set; }
    public DateTime? ExpectedDeliveryDate { get; set; }
    public string? Notes { get; set; }
    public string? TermsAndConditions { get; set; }
    public string? Status { get; set; }
    public List<UpdatePurchaseOrderItemDto> Items { get; set; } = new();
}

public class UpdatePurchaseOrderItemDto
{
    public Guid ProductId { get; set; }
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal DiscountPercent { get; set; } = 0;
    public decimal TaxPercent { get; set; } = 0;
    public string? Notes { get; set; }
}
