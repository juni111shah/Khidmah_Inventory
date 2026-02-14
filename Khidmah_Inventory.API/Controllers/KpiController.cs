using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Khidmah_Inventory.API.Attributes;
using Khidmah_Inventory.API.Constants;
using Khidmah_Inventory.Application.Features.Kpi.Queries.GetExecutiveKpis;
using Khidmah_Inventory.Application.Features.Kpi.Queries.GetSalesKpis;
using Khidmah_Inventory.Application.Features.Kpi.Queries.GetInventoryKpis;
using Khidmah_Inventory.Application.Features.Kpi.Queries.GetCustomerKpis;

namespace Khidmah_Inventory.API.Controllers;

[Route(ApiRoutes.Kpi.Base)]
[Authorize]
public class KpiController : BaseController
{
    public KpiController(IMediator mediator) : base(mediator)
    {
    }

    [HttpGet(ApiRoutes.Kpi.Executive)]
    [ValidateApiCode(ApiValidationCodes.KpiModuleCode.Executive)]
    [AuthorizeResource(AuthorizePermissions.KpiPermissions.Controller, AuthorizePermissions.KpiPermissions.Actions.Read)]
    public async Task<IActionResult> GetExecutiveKpis([FromQuery] GetExecutiveKpisQuery query)
    {
        return await ExecuteRequest(query);
    }

    [HttpGet(ApiRoutes.Kpi.Sales)]
    [ValidateApiCode(ApiValidationCodes.KpiModuleCode.Sales)]
    [AuthorizeResource(AuthorizePermissions.KpiPermissions.Controller, AuthorizePermissions.KpiPermissions.Actions.Read)]
    public async Task<IActionResult> GetSalesKpis([FromQuery] GetSalesKpisQuery query)
    {
        return await ExecuteRequest(query);
    }

    [HttpGet(ApiRoutes.Kpi.Inventory)]
    [ValidateApiCode(ApiValidationCodes.KpiModuleCode.Inventory)]
    [AuthorizeResource(AuthorizePermissions.KpiPermissions.Controller, AuthorizePermissions.KpiPermissions.Actions.Read)]
    public async Task<IActionResult> GetInventoryKpis([FromQuery] GetInventoryKpisQuery query)
    {
        return await ExecuteRequest(query);
    }

    [HttpGet(ApiRoutes.Kpi.Customers)]
    [ValidateApiCode(ApiValidationCodes.KpiModuleCode.Customers)]
    [AuthorizeResource(AuthorizePermissions.KpiPermissions.Controller, AuthorizePermissions.KpiPermissions.Actions.Read)]
    public async Task<IActionResult> GetCustomerKpis([FromQuery] GetCustomerKpisQuery query)
    {
        return await ExecuteRequest(query);
    }
}
