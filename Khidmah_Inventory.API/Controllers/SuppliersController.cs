using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Khidmah_Inventory.API.Attributes;
using Khidmah_Inventory.API.Constants;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Suppliers.Commands.CreateSupplier;
using Khidmah_Inventory.Application.Features.Suppliers.Commands.UpdateSupplier;
using Khidmah_Inventory.Application.Features.Suppliers.Commands.DeleteSupplier;
using Khidmah_Inventory.Application.Features.Suppliers.Commands.ActivateSupplier;
using Khidmah_Inventory.Application.Features.Suppliers.Commands.DeactivateSupplier;
using Khidmah_Inventory.Application.Features.Suppliers.Queries.GetSupplier;
using Khidmah_Inventory.Application.Features.Suppliers.Queries.GetSuppliersList;
using Khidmah_Inventory.Application.Features.Suppliers.Commands.UploadSupplierImage;

namespace Khidmah_Inventory.API.Controllers;

[Route(ApiRoutes.Suppliers.Base)]
[Authorize]
public class SuppliersController : BaseController
{
    public SuppliersController(IMediator mediator) : base(mediator)
    {
    }

    [HttpPost(ApiRoutes.Suppliers.Index)]
    [ValidateApiCode(ApiValidationCodes.SuppliersModuleCode.ViewAll)]
    [AuthorizeResource(AuthorizePermissions.SuppliersPermissions.Controller, AuthorizePermissions.SuppliersPermissions.Actions.ViewAll)]
    public async Task<IActionResult> GetAll([FromBody] FilterRequest request)
    {
        var query = new GetSuppliersListQuery { FilterRequest = request };
        return await ExecuteRequest(query);
    }

    [HttpGet(ApiRoutes.Suppliers.GetById)]
    [ValidateApiCode(ApiValidationCodes.SuppliersModuleCode.ViewById)]
    [AuthorizeResource(AuthorizePermissions.SuppliersPermissions.Controller, AuthorizePermissions.SuppliersPermissions.Actions.ViewById)]
    public async Task<IActionResult> GetById(Guid id)
    {
        return await ExecuteRequestWithCache(new GetSupplierQuery { Id = id });
    }

    [HttpPost(ApiRoutes.Suppliers.Add)]
    [ValidateApiCode(ApiValidationCodes.SuppliersModuleCode.Add)]
    [AuthorizeResource(AuthorizePermissions.SuppliersPermissions.Controller, AuthorizePermissions.SuppliersPermissions.Actions.Add)]
    public async Task<IActionResult> Create([FromBody] CreateSupplierCommand command)
    {
        return await ExecuteRequest(command);
    }

    [HttpPut(ApiRoutes.Suppliers.Update)]
    [ValidateApiCode(ApiValidationCodes.SuppliersModuleCode.Update)]
    [AuthorizeResource(AuthorizePermissions.SuppliersPermissions.Controller, AuthorizePermissions.SuppliersPermissions.Actions.Update)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateSupplierCommand command)
    {
        command.Id = id;
        return await ExecuteRequest(command);
    }

    [HttpDelete(ApiRoutes.Suppliers.Delete)]
    [ValidateApiCode(ApiValidationCodes.SuppliersModuleCode.Delete)]
    [AuthorizeResource(AuthorizePermissions.SuppliersPermissions.Controller, AuthorizePermissions.SuppliersPermissions.Actions.Delete)]
    public async Task<IActionResult> Delete(Guid id)
    {
        return await ExecuteRequest(new DeleteSupplierCommand { Id = id });
    }

    [HttpPatch(ApiRoutes.Suppliers.Activate)]
    [ValidateApiCode(ApiValidationCodes.SuppliersModuleCode.UpdateStatus)]
    [AuthorizeResource(AuthorizePermissions.SuppliersPermissions.Controller, AuthorizePermissions.SuppliersPermissions.Actions.Update)]
    public async Task<IActionResult> Activate(Guid id)
    {
        return await ExecuteRequest(new ActivateSupplierCommand { Id = id });
    }

    [HttpPatch(ApiRoutes.Suppliers.Deactivate)]
    [ValidateApiCode(ApiValidationCodes.SuppliersModuleCode.UpdateStatus)]
    [AuthorizeResource(AuthorizePermissions.SuppliersPermissions.Controller, AuthorizePermissions.SuppliersPermissions.Actions.Update)]
    public async Task<IActionResult> Deactivate(Guid id)
    {
        return await ExecuteRequest(new DeactivateSupplierCommand { Id = id });
    }

    [HttpPost(ApiRoutes.Suppliers.UploadImage)]
    [ValidateApiCode(ApiValidationCodes.SuppliersModuleCode.UploadImage)]
    [AuthorizeResource(AuthorizePermissions.SuppliersPermissions.Controller, AuthorizePermissions.SuppliersPermissions.Actions.Update)]
    public async Task<IActionResult> UploadImage(Guid id, IFormFile file)
    {
        return await ExecuteRequest(new UploadSupplierImageCommand { SupplierId = id, File = file });
    }
}

