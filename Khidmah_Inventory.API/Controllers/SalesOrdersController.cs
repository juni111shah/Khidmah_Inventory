using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Khidmah_Inventory.API.Attributes;
using Khidmah_Inventory.API.Constants;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.SalesOrders.Commands.CreateSalesOrder;
using Khidmah_Inventory.Application.Features.SalesOrders.Queries.GetSalesOrdersList;
using Khidmah_Inventory.Application.Features.SalesOrders.Queries.GetSalesOrder;

namespace Khidmah_Inventory.API.Controllers;

[Route(ApiRoutes.SalesOrders.Base)]
[Authorize]
public class SalesOrdersController : BaseController
{
    public SalesOrdersController(IMediator mediator) : base(mediator)
    {
    }

    [HttpPost(ApiRoutes.SalesOrders.Index)]
    [ValidateApiCode(ApiValidationCodes.SalesOrdersModuleCode.ViewAll)]
    [AuthorizeResource(AuthorizePermissions.SalesOrdersPermissions.Controller, AuthorizePermissions.SalesOrdersPermissions.Actions.ViewAll)]
    public async Task<IActionResult> GetAll([FromBody] FilterRequest request)
    {
        var query = new GetSalesOrdersListQuery { FilterRequest = request };
        return await ExecuteRequest(query);
    }

    [HttpGet(ApiRoutes.SalesOrders.GetById)]
    [ValidateApiCode(ApiValidationCodes.SalesOrdersModuleCode.ViewById)]
    [AuthorizeResource(AuthorizePermissions.SalesOrdersPermissions.Controller, AuthorizePermissions.SalesOrdersPermissions.Actions.ViewById)]
    public async Task<IActionResult> GetById(Guid id)
    {
        return await ExecuteRequestWithCache(new GetSalesOrderQuery { Id = id });
    }

    [HttpPost(ApiRoutes.SalesOrders.Add)]
    [ValidateApiCode(ApiValidationCodes.SalesOrdersModuleCode.Add)]
    [AuthorizeResource(AuthorizePermissions.SalesOrdersPermissions.Controller, AuthorizePermissions.SalesOrdersPermissions.Actions.Add)]
    public async Task<IActionResult> Create([FromBody] CreateSalesOrderCommand command)
    {
        return await ExecuteRequest(command);
    }
}

