using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Khidmah_Inventory.API.Attributes;
using Khidmah_Inventory.API.Constants;
using Khidmah_Inventory.Application.Features.AI.Queries.GetDemandForecast;

namespace Khidmah_Inventory.API.Controllers;

[Route(ApiRoutes.AI.Base)]
[Authorize]
public class AIController : BaseController
{
    public AIController(IMediator mediator) : base(mediator)
    {
    }

    [HttpGet(ApiRoutes.AI.DemandForecast)]
    [ValidateApiCode(ApiValidationCodes.AIModuleCode.DemandForecast)]
    [AuthorizeResource(AuthorizePermissions.AIPermissions.Controller, AuthorizePermissions.AIPermissions.Actions.DemandForecast)]
    public async Task<IActionResult> GetDemandForecast(Guid productId, [FromQuery] int forecastDays = 30)
    {
        return await ExecuteRequest(new GetDemandForecastQuery
        {
            ProductId = productId,
            ForecastDays = forecastDays
        });
    }
}

