using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Khidmah_Inventory.API.Attributes;
using Khidmah_Inventory.Application.Features.Suppliers.Commands.CreateSupplier;
using Khidmah_Inventory.Application.Features.Suppliers.Commands.UpdateSupplier;
using Khidmah_Inventory.Application.Features.Suppliers.Commands.DeleteSupplier;
using Khidmah_Inventory.Application.Features.Suppliers.Commands.ActivateSupplier;
using Khidmah_Inventory.Application.Features.Suppliers.Commands.DeactivateSupplier;
using Khidmah_Inventory.Application.Features.Suppliers.Queries.GetSupplier;
using Khidmah_Inventory.Application.Features.Suppliers.Queries.GetSuppliersList;

namespace Khidmah_Inventory.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SuppliersController : BaseApiController
{
    [HttpGet("{id}")]
    [AuthorizePermission("Suppliers:Read")]
    public async Task<IActionResult> Get(Guid id)
    {
        var query = new GetSupplierQuery { Id = id };
        var result = await Mediator.Send(query);
        return HandleResult(result, "Supplier retrieved successfully");
    }

    [HttpPost("list")]
    [AuthorizePermission("Suppliers:List")]
    public async Task<IActionResult> GetList([FromBody] GetSuppliersListQuery query)
    {
        var result = await Mediator.Send(query);
        return HandleResult(result, "Suppliers retrieved successfully");
    }

    [HttpPost]
    [AuthorizePermission("Suppliers:Create")]
    public async Task<IActionResult> Create([FromBody] CreateSupplierCommand command)
    {
        var result = await Mediator.Send(command);
        return HandleResult(result, "Supplier created successfully");
    }

    [HttpPut("{id}")]
    [AuthorizePermission("Suppliers:Update")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateSupplierCommand command)
    {
        command.Id = id;
        var result = await Mediator.Send(command);
        return HandleResult(result, "Supplier updated successfully");
    }

    [HttpDelete("{id}")]
    [AuthorizePermission("Suppliers:Delete")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var command = new DeleteSupplierCommand { Id = id };
        var result = await Mediator.Send(command);
        return HandleResult(result, "Supplier deleted successfully");
    }

    [HttpPost("{id}/activate")]
    [AuthorizePermission("Suppliers:Update")]
    public async Task<IActionResult> Activate(Guid id)
    {
        var command = new ActivateSupplierCommand { Id = id };
        var result = await Mediator.Send(command);
        return HandleResult(result, "Supplier activated successfully");
    }

    [HttpPost("{id}/deactivate")]
    [AuthorizePermission("Suppliers:Update")]
    public async Task<IActionResult> Deactivate(Guid id)
    {
        var command = new DeactivateSupplierCommand { Id = id };
        var result = await Mediator.Send(command);
        return HandleResult(result, "Supplier deactivated successfully");
    }
}

