using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Khidmah_Inventory.API.Attributes;
using Khidmah_Inventory.Application.Features.Customers.Commands.CreateCustomer;
using Khidmah_Inventory.Application.Features.Customers.Queries.GetCustomersList;

namespace Khidmah_Inventory.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CustomersController : BaseApiController
{
    [HttpPost("list")]
    [AuthorizePermission("Customers:List")]
    public async Task<IActionResult> GetList([FromBody] GetCustomersListQuery query)
    {
        var result = await Mediator.Send(query);
        return HandleResult(result, "Customers retrieved successfully");
    }

    [HttpPost]
    [AuthorizePermission("Customers:Create")]
    public async Task<IActionResult> Create([FromBody] CreateCustomerCommand command)
    {
        var result = await Mediator.Send(command);
        return HandleResult(result, "Customer created successfully");
    }
}

