using MediatR;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Roles.Models;

namespace Khidmah_Inventory.Application.Features.Roles.Queries.GetRolesList;

public class GetRolesListQuery : IRequest<Result<List<RoleDto>>>
{
}

