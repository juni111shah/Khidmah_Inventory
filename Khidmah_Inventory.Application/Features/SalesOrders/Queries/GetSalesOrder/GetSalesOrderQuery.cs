using MediatR;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.SalesOrders.Models;

namespace Khidmah_Inventory.Application.Features.SalesOrders.Queries.GetSalesOrder;

public class GetSalesOrderQuery : IRequest<Result<SalesOrderDto>>
{
    public Guid Id { get; set; }
}