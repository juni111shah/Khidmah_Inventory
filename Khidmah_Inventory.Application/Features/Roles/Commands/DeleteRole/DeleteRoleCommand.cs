using MediatR;
using Khidmah_Inventory.Application.Common.Models;

namespace Khidmah_Inventory.Application.Features.Roles.Commands.DeleteRole;

public class DeleteRoleCommand : IRequest<Result>
{
    public Guid Id { get; set; }
}

