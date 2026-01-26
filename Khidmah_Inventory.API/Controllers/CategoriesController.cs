using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Khidmah_Inventory.API.Attributes;
using Khidmah_Inventory.API.Constants;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Categories.Commands.CreateCategory;
using Khidmah_Inventory.Application.Features.Categories.Commands.UpdateCategory;
using Khidmah_Inventory.Application.Features.Categories.Commands.DeleteCategory;
using Khidmah_Inventory.Application.Features.Categories.Queries.GetCategory;
using Khidmah_Inventory.Application.Features.Categories.Queries.GetCategoriesList;
using Khidmah_Inventory.Application.Features.Categories.Queries.GetCategoryTree;
using Khidmah_Inventory.Application.Features.Categories.Commands.UploadCategoryImage;

namespace Khidmah_Inventory.API.Controllers;

[Route(ApiRoutes.Categories.Base)]
[Authorize]
public class CategoriesController : BaseController
{
    public CategoriesController(IMediator mediator) : base(mediator)
    {
    }

    [HttpPost(ApiRoutes.Categories.Index)]
    [ValidateApiCode(ApiValidationCodes.CategoriesModuleCode.ViewAll)]
    [AuthorizeResource(AuthorizePermissions.CategoriesPermissions.Controller, AuthorizePermissions.CategoriesPermissions.Actions.ViewAll)]
    public async Task<IActionResult> GetAll([FromBody] FilterRequest request)
    {
        var query = new GetCategoriesListQuery { FilterRequest = request };
        return await ExecuteRequest(query);
    }

    [HttpGet(ApiRoutes.Categories.GetById)]
    [ValidateApiCode(ApiValidationCodes.CategoriesModuleCode.ViewById)]
    [AuthorizeResource(AuthorizePermissions.CategoriesPermissions.Controller, AuthorizePermissions.CategoriesPermissions.Actions.ViewById)]
    public async Task<IActionResult> GetById(Guid id)
    {
        return await ExecuteRequestWithCache(new GetCategoryQuery { Id = id });
    }

    [HttpGet(ApiRoutes.Categories.Tree)]
    [ValidateApiCode(ApiValidationCodes.CategoriesModuleCode.ViewTree)]
    [AuthorizeResource(AuthorizePermissions.CategoriesPermissions.Controller, AuthorizePermissions.CategoriesPermissions.Actions.ViewAll)]
    public async Task<IActionResult> GetTree()
    {
        return await ExecuteRequest(new GetCategoryTreeQuery());
    }

    [HttpPost(ApiRoutes.Categories.Add)]
    [ValidateApiCode(ApiValidationCodes.CategoriesModuleCode.Add)]
    [AuthorizeResource(AuthorizePermissions.CategoriesPermissions.Controller, AuthorizePermissions.CategoriesPermissions.Actions.Add)]
    public async Task<IActionResult> Create([FromBody] CreateCategoryCommand command)
    {
        return await ExecuteRequest(command);
    }

    [HttpPut(ApiRoutes.Categories.Update)]
    [ValidateApiCode(ApiValidationCodes.CategoriesModuleCode.Update)]
    [AuthorizeResource(AuthorizePermissions.CategoriesPermissions.Controller, AuthorizePermissions.CategoriesPermissions.Actions.Update)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCategoryCommand command)
    {
        command.Id = id;
        return await ExecuteRequest(command);
    }

    [HttpDelete(ApiRoutes.Categories.Delete)]
    [ValidateApiCode(ApiValidationCodes.CategoriesModuleCode.Delete)]
    [AuthorizeResource(AuthorizePermissions.CategoriesPermissions.Controller, AuthorizePermissions.CategoriesPermissions.Actions.Delete)]
    public async Task<IActionResult> Delete(Guid id)
    {
        return await ExecuteRequest(new DeleteCategoryCommand { Id = id });
    }

    [HttpPost(ApiRoutes.Categories.UploadImage)]
    [ValidateApiCode(ApiValidationCodes.CategoriesModuleCode.UploadImage)]
    [AuthorizeResource(AuthorizePermissions.CategoriesPermissions.Controller, AuthorizePermissions.CategoriesPermissions.Actions.Update)]
    public async Task<IActionResult> UploadImage(Guid id, IFormFile file)
    {
        return await ExecuteRequest(new UploadCategoryImageCommand { CategoryId = id, File = file });
    }
}

