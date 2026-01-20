using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Khidmah_Inventory.API.Attributes;
using Khidmah_Inventory.Application.Features.PurchaseOrders.Commands.CreatePurchaseOrder;
using Khidmah_Inventory.Application.Features.PurchaseOrders.Queries.GetPurchaseOrder;
using Khidmah_Inventory.Application.Features.PurchaseOrders.Queries.GetPurchaseOrdersList;

namespace Khidmah_Inventory.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PurchaseOrdersController : BaseApiController
{
    [HttpGet("{id}")]
    [AuthorizePermission("PurchaseOrders:Read")]
    public async Task<IActionResult> Get(Guid id)
    {
        var query = new GetPurchaseOrderQuery { Id = id };
        var result = await Mediator.Send(query);
        return HandleResult(result, "Purchase order retrieved successfully");
    }

    [HttpPost("list")]
    [AuthorizePermission("PurchaseOrders:List")]
    public async Task<IActionResult> GetList([FromBody] GetPurchaseOrdersListQuery query)
    {
        var result = await Mediator.Send(query);
        return HandleResult(result, "Purchase orders retrieved successfully");
    }

    [HttpPost]
    [AuthorizePermission("PurchaseOrders:Create")]
    public async Task<IActionResult> Create([FromBody] CreatePurchaseOrderCommand command)
    {
        var result = await Mediator.Send(command);
        return HandleResult(result, "Purchase order created successfully");
    }
}

