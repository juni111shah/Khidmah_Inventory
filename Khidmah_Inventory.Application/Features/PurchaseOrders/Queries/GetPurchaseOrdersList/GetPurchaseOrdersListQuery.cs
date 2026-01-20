using MediatR;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.PurchaseOrders.Models;

namespace Khidmah_Inventory.Application.Features.PurchaseOrders.Queries.GetPurchaseOrdersList;

public class GetPurchaseOrdersListQuery : IRequest<Result<PagedResult<PurchaseOrderDto>>>
{
    public FilterRequest? FilterRequest { get; set; }
    public Guid? SupplierId { get; set; }
    public string? Status { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}

