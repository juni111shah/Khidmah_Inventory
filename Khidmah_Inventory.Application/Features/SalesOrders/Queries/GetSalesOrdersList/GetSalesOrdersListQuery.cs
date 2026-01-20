using MediatR;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.SalesOrders.Models;

namespace Khidmah_Inventory.Application.Features.SalesOrders.Queries.GetSalesOrdersList;

public class GetSalesOrdersListQuery : IRequest<Result<PagedResult<SalesOrderDto>>>
{
    public FilterRequest? FilterRequest { get; set; }
    public Guid? CustomerId { get; set; }
    public string? Status { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}

