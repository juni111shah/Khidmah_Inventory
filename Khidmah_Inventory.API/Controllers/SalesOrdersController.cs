using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Khidmah_Inventory.API.Attributes;
using Khidmah_Inventory.Application.Features.SalesOrders.Commands.CreateSalesOrder;
using Khidmah_Inventory.Application.Features.SalesOrders.Queries.GetSalesOrdersList;

namespace Khidmah_Inventory.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SalesOrdersController : BaseApiController
{
    [HttpPost("list")]
    [AuthorizePermission("SalesOrders:List")]
    public async Task<IActionResult> GetList([FromBody] GetSalesOrdersListQuery query)
    {
        var result = await Mediator.Send(query);
        return HandleResult(result, "Sales orders retrieved successfully");
    }

    [HttpPost]
    [AuthorizePermission("SalesOrders:Create")]
    public async Task<IActionResult> Create([FromBody] CreateSalesOrderCommand command)
    {
        var result = await Mediator.Send(command);
        return HandleResult(result, "Sales order created successfully");
    }
}

