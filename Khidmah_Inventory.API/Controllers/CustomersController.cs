using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Khidmah_Inventory.API.Attributes;
using Khidmah_Inventory.API.Constants;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Customers.Commands.CreateCustomer;
using Khidmah_Inventory.Application.Features.Customers.Commands.UpdateCustomer;
using Khidmah_Inventory.Application.Features.Customers.Queries.GetCustomersList;
using Khidmah_Inventory.Application.Features.Customers.Queries.GetCustomer;
using Khidmah_Inventory.Application.Features.Customers.Commands.UploadCustomerImage;
using Khidmah_Inventory.Application.Features.Customers.Models;

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

    [HttpGet(ApiRoutes.Customers.GetById)]
    [ValidateApiCode(ApiValidationCodes.CustomersModuleCode.ViewById)]
    [AuthorizeResource(AuthorizePermissions.CustomersPermissions.Controller, AuthorizePermissions.CustomersPermissions.Actions.ViewById)]
    public async Task<IActionResult> GetById(Guid id)
    {
        return await ExecuteRequest<GetCustomerQuery, CustomerDto>(new GetCustomerQuery { Id = id });
    }

    [HttpPost(ApiRoutes.Customers.Add)]
    [ValidateApiCode(ApiValidationCodes.CustomersModuleCode.Add)]
    [AuthorizeResource(AuthorizePermissions.CustomersPermissions.Controller, AuthorizePermissions.CustomersPermissions.Actions.Add)]
    public async Task<IActionResult> Create([FromBody] CreateCustomerCommand command)
    {
        return await ExecuteRequest(command);
    }

    [HttpPut(ApiRoutes.Customers.Update)]
    [ValidateApiCode(ApiValidationCodes.CustomersModuleCode.Update)]
    [AuthorizeResource(AuthorizePermissions.CustomersPermissions.Controller, AuthorizePermissions.CustomersPermissions.Actions.Update)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCustomerCommand command)
    {
        command.Id = id;
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

