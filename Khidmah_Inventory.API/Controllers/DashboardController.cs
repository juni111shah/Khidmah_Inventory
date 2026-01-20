using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Khidmah_Inventory.API.Attributes;
using Khidmah_Inventory.Application.Features.Dashboard.Queries.GetDashboardData;

namespace Khidmah_Inventory.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DashboardController : BaseApiController
{
    [HttpGet]
    [AuthorizePermission("Dashboard:Read")]
    public async Task<IActionResult> GetDashboardData([FromQuery] GetDashboardDataQuery query)
    {
        var result = await Mediator.Send(query);
        return HandleResult(result, "Dashboard data retrieved successfully");
    }
}

