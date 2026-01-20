using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Khidmah_Inventory.API.Attributes;
using Khidmah_Inventory.Application.Features.Products.Commands.CreateProduct;
using Khidmah_Inventory.Application.Features.Products.Commands.UpdateProduct;
using Khidmah_Inventory.Application.Features.Products.Commands.DeleteProduct;
using Khidmah_Inventory.Application.Features.Products.Commands.ActivateProduct;
using Khidmah_Inventory.Application.Features.Products.Commands.DeactivateProduct;
using Khidmah_Inventory.Application.Features.Products.Queries.GetProduct;
using Khidmah_Inventory.Application.Features.Products.Queries.GetProductsList;

namespace Khidmah_Inventory.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProductsController : BaseApiController
{
    [HttpGet("{id}")]
    [AuthorizePermission("Products:Read")]
    public async Task<IActionResult> Get(Guid id)
    {
        var query = new GetProductQuery { Id = id };
        var result = await Mediator.Send(query);
        return HandleResult(result, "Product retrieved successfully");
    }

    [HttpPost("list")]
    [AuthorizePermission("Products:List")]
    public async Task<IActionResult> GetList([FromBody] GetProductsListQuery query)
    {
        var result = await Mediator.Send(query);
        return HandleResult(result, "Products retrieved successfully");
    }

    [HttpPost]
    [AuthorizePermission("Products:Create")]
    public async Task<IActionResult> Create([FromBody] CreateProductCommand command)
    {
        var result = await Mediator.Send(command);
        return HandleResult(result, "Product created successfully");
    }

    [HttpPut("{id}")]
    [AuthorizePermission("Products:Update")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProductCommand command)
    {
        command.Id = id;
        var result = await Mediator.Send(command);
        return HandleResult(result, "Product updated successfully");
    }

    [HttpDelete("{id}")]
    [AuthorizePermission("Products:Delete")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var command = new DeleteProductCommand { Id = id };
        var result = await Mediator.Send(command);
        return HandleResult(result, "Product deleted successfully");
    }

    [HttpPost("{id}/activate")]
    [AuthorizePermission("Products:Update")]
    public async Task<IActionResult> Activate(Guid id)
    {
        var command = new ActivateProductCommand { Id = id };
        var result = await Mediator.Send(command);
        return HandleResult(result, "Product activated successfully");
    }

    [HttpPost("{id}/deactivate")]
    [AuthorizePermission("Products:Update")]
    public async Task<IActionResult> Deactivate(Guid id)
    {
        var command = new DeactivateProductCommand { Id = id };
        var result = await Mediator.Send(command);
        return HandleResult(result, "Product deactivated successfully");
    }
}
