using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Khidmah_Inventory.Application.Features.Roles.Queries.GetRole;
using Khidmah_Inventory.Application.Features.Roles.Queries.GetRolesList;
using Khidmah_Inventory.Application.Features.Roles.Commands.CreateRole;
using Khidmah_Inventory.Application.Features.Roles.Commands.UpdateRole;
using Khidmah_Inventory.Application.Features.Roles.Commands.DeleteRole;
using Khidmah_Inventory.Application.Features.Roles.Commands.AssignRoleToUser;
using Khidmah_Inventory.Application.Features.Roles.Commands.RemoveRoleFromUser;
using Khidmah_Inventory.API.Attributes;

namespace Khidmah_Inventory.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RolesController : BaseApiController
{
    [HttpGet("{id}")]
    [AuthorizePermission("Roles:Read")]
    public async Task<IActionResult> Get(Guid id)
    {
        var query = new GetRoleQuery { Id = id };
        var result = await Mediator.Send(query);
        return HandleResult(result, "Role retrieved successfully");
    }

    [HttpGet]
    [AuthorizePermission("Roles:List")]
    public async Task<IActionResult> GetList()
    {
        var query = new GetRolesListQuery();
        var result = await Mediator.Send(query);
        return HandleResult(result, "Roles retrieved successfully");
    }

    [HttpPost]
    [AuthorizePermission("Roles:Create")]
    public async Task<IActionResult> Create([FromBody] CreateRoleCommand command)
    {
        var result = await Mediator.Send(command);
        return HandleResult(result, "Role created successfully");
    }

    [HttpPut("{id}")]
    [AuthorizePermission("Roles:Update")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateRoleCommand command)
    {
        command.Id = id;
        var result = await Mediator.Send(command);
        return HandleResult(result, "Role updated successfully");
    }

    [HttpDelete("{id}")]
    [AuthorizePermission("Roles:Delete")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var command = new DeleteRoleCommand { Id = id };
        var result = await Mediator.Send(command);
        return HandleResult(result, "Role deleted successfully");
    }

    [HttpPost("{roleId}/assign-user/{userId}")]
    [AuthorizePermission("Roles:Assign")]
    public async Task<IActionResult> AssignRoleToUser(Guid roleId, Guid userId)
    {
        var command = new AssignRoleToUserCommand { RoleId = roleId, UserId = userId };
        var result = await Mediator.Send(command);
        return HandleResult(result, "Role assigned to user successfully");
    }

    [HttpDelete("{roleId}/remove-user/{userId}")]
    [AuthorizePermission("Roles:Assign")]
    public async Task<IActionResult> RemoveRoleFromUser(Guid roleId, Guid userId)
    {
        var command = new RemoveRoleFromUserCommand { RoleId = roleId, UserId = userId };
        var result = await Mediator.Send(command);
        return HandleResult(result, "Role removed from user successfully");
    }
}

