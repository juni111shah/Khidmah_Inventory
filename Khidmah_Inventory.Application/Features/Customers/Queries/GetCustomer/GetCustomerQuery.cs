using MediatR;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Customers.Models;

namespace Khidmah_Inventory.Application.Features.Customers.Queries.GetCustomer;

public class GetCustomerQuery : IRequest<Result<CustomerDto>>
{
    public Guid Id { get; set; }
}
