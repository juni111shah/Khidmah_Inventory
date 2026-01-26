using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Khidmah_Inventory.API.Attributes;
using Khidmah_Inventory.API.Constants;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Products.Commands.CreateProduct;
using Khidmah_Inventory.Application.Features.Products.Commands.UpdateProduct;
using Khidmah_Inventory.Application.Features.Products.Commands.DeleteProduct;
using Khidmah_Inventory.Application.Features.Products.Commands.ActivateProduct;
using Khidmah_Inventory.Application.Features.Products.Commands.DeactivateProduct;
using Khidmah_Inventory.Application.Features.Products.Queries.GetProduct;
using Khidmah_Inventory.Application.Features.Products.Queries.GetProductsList;
using Khidmah_Inventory.Application.Features.Products.Commands.UploadProductImage;

namespace Khidmah_Inventory.API.Controllers;

[Route(ApiRoutes.Products.Base)]
[Authorize]
public class ProductsController : BaseController
{
    public ProductsController(IMediator mediator) : base(mediator)
    {
    }

    [HttpPost(ApiRoutes.Products.Index)]
    [ValidateApiCode(ApiValidationCodes.ProductsModuleCode.ViewAll)]
    [AuthorizeResource(AuthorizePermissions.ProductsPermissions.Controller, AuthorizePermissions.ProductsPermissions.Actions.ViewAll)]
    public async Task<IActionResult> GetAll([FromBody] FilterRequest request)
    {
        var query = new GetProductsListQuery { FilterRequest = request };
        return await ExecuteRequest(query);
    }

    [HttpGet(ApiRoutes.Products.GetById)]
    [ValidateApiCode(ApiValidationCodes.ProductsModuleCode.ViewById)]
    [AuthorizeResource(AuthorizePermissions.ProductsPermissions.Controller, AuthorizePermissions.ProductsPermissions.Actions.ViewById)]
    public async Task<IActionResult> GetById(Guid id)
    {
        return await ExecuteRequestWithCache(new GetProductQuery { Id = id });
    }

    [HttpPost(ApiRoutes.Products.Add)]
    [ValidateApiCode(ApiValidationCodes.ProductsModuleCode.Add)]
    [AuthorizeResource(AuthorizePermissions.ProductsPermissions.Controller, AuthorizePermissions.ProductsPermissions.Actions.Add)]
    public async Task<IActionResult> Create([FromBody] CreateProductCommand command)
    {
        return await ExecuteRequest(command);
    }

    [HttpPut(ApiRoutes.Products.Update)]
    [ValidateApiCode(ApiValidationCodes.ProductsModuleCode.Update)]
    [AuthorizeResource(AuthorizePermissions.ProductsPermissions.Controller, AuthorizePermissions.ProductsPermissions.Actions.Update)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProductCommand command)
    {
        command.Id = id;
        return await ExecuteRequest(command);
    }

    [HttpDelete(ApiRoutes.Products.Delete)]
    [ValidateApiCode(ApiValidationCodes.ProductsModuleCode.Delete)]
    [AuthorizeResource(AuthorizePermissions.ProductsPermissions.Controller, AuthorizePermissions.ProductsPermissions.Actions.Delete)]
    public async Task<IActionResult> Delete(Guid id)
    {
        return await ExecuteRequest(new DeleteProductCommand { Id = id });
    }

    [HttpPatch(ApiRoutes.Products.Activate)]
    [ValidateApiCode(ApiValidationCodes.ProductsModuleCode.UpdateStatus)]
    [AuthorizeResource(AuthorizePermissions.ProductsPermissions.Controller, AuthorizePermissions.ProductsPermissions.Actions.Update)]
    public async Task<IActionResult> Activate(Guid id)
    {
        return await ExecuteRequest(new ActivateProductCommand { Id = id });
    }

    [HttpPatch(ApiRoutes.Products.Deactivate)]
    [ValidateApiCode(ApiValidationCodes.ProductsModuleCode.UpdateStatus)]
    [AuthorizeResource(AuthorizePermissions.ProductsPermissions.Controller, AuthorizePermissions.ProductsPermissions.Actions.Update)]
    public async Task<IActionResult> Deactivate(Guid id)
    {
        return await ExecuteRequest(new DeactivateProductCommand { Id = id });
    }

    [HttpPost(ApiRoutes.Products.UploadImage)]
    [ValidateApiCode(ApiValidationCodes.ProductsModuleCode.UploadImage)]
    [AuthorizeResource(AuthorizePermissions.ProductsPermissions.Controller, AuthorizePermissions.ProductsPermissions.Actions.Update)]
    public async Task<IActionResult> UploadImage(Guid id, IFormFile file, [FromForm] string? altText, [FromForm] bool isPrimary = false)
    {
        var command = new UploadProductImageCommand
        {
            ProductId = id,
            File = file,
            AltText = altText,
            IsPrimary = isPrimary
        };
        return await ExecuteRequest(command);
    }
}
