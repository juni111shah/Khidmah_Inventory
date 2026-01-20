using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Khidmah_Inventory.Application.Features.Permissions.Queries.GetPermissionsList;
using Khidmah_Inventory.API.Attributes;

namespace Khidmah_Inventory.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PermissionsController : BaseApiController
{
    [HttpGet]
    [AuthorizePermission("Permissions:Read")]
    public async Task<IActionResult> GetList([FromQuery] string? module)
    {
        var query = new GetPermissionsListQuery { Module = module };
        var result = await Mediator.Send(query);
        return HandleResult(result, "Permissions retrieved successfully");
    }
}

