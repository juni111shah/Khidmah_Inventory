using MediatR;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Roles.Models;

namespace Khidmah_Inventory.Application.Features.Roles.Queries.GetRole;

public class GetRoleQuery : IRequest<Result<RoleDto>>
{
    public Guid Id { get; set; }
}

