using MediatR;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Roles.Models;

namespace Khidmah_Inventory.Application.Features.Permissions.Queries.GetPermissionsList;

public class GetPermissionsListQuery : IRequest<Result<List<PermissionDto>>>
{
    public string? Module { get; set; }
}

