using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Khidmah_Inventory.Application.Features.Users.Queries.GetUser;
using Khidmah_Inventory.Application.Features.Users.Queries.GetUsersList;
using Khidmah_Inventory.Application.Features.Users.Queries.GetCurrentUser;
using Khidmah_Inventory.Application.Features.Users.Commands.UpdateUserProfile;
using Khidmah_Inventory.Application.Features.Users.Commands.ChangePassword;
using Khidmah_Inventory.Application.Features.Users.Commands.ActivateUser;
using Khidmah_Inventory.Application.Features.Users.Commands.DeactivateUser;
using Khidmah_Inventory.API.Attributes;

namespace Khidmah_Inventory.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : BaseApiController
{
    [HttpGet("current")]
    public async Task<IActionResult> GetCurrent()
    {
        var query = new GetCurrentUserQuery();
        var result = await Mediator.Send(query);
        return HandleResult(result, "Current user retrieved successfully");
    }

    [HttpGet("{id}")]
    [AuthorizePermission("Users:Read")]
    public async Task<IActionResult> Get(Guid id)
    {
        var query = new GetUserQuery { Id = id };
        var result = await Mediator.Send(query);
        return HandleResult(result, "User retrieved successfully");
    }

    [HttpPost("list")]
    [AuthorizePermission("Users:List")]
    public async Task<IActionResult> GetList([FromBody] GetUsersListQuery query)
    {
        var result = await Mediator.Send(query);
        return HandleResult(result, "Users retrieved successfully");
    }

    [HttpPut("{id}/profile")]
    [AuthorizePermission("Users:Update")]
    public async Task<IActionResult> UpdateProfile(Guid id, [FromBody] UpdateUserProfileCommand command)
    {
        command.Id = id;
        var result = await Mediator.Send(command);
        return HandleResult(result, "Profile updated successfully");
    }

    [HttpPost("{id}/change-password")]
    [AuthorizePermission("Users:Update")]
    public async Task<IActionResult> ChangePassword(Guid id, [FromBody] ChangePasswordCommand command)
    {
        command.UserId = id;
        var result = await Mediator.Send(command);
        return HandleResult(result, "Password changed successfully");
    }

    [HttpPost("{id}/activate")]
    [AuthorizePermission("Users:Update")]
    public async Task<IActionResult> Activate(Guid id)
    {
        var command = new ActivateUserCommand { Id = id };
        var result = await Mediator.Send(command);
        return HandleResult(result, "User activated successfully");
    }

    [HttpPost("{id}/deactivate")]
    [AuthorizePermission("Users:Update")]
    public async Task<IActionResult> Deactivate(Guid id)
    {
        var command = new DeactivateUserCommand { Id = id };
        var result = await Mediator.Send(command);
        return HandleResult(result, "User deactivated successfully");
    }
}

