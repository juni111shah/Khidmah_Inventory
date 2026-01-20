using MediatR;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.SalesOrders.Models;

namespace Khidmah_Inventory.Application.Features.SalesOrders.Commands.CreateSalesOrder;

public class CreateSalesOrderCommand : IRequest<Result<SalesOrderDto>>
{
    public Guid CustomerId { get; set; }
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;
    public DateTime? ExpectedDeliveryDate { get; set; }
    public string? Notes { get; set; }
    public string? TermsAndConditions { get; set; }
    public List<CreateSalesOrderItemDto> Items { get; set; } = new();
}

public class CreateSalesOrderItemDto
{
    public Guid ProductId { get; set; }
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal DiscountPercent { get; set; } = 0;
    public decimal TaxPercent { get; set; } = 0;
    public string? Notes { get; set; }
}

