using MediatR;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Users.Models;

namespace Khidmah_Inventory.Application.Features.Users.Queries.GetUsersList;

public class GetUsersListQuery : IRequest<Result<PagedResult<UserDto>>>
{
    public FilterRequest? FilterRequest { get; set; }
}

