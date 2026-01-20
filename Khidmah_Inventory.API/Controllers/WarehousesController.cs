using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Khidmah_Inventory.API.Attributes;
using Khidmah_Inventory.Application.Features.Warehouses.Commands.CreateWarehouse;
using Khidmah_Inventory.Application.Features.Warehouses.Commands.UpdateWarehouse;
using Khidmah_Inventory.Application.Features.Warehouses.Commands.DeleteWarehouse;
using Khidmah_Inventory.Application.Features.Warehouses.Commands.ActivateWarehouse;
using Khidmah_Inventory.Application.Features.Warehouses.Commands.DeactivateWarehouse;
using Khidmah_Inventory.Application.Features.Warehouses.Queries.GetWarehouse;
using Khidmah_Inventory.Application.Features.Warehouses.Queries.GetWarehousesList;

namespace Khidmah_Inventory.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class WarehousesController : BaseApiController
{
    [HttpGet("{id}")]
    [AuthorizePermission("Warehouses:Read")]
    public async Task<IActionResult> Get(Guid id)
    {
        var query = new GetWarehouseQuery { Id = id };
        var result = await Mediator.Send(query);
        return HandleResult(result, "Warehouse retrieved successfully");
    }

    [HttpPost("list")]
    [AuthorizePermission("Warehouses:List")]
    public async Task<IActionResult> GetList([FromBody] GetWarehousesListQuery query)
    {
        var result = await Mediator.Send(query);
        return HandleResult(result, "Warehouses retrieved successfully");
    }

    [HttpPost]
    [AuthorizePermission("Warehouses:Create")]
    public async Task<IActionResult> Create([FromBody] CreateWarehouseCommand command)
    {
        var result = await Mediator.Send(command);
        return HandleResult(result, "Warehouse created successfully");
    }

    [HttpPut("{id}")]
    [AuthorizePermission("Warehouses:Update")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateWarehouseCommand command)
    {
        command.Id = id;
        var result = await Mediator.Send(command);
        return HandleResult(result, "Warehouse updated successfully");
    }

    [HttpDelete("{id}")]
    [AuthorizePermission("Warehouses:Delete")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var command = new DeleteWarehouseCommand { Id = id };
        var result = await Mediator.Send(command);
        return HandleResult(result, "Warehouse deleted successfully");
    }

    [HttpPost("{id}/activate")]
    [AuthorizePermission("Warehouses:Update")]
    public async Task<IActionResult> Activate(Guid id)
    {
        var command = new ActivateWarehouseCommand { Id = id };
        var result = await Mediator.Send(command);
        return HandleResult(result, "Warehouse activated successfully");
    }

    [HttpPost("{id}/deactivate")]
    [AuthorizePermission("Warehouses:Update")]
    public async Task<IActionResult> Deactivate(Guid id)
    {
        var command = new DeactivateWarehouseCommand { Id = id };
        var result = await Mediator.Send(command);
        return HandleResult(result, "Warehouse deactivated successfully");
    }
}

