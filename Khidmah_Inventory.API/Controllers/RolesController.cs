using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Khidmah_Inventory.API.Attributes;
using Khidmah_Inventory.API.Constants;
using Khidmah_Inventory.Application.Features.Roles.Queries.GetRole;
using Khidmah_Inventory.Application.Features.Roles.Queries.GetRolesList;
using Khidmah_Inventory.Application.Features.Roles.Commands.CreateRole;
using Khidmah_Inventory.Application.Features.Roles.Commands.UpdateRole;
using Khidmah_Inventory.Application.Features.Roles.Commands.DeleteRole;
using Khidmah_Inventory.Application.Features.Roles.Commands.AssignRoleToUser;
using Khidmah_Inventory.Application.Features.Roles.Commands.RemoveRoleFromUser;

namespace Khidmah_Inventory.API.Controllers;

[Route(ApiRoutes.Roles.Base)]
[Authorize]
public class RolesController : BaseController
{
    public RolesController(IMediator mediator) : base(mediator)
    {
    }

    [HttpGet(ApiRoutes.Roles.Index)]
    [ValidateApiCode(ApiValidationCodes.RolesModuleCode.ViewAll)]
    [AuthorizeResource(AuthorizePermissions.RolesPermissions.Controller, AuthorizePermissions.RolesPermissions.Actions.ViewAll)]
    public async Task<IActionResult> GetAll()
    {
        return await ExecuteRequest(new GetRolesListQuery());
    }

    [HttpGet(ApiRoutes.Roles.GetById)]
    [ValidateApiCode(ApiValidationCodes.RolesModuleCode.ViewById)]
    [AuthorizeResource(AuthorizePermissions.RolesPermissions.Controller, AuthorizePermissions.RolesPermissions.Actions.ViewById)]
    public async Task<IActionResult> GetById(Guid id)
    {
        return await ExecuteRequestWithCache(new GetRoleQuery { Id = id });
    }

    [HttpPost(ApiRoutes.Roles.Add)]
    [ValidateApiCode(ApiValidationCodes.RolesModuleCode.Add)]
    [AuthorizeResource(AuthorizePermissions.RolesPermissions.Controller, AuthorizePermissions.RolesPermissions.Actions.Add)]
    public async Task<IActionResult> Create([FromBody] CreateRoleCommand command)
    {
        return await ExecuteRequest(command);
    }

    [HttpPut(ApiRoutes.Roles.Update)]
    [ValidateApiCode(ApiValidationCodes.RolesModuleCode.Update)]
    [AuthorizeResource(AuthorizePermissions.RolesPermissions.Controller, AuthorizePermissions.RolesPermissions.Actions.Update)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateRoleCommand command)
    {
        command.Id = id;
        return await ExecuteRequest(command);
    }

    [HttpDelete(ApiRoutes.Roles.Delete)]
    [ValidateApiCode(ApiValidationCodes.RolesModuleCode.Delete)]
    [AuthorizeResource(AuthorizePermissions.RolesPermissions.Controller, AuthorizePermissions.RolesPermissions.Actions.Delete)]
    public async Task<IActionResult> Delete(Guid id)
    {
        return await ExecuteRequest(new DeleteRoleCommand { Id = id });
    }

    [HttpPost(ApiRoutes.Roles.AssignUser)]
    [ValidateApiCode(ApiValidationCodes.RolesModuleCode.Assign)]
    [AuthorizeResource(AuthorizePermissions.RolesPermissions.Controller, AuthorizePermissions.RolesPermissions.Actions.Assign)]
    public async Task<IActionResult> AssignRoleToUser(Guid roleId, Guid userId)
    {
        return await ExecuteRequest(new AssignRoleToUserCommand { RoleId = roleId, UserId = userId });
    }

    [HttpDelete(ApiRoutes.Roles.RemoveUser)]
    [ValidateApiCode(ApiValidationCodes.RolesModuleCode.Assign)]
    [AuthorizeResource(AuthorizePermissions.RolesPermissions.Controller, AuthorizePermissions.RolesPermissions.Actions.Assign)]
    public async Task<IActionResult> RemoveRoleFromUser(Guid roleId, Guid userId)
    {
        return await ExecuteRequest(new RemoveRoleFromUserCommand { RoleId = roleId, UserId = userId });
    }
}

