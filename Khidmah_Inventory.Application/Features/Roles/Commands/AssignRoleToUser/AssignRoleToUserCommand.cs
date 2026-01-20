using MediatR;
using Khidmah_Inventory.Application.Common.Models;

namespace Khidmah_Inventory.Application.Features.Roles.Commands.AssignRoleToUser;

public class AssignRoleToUserCommand : IRequest<Result>
{
    public Guid UserId { get; set; }
    public Guid RoleId { get; set; }
}

