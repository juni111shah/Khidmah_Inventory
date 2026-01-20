using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Khidmah_Inventory.API.Attributes;
using Khidmah_Inventory.Application.Features.Analytics.Queries.GetSalesAnalytics;
using Khidmah_Inventory.Application.Features.Analytics.Queries.GetInventoryAnalytics;
using Khidmah_Inventory.Application.Features.Analytics.Queries.GetProfitAnalytics;

namespace Khidmah_Inventory.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AnalyticsController : BaseApiController
{
    [HttpPost("sales")]
    [AuthorizePermission("Analytics:Sales:Read")]
    public async Task<IActionResult> GetSalesAnalytics([FromBody] GetSalesAnalyticsQuery query)
    {
        var result = await Mediator.Send(query);
        return HandleResult(result, "Sales analytics retrieved successfully");
    }

    [HttpGet("inventory")]
    [AuthorizePermission("Analytics:Inventory:Read")]
    public async Task<IActionResult> GetInventoryAnalytics([FromQuery] GetInventoryAnalyticsQuery query)
    {
        var result = await Mediator.Send(query);
        return HandleResult(result, "Inventory analytics retrieved successfully");
    }

    [HttpPost("profit")]
    [AuthorizePermission("Analytics:Profit:Read")]
    public async Task<IActionResult> GetProfitAnalytics([FromBody] GetProfitAnalyticsQuery query)
    {
        var result = await Mediator.Send(query);
        return HandleResult(result, "Profit analytics retrieved successfully");
    }
}

