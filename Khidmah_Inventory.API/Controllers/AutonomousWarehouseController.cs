using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Khidmah_Inventory.API.Attributes;
using Khidmah_Inventory.API.Constants;
using Khidmah_Inventory.Application.Features.AutonomousWarehouse.Queries.GetWorkTasks;
using Khidmah_Inventory.Application.Features.AutonomousWarehouse.Queries.GetOptimizedRoute;
using Khidmah_Inventory.Application.Features.AutonomousWarehouse.Commands.PlanWorkTasks;
using Khidmah_Inventory.Application.Features.AutonomousWarehouse.Commands.AssignWorkTasks;
using Khidmah_Inventory.Application.Features.AutonomousWarehouse.Commands.CompleteWorkTask;
using Khidmah_Inventory.Application.Features.AutonomousWarehouse.Models;

namespace Khidmah_Inventory.API.Controllers;

[Route(ApiRoutes.AutonomousWarehouse.Base)]
[Authorize]
public class AutonomousWarehouseController : BaseController
{
    public AutonomousWarehouseController(IMediator mediator) : base(mediator)
    {
    }

    [HttpGet(ApiRoutes.AutonomousWarehouse.Tasks)]
    [ValidateApiCode(ApiValidationCodes.WarehouseMapModuleCode.ViewAll)]
    [AuthorizeResource(AuthorizePermissions.WarehouseMapPermissions.Controller, AuthorizePermissions.WarehouseMapPermissions.Actions.ViewAll)]
    public async Task<IActionResult> GetTasks([FromQuery] Guid warehouseId, [FromQuery] Guid? assignedToId, [FromQuery] int? status, [FromQuery] int? type, [FromQuery] int maxCount = 100)
    {
        return await ExecuteRequest(new GetWorkTasksQuery
        {
            WarehouseId = warehouseId,
            AssignedToId = assignedToId,
            Status = status,
            Type = type,
            MaxCount = maxCount
        });
    }

    [HttpPost(ApiRoutes.AutonomousWarehouse.Plan)]
    [ValidateApiCode(ApiValidationCodes.WarehouseMapModuleCode.Add)]
    [AuthorizeResource(AuthorizePermissions.WarehouseMapPermissions.Controller, AuthorizePermissions.WarehouseMapPermissions.Actions.Add)]
    public async Task<IActionResult> PlanTasks([FromQuery] Guid warehouseId, [FromBody] OrderTaskRequest request)
    {
        return await ExecuteRequest(new PlanWorkTasksCommand { WarehouseId = warehouseId, Request = request ?? new() });
    }

    [HttpPost(ApiRoutes.AutonomousWarehouse.Assign)]
    [ValidateApiCode(ApiValidationCodes.WarehouseMapModuleCode.Update)]
    [AuthorizeResource(AuthorizePermissions.WarehouseMapPermissions.Controller, AuthorizePermissions.WarehouseMapPermissions.Actions.Update)]
    public async Task<IActionResult> AssignTasks([FromQuery] Guid warehouseId, [FromBody] List<Guid> taskIds)
    {
        return await ExecuteRequest(new AssignWorkTasksCommand { WarehouseId = warehouseId, TaskIds = taskIds ?? new List<Guid>() });
    }

    [HttpPost(ApiRoutes.AutonomousWarehouse.Routes)]
    [ValidateApiCode(ApiValidationCodes.WarehouseMapModuleCode.ViewById)]
    [AuthorizeResource(AuthorizePermissions.WarehouseMapPermissions.Controller, AuthorizePermissions.WarehouseMapPermissions.Actions.ViewById)]
    public async Task<IActionResult> GetRoutes([FromQuery] Guid warehouseId, [FromBody] RouteOptimizerRequest request)
    {
        return await ExecuteRequest(new GetOptimizedRouteQuery { WarehouseId = warehouseId, Request = request ?? new() });
    }

    [HttpPost("tasks/{taskId}/complete")]
    [ValidateApiCode(ApiValidationCodes.WarehouseMapModuleCode.Update)]
    [AuthorizeResource(AuthorizePermissions.InventoryPermissions.Controller, AuthorizePermissions.InventoryPermissions.Actions.StockTransactionCreate)]
    public async Task<IActionResult> CompleteTask(Guid taskId, [FromBody] CompleteWorkTaskCommand? body = null)
    {
        return await ExecuteRequest(new CompleteWorkTaskCommand { TaskId = taskId, Notes = body?.Notes });
    }
}
