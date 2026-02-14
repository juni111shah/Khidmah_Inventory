using MediatR;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.SalesOrders.Models;

namespace Khidmah_Inventory.Application.Features.SalesOrders.Commands.UpdateSalesOrder;

public class UpdateSalesOrderCommand : IRequest<Result<SalesOrderDto>>
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public DateTime OrderDate { get; set; }
    public DateTime? ExpectedDeliveryDate { get; set; }
    public string? Notes { get; set; }
    public string? TermsAndConditions { get; set; }
    public string? Status { get; set; }
    public List<UpdateSalesOrderItemDto> Items { get; set; } = new();
}

public class UpdateSalesOrderItemDto
{
    public Guid ProductId { get; set; }
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal DiscountPercent { get; set; } = 0;
    public decimal TaxPercent { get; set; } = 0;
    public string? Notes { get; set; }
}
