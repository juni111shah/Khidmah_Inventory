using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Khidmah_Inventory.API.Attributes;
using Khidmah_Inventory.API.Constants;
using Khidmah_Inventory.Application.Features.Analytics.Queries.GetSalesAnalytics;
using Khidmah_Inventory.Application.Features.Analytics.Queries.GetInventoryAnalytics;
using Khidmah_Inventory.Application.Features.Analytics.Queries.GetProfitAnalytics;

namespace Khidmah_Inventory.API.Controllers;

[Route(ApiRoutes.Analytics.Base)]
[Authorize]
public class AnalyticsController : BaseController
{
    public AnalyticsController(IMediator mediator) : base(mediator)
    {
    }

    [HttpPost(ApiRoutes.Analytics.Sales)]
    [ValidateApiCode(ApiValidationCodes.AnalyticsModuleCode.Sales)]
    [AuthorizeResource(AuthorizePermissions.AnalyticsPermissions.Controller, AuthorizePermissions.AnalyticsPermissions.Actions.Sales)]
    public async Task<IActionResult> GetSalesAnalytics([FromBody] GetSalesAnalyticsQuery query)
    {
        return await ExecuteRequest(query);
    }

    [HttpGet(ApiRoutes.Analytics.Inventory)]
    [ValidateApiCode(ApiValidationCodes.AnalyticsModuleCode.Inventory)]
    [AuthorizeResource(AuthorizePermissions.AnalyticsPermissions.Controller, AuthorizePermissions.AnalyticsPermissions.Actions.Inventory)]
    public async Task<IActionResult> GetInventoryAnalytics([FromQuery] GetInventoryAnalyticsQuery query)
    {
        return await ExecuteRequest(query);
    }

    [HttpPost(ApiRoutes.Analytics.Profit)]
    [ValidateApiCode(ApiValidationCodes.AnalyticsModuleCode.Profit)]
    [AuthorizeResource(AuthorizePermissions.AnalyticsPermissions.Controller, AuthorizePermissions.AnalyticsPermissions.Actions.Profit)]
    public async Task<IActionResult> GetProfitAnalytics([FromBody] GetProfitAnalyticsQuery query)
    {
        return await ExecuteRequest(query);
    }
}

