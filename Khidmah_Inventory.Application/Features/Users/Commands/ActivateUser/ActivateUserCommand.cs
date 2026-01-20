using MediatR;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Users.Models;

namespace Khidmah_Inventory.Application.Features.Users.Commands.ActivateUser;

public class ActivateUserCommand : IRequest<Result<UserDto>>
{
    public Guid Id { get; set; }
}

