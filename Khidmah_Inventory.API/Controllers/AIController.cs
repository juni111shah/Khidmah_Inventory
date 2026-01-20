using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Khidmah_Inventory.API.Attributes;
using Khidmah_Inventory.Application.Features.AI.Queries.GetDemandForecast;

namespace Khidmah_Inventory.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AIController : BaseApiController
{
    [HttpGet("demand-forecast/{productId}")]
    [AuthorizePermission("AI:DemandForecast:Read")]
    public async Task<IActionResult> GetDemandForecast(Guid productId, [FromQuery] int forecastDays = 30)
    {
        var query = new GetDemandForecastQuery
        {
            ProductId = productId,
            ForecastDays = forecastDays
        };
        var result = await Mediator.Send(query);
        return HandleResult(result, "Demand forecast retrieved successfully");
    }
}

