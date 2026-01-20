using MediatR;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.PurchaseOrders.Models;

namespace Khidmah_Inventory.Application.Features.PurchaseOrders.Queries.GetPurchaseOrder;

public class GetPurchaseOrderQuery : IRequest<Result<PurchaseOrderDto>>
{
    public Guid Id { get; set; }
}

