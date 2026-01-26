using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Khidmah_Inventory.API.Attributes;
using Khidmah_Inventory.API.Constants;
using Khidmah_Inventory.Application.Features.Permissions.Queries.GetPermissionsList;

namespace Khidmah_Inventory.API.Controllers;

[Route(ApiRoutes.Permissions.Base)]
[Authorize]
public class PermissionsController : BaseController
{
    public PermissionsController(IMediator mediator) : base(mediator)
    {
    }

    [HttpGet(ApiRoutes.Permissions.Index)]
    [ValidateApiCode(ApiValidationCodes.PermissionsModuleCode.ViewAll)]
    [AuthorizeResource(AuthorizePermissions.PermissionsPermissions.Controller, AuthorizePermissions.PermissionsPermissions.Actions.ViewAll)]
    public async Task<IActionResult> GetAll([FromQuery] string? module)
    {
        return await ExecuteRequest(new GetPermissionsListQuery { Module = module });
    }
}

