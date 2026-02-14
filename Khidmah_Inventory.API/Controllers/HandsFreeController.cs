using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Khidmah_Inventory.API.Attributes;
using Khidmah_Inventory.API.Constants;
using Khidmah_Inventory.Application.Features.HandsFree.Queries.GetHandsFreeTasks;
using Khidmah_Inventory.Application.Features.HandsFree.Commands.CompleteHandsFreeTask;
using Khidmah_Inventory.Application.Features.HandsFree.Queries.ValidateBarcode;
using Khidmah_Inventory.Application.Features.HandsFree.Queries.GetHandsFreeSessions;

namespace Khidmah_Inventory.API.Controllers;

[Route(ApiRoutes.HandsFree.Base)]
[Authorize]
public class HandsFreeController : BaseController
{
    public HandsFreeController(IMediator mediator) : base(mediator)
    {
    }

    [HttpGet(ApiRoutes.HandsFree.Tasks)]
    [ValidateApiCode(ApiValidationCodes.HandsFreeModuleCode.Tasks)]
    [AuthorizeResource(AuthorizePermissions.InventoryPermissions.Controller, AuthorizePermissions.InventoryPermissions.Actions.StockLevelList)]
    public async Task<IActionResult> GetTasks([FromQuery] Guid warehouseId, [FromQuery] int maxTasks = 50)
    {
        return await ExecuteRequest(new GetHandsFreeTasksQuery { WarehouseId = warehouseId, MaxTasks = maxTasks });
    }

    [HttpPost(ApiRoutes.HandsFree.Complete)]
    [ValidateApiCode(ApiValidationCodes.HandsFreeModuleCode.Complete)]
    [AuthorizeResource(AuthorizePermissions.InventoryPermissions.Controller, AuthorizePermissions.InventoryPermissions.Actions.StockTransactionCreate)]
    public async Task<IActionResult> CompleteTask([FromBody] CompleteHandsFreeTaskCommand command)
    {
        return await ExecuteRequest(command);
    }

    [HttpGet(ApiRoutes.HandsFree.ValidateBarcode)]
    [ValidateApiCode(ApiValidationCodes.HandsFreeModuleCode.ValidateBarcode)]
    [AuthorizeResource(AuthorizePermissions.InventoryPermissions.Controller, AuthorizePermissions.InventoryPermissions.Actions.StockLevelList)]
    public async Task<IActionResult> ValidateBarcode([FromQuery] string code)
    {
        return await ExecuteRequest(new ValidateBarcodeQuery { Barcode = code ?? "" });
    }

    [HttpGet(ApiRoutes.HandsFree.Sessions)]
    [ValidateApiCode(ApiValidationCodes.HandsFreeModuleCode.Sessions)]
    [AuthorizeResource(AuthorizePermissions.InventoryPermissions.Controller, AuthorizePermissions.InventoryPermissions.Actions.StockLevelList)]
    public async Task<IActionResult> GetSessions([FromQuery] int activeWithinMinutes = 60)
    {
        return await ExecuteRequest(new GetHandsFreeSessionsQuery { ActiveWithinMinutes = activeWithinMinutes });
    }
}
