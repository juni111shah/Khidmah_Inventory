using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Khidmah_Inventory.API.Attributes;
using Khidmah_Inventory.API.Constants;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.PurchaseOrders.Commands.CreatePurchaseOrder;
using Khidmah_Inventory.Application.Features.PurchaseOrders.Commands.UpdatePurchaseOrder;
using Khidmah_Inventory.Application.Features.PurchaseOrders.Queries.GetPurchaseOrder;
using Khidmah_Inventory.Application.Features.PurchaseOrders.Queries.GetPurchaseOrdersList;

namespace Khidmah_Inventory.API.Controllers;

[Route(ApiRoutes.PurchaseOrders.Base)]
[Authorize]
public class PurchaseOrdersController : BaseController
{
    public PurchaseOrdersController(IMediator mediator) : base(mediator)
    {
    }

    [HttpPost(ApiRoutes.PurchaseOrders.Index)]
    [ValidateApiCode(ApiValidationCodes.PurchaseOrdersModuleCode.ViewAll)]
    [AuthorizeResource(AuthorizePermissions.PurchaseOrdersPermissions.Controller, AuthorizePermissions.PurchaseOrdersPermissions.Actions.ViewAll)]
    public async Task<IActionResult> GetAll([FromBody] FilterRequest request)
    {
        var query = new GetPurchaseOrdersListQuery { FilterRequest = request };
        return await ExecuteRequest(query);
    }

    [HttpGet(ApiRoutes.PurchaseOrders.GetById)]
    [ValidateApiCode(ApiValidationCodes.PurchaseOrdersModuleCode.ViewById)]
    [AuthorizeResource(AuthorizePermissions.PurchaseOrdersPermissions.Controller, AuthorizePermissions.PurchaseOrdersPermissions.Actions.ViewById)]
    public async Task<IActionResult> GetById(Guid id)
    {
        return await ExecuteRequestWithCache(new GetPurchaseOrderQuery { Id = id });
    }

    [HttpPost(ApiRoutes.PurchaseOrders.Add)]
    [ValidateApiCode(ApiValidationCodes.PurchaseOrdersModuleCode.Add)]
    [AuthorizeResource(AuthorizePermissions.PurchaseOrdersPermissions.Controller, AuthorizePermissions.PurchaseOrdersPermissions.Actions.Add)]
    public async Task<IActionResult> Create([FromBody] CreatePurchaseOrderCommand command)
    {
        return await ExecuteRequest(command);
    }

    [HttpPut(ApiRoutes.PurchaseOrders.Update)]
    [ValidateApiCode(ApiValidationCodes.PurchaseOrdersModuleCode.Update)]
    [AuthorizeResource(AuthorizePermissions.PurchaseOrdersPermissions.Controller, AuthorizePermissions.PurchaseOrdersPermissions.Actions.Update)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePurchaseOrderCommand command)
    {
        command.Id = id;
        return await ExecuteRequest(command);
    }
}

