using MediatR;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Roles.Models;

namespace Khidmah_Inventory.Application.Features.Roles.Commands.CreateRole;

public class CreateRoleCommand : IRequest<Result<RoleDto>>
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public List<Guid> PermissionIds { get; set; } = new();
}

