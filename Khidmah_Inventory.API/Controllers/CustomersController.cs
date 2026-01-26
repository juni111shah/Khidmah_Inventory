using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Khidmah_Inventory.API.Attributes;
using Khidmah_Inventory.API.Constants;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Customers.Commands.CreateCustomer;
using Khidmah_Inventory.Application.Features.Customers.Queries.GetCustomersList;
using Khidmah_Inventory.Application.Features.Customers.Commands.UploadCustomerImage;

namespace Khidmah_Inventory.API.Controllers;

[Route(ApiRoutes.Customers.Base)]
[Authorize]
public class CustomersController : BaseController
{
    public CustomersController(IMediator mediator) : base(mediator)
    {
    }

    [HttpPost(ApiRoutes.Customers.Index)]
    [ValidateApiCode(ApiValidationCodes.CustomersModuleCode.ViewAll)]
    [AuthorizeResource(AuthorizePermissions.CustomersPermissions.Controller, AuthorizePermissions.CustomersPermissions.Actions.ViewAll)]
    public async Task<IActionResult> GetAll([FromBody] FilterRequest request)
    {
        var query = new GetCustomersListQuery { FilterRequest = request };
        return await ExecuteRequest(query);
    }

    [HttpPost(ApiRoutes.Customers.Add)]
    [ValidateApiCode(ApiValidationCodes.CustomersModuleCode.Add)]
    [AuthorizeResource(AuthorizePermissions.CustomersPermissions.Controller, AuthorizePermissions.CustomersPermissions.Actions.Add)]
    public async Task<IActionResult> Create([FromBody] CreateCustomerCommand command)
    {
        return await ExecuteRequest(command);
    }

    [HttpPost(ApiRoutes.Customers.UploadImage)]
    [ValidateApiCode(ApiValidationCodes.CustomersModuleCode.UploadImage)]
    [AuthorizeResource(AuthorizePermissions.CustomersPermissions.Controller, AuthorizePermissions.CustomersPermissions.Actions.Update)]
    public async Task<IActionResult> UploadImage(Guid id, IFormFile file)
    {
        return await ExecuteRequest(new UploadCustomerImageCommand { CustomerId = id, File = file });
    }
}

