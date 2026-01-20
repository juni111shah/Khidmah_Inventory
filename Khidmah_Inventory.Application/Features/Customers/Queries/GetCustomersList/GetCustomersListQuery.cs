using MediatR;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Customers.Models;

namespace Khidmah_Inventory.Application.Features.Customers.Queries.GetCustomersList;

public class GetCustomersListQuery : IRequest<Result<PagedResult<CustomerDto>>>
{
    public FilterRequest? FilterRequest { get; set; }
    public bool? IsActive { get; set; }
}

