using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Khidmah_Inventory.API.Attributes;
using Khidmah_Inventory.API.Constants;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Warehouses.Commands.CreateWarehouse;
using Khidmah_Inventory.Application.Features.Warehouses.Commands.UpdateWarehouse;
using Khidmah_Inventory.Application.Features.Warehouses.Commands.DeleteWarehouse;
using Khidmah_Inventory.Application.Features.Warehouses.Commands.ActivateWarehouse;
using Khidmah_Inventory.Application.Features.Warehouses.Commands.DeactivateWarehouse;
using Khidmah_Inventory.Application.Features.Warehouses.Queries.GetWarehouse;
using Khidmah_Inventory.Application.Features.Warehouses.Queries.GetWarehousesList;
using Khidmah_Inventory.Application.Features.Warehouses.Models;
using Khidmah_Inventory.Application.Common.Models;

namespace Khidmah_Inventory.API.Controllers;

[Route(ApiRoutes.Warehouses.Base)]
[Authorize]
public class WarehousesController : BaseController
{
    public WarehousesController(IMediator mediator) : base(mediator)
    {
    }

    [HttpPost(ApiRoutes.Warehouses.Index)]
    [ValidateApiCode(ApiValidationCodes.WarehousesModuleCode.ViewAll)]
    [AuthorizeResource(AuthorizePermissions.WarehousesPermissions.Controller, AuthorizePermissions.WarehousesPermissions.Actions.ViewAll)]
    public async Task<IActionResult> GetAll([FromBody] FilterRequest request)
    {
        var query = new GetWarehousesListQuery { FilterRequest = request };
        return await ExecuteRequest<GetWarehousesListQuery, PagedResult<WarehouseDto>>(query);
    }

    [HttpGet(ApiRoutes.Warehouses.GetById)]
    [ValidateApiCode(ApiValidationCodes.WarehousesModuleCode.ViewById)]
    [AuthorizeResource(AuthorizePermissions.WarehousesPermissions.Controller, AuthorizePermissions.WarehousesPermissions.Actions.ViewById)]
    public async Task<IActionResult> GetById(Guid id)
    {
        return await ExecuteRequestWithCache(new GetWarehouseQuery { Id = id });
    }

    [HttpPost(ApiRoutes.Warehouses.Add)]
    [ValidateApiCode(ApiValidationCodes.WarehousesModuleCode.Add)]
    [AuthorizeResource(AuthorizePermissions.WarehousesPermissions.Controller, AuthorizePermissions.WarehousesPermissions.Actions.Add)]
    public async Task<IActionResult> Create([FromBody] CreateWarehouseCommand command)
    {
        return await ExecuteRequest(command);
    }

    [HttpPut(ApiRoutes.Warehouses.Update)]
    [ValidateApiCode(ApiValidationCodes.WarehousesModuleCode.Update)]
    [AuthorizeResource(AuthorizePermissions.WarehousesPermissions.Controller, AuthorizePermissions.WarehousesPermissions.Actions.Update)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateWarehouseCommand command)
    {
        command.Id = id;
        return await ExecuteRequest(command);
    }

    [HttpDelete(ApiRoutes.Warehouses.Delete)]
    [ValidateApiCode(ApiValidationCodes.WarehousesModuleCode.Delete)]
    [AuthorizeResource(AuthorizePermissions.WarehousesPermissions.Controller, AuthorizePermissions.WarehousesPermissions.Actions.Delete)]
    public async Task<IActionResult> Delete(Guid id)
    {
        return await ExecuteRequest(new DeleteWarehouseCommand { Id = id });
    }

    [HttpPatch(ApiRoutes.Warehouses.Activate)]
    [ValidateApiCode(ApiValidationCodes.WarehousesModuleCode.UpdateStatus)]
    [AuthorizeResource(AuthorizePermissions.WarehousesPermissions.Controller, AuthorizePermissions.WarehousesPermissions.Actions.Update)]
    public async Task<IActionResult> Activate(Guid id)
    {
        return await ExecuteRequest(new ActivateWarehouseCommand { Id = id });
    }

    [HttpPatch(ApiRoutes.Warehouses.Deactivate)]
    [ValidateApiCode(ApiValidationCodes.WarehousesModuleCode.UpdateStatus)]
    [AuthorizeResource(AuthorizePermissions.WarehousesPermissions.Controller, AuthorizePermissions.WarehousesPermissions.Actions.Update)]
    public async Task<IActionResult> Deactivate(Guid id)
    {
        return await ExecuteRequest(new DeactivateWarehouseCommand { Id = id });
    }
}

