using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Khidmah_Inventory.API.Attributes;
using Khidmah_Inventory.API.Constants;
using Khidmah_Inventory.Application.Features.Dashboard.Queries.GetDashboardData;
using Khidmah_Inventory.Application.Features.Dashboard.Models;

namespace Khidmah_Inventory.API.Controllers;

[Route(ApiRoutes.Dashboard.Base)]
[Authorize]
public class DashboardController : BaseController
{
    public DashboardController(IMediator mediator) : base(mediator)
    {
    }

    [HttpGet(ApiRoutes.Dashboard.Index)]
    [ValidateApiCode(ApiValidationCodes.DashboardModuleCode.View)]
    [AuthorizeResource(AuthorizePermissions.DashboardPermissions.Controller, AuthorizePermissions.DashboardPermissions.Actions.Read)]
    public async Task<IActionResult> GetDashboardData([FromQuery] GetDashboardDataQuery query)
    {
        return await ExecuteRequest<GetDashboardDataQuery, DashboardDto>(query);
    }
}

