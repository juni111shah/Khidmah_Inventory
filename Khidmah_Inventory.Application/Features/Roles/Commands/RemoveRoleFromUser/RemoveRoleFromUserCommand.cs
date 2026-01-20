using MediatR;
using Khidmah_Inventory.Application.Common.Models;

namespace Khidmah_Inventory.Application.Features.Roles.Commands.RemoveRoleFromUser;

public class RemoveRoleFromUserCommand : IRequest<Result>
{
    public Guid UserId { get; set; }
    public Guid RoleId { get; set; }
}

