using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Khidmah_Inventory.API.Attributes;
using Khidmah_Inventory.API.Constants;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Brands.Commands.CreateBrand;
using Khidmah_Inventory.Application.Features.Brands.Commands.UpdateBrand;
using Khidmah_Inventory.Application.Features.Brands.Commands.DeleteBrand;
using Khidmah_Inventory.Application.Features.Brands.Queries.GetBrand;
using Khidmah_Inventory.Application.Features.Brands.Queries.GetBrandsList;
using Khidmah_Inventory.Application.Features.Brands.Commands.UploadBrandLogo;

namespace Khidmah_Inventory.API.Controllers;

[Route(ApiRoutes.Brands.Base)]
[Authorize]
public class BrandsController : BaseController
{
    public BrandsController(IMediator mediator) : base(mediator)
    {
    }

    [HttpPost(ApiRoutes.Brands.Index)]
    [ValidateApiCode(ApiValidationCodes.BrandsModuleCode.ViewAll)]
    [AuthorizeResource(AuthorizePermissions.BrandsPermissions.Controller, AuthorizePermissions.BrandsPermissions.Actions.ViewAll)]
    public async Task<IActionResult> GetAll([FromBody] FilterRequest request)
    {
        var query = new GetBrandsListQuery { FilterRequest = request };
        return await ExecuteRequest(query);
    }

    [HttpGet(ApiRoutes.Brands.GetById)]
    [ValidateApiCode(ApiValidationCodes.BrandsModuleCode.ViewById)]
    [AuthorizeResource(AuthorizePermissions.BrandsPermissions.Controller, AuthorizePermissions.BrandsPermissions.Actions.ViewById)]
    public async Task<IActionResult> GetById(Guid id)
    {
        return await ExecuteRequestWithCache(new GetBrandQuery { Id = id });
    }

    [HttpPost(ApiRoutes.Brands.Add)]
    [ValidateApiCode(ApiValidationCodes.BrandsModuleCode.Add)]
    [AuthorizeResource(AuthorizePermissions.BrandsPermissions.Controller, AuthorizePermissions.BrandsPermissions.Actions.Add)]
    public async Task<IActionResult> Create([FromBody] CreateBrandCommand command)
    {
        return await ExecuteRequest(command);
    }

    [HttpPut(ApiRoutes.Brands.Update)]
    [ValidateApiCode(ApiValidationCodes.BrandsModuleCode.Update)]
    [AuthorizeResource(AuthorizePermissions.BrandsPermissions.Controller, AuthorizePermissions.BrandsPermissions.Actions.Update)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateBrandCommand command)
    {
        command.Id = id;
        return await ExecuteRequest(command);
    }

    [HttpDelete(ApiRoutes.Brands.Delete)]
    [ValidateApiCode(ApiValidationCodes.BrandsModuleCode.Delete)]
    [AuthorizeResource(AuthorizePermissions.BrandsPermissions.Controller, AuthorizePermissions.BrandsPermissions.Actions.Delete)]
    public async Task<IActionResult> Delete(Guid id)
    {
        return await ExecuteRequest(new DeleteBrandCommand { Id = id });
    }

    [HttpPost(ApiRoutes.Brands.UploadLogo)]
    [ValidateApiCode(ApiValidationCodes.BrandsModuleCode.UploadLogo)]
    [AuthorizeResource(AuthorizePermissions.BrandsPermissions.Controller, AuthorizePermissions.BrandsPermissions.Actions.Update)]
    public async Task<IActionResult> UploadLogo(Guid id, IFormFile file)
    {
        return await ExecuteRequest(new UploadBrandLogoCommand { BrandId = id, File = file });
    }
}