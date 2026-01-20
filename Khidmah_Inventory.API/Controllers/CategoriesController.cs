using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Khidmah_Inventory.API.Attributes;
using Khidmah_Inventory.Application.Features.Categories.Commands.CreateCategory;
using Khidmah_Inventory.Application.Features.Categories.Commands.UpdateCategory;
using Khidmah_Inventory.Application.Features.Categories.Commands.DeleteCategory;
using Khidmah_Inventory.Application.Features.Categories.Queries.GetCategory;
using Khidmah_Inventory.Application.Features.Categories.Queries.GetCategoriesList;
using Khidmah_Inventory.Application.Features.Categories.Queries.GetCategoryTree;

namespace Khidmah_Inventory.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CategoriesController : BaseApiController
{
    [HttpGet("{id}")]
    [AuthorizePermission("Categories:Read")]
    public async Task<IActionResult> Get(Guid id)
    {
        var query = new GetCategoryQuery { Id = id };
        var result = await Mediator.Send(query);
        return HandleResult(result, "Category retrieved successfully");
    }

    [HttpPost("list")]
    [AuthorizePermission("Categories:List")]
    public async Task<IActionResult> GetList([FromBody] GetCategoriesListQuery query)
    {
        var result = await Mediator.Send(query);
        return HandleResult(result, "Categories retrieved successfully");
    }

    [HttpGet("tree")]
    [AuthorizePermission("Categories:List")]
    public async Task<IActionResult> GetTree()
    {
        var query = new GetCategoryTreeQuery();
        var result = await Mediator.Send(query);
        return HandleResult(result, "Category tree retrieved successfully");
    }

    [HttpPost]
    [AuthorizePermission("Categories:Create")]
    public async Task<IActionResult> Create([FromBody] CreateCategoryCommand command)
    {
        var result = await Mediator.Send(command);
        return HandleResult(result, "Category created successfully");
    }

    [HttpPut("{id}")]
    [AuthorizePermission("Categories:Update")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCategoryCommand command)
    {
        command.Id = id;
        var result = await Mediator.Send(command);
        return HandleResult(result, "Category updated successfully");
    }

    [HttpDelete("{id}")]
    [AuthorizePermission("Categories:Delete")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var command = new DeleteCategoryCommand { Id = id };
        var result = await Mediator.Send(command);
        return HandleResult(result, "Category deleted successfully");
    }
}

