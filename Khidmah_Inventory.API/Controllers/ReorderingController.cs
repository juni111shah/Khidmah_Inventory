using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Khidmah_Inventory.API.Attributes;
using Khidmah_Inventory.Application.Features.Reordering.Queries.GetReorderSuggestions;
using Khidmah_Inventory.Application.Features.Reordering.Commands.GeneratePurchaseOrderFromSuggestions;

namespace Khidmah_Inventory.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ReorderingController : BaseApiController
{
    [HttpGet("suggestions")]
    [AuthorizePermission("Reordering:Suggestions:Read")]
    public async Task<IActionResult> GetReorderSuggestions([FromQuery] GetReorderSuggestionsQuery query)
    {
        var result = await Mediator.Send(query);
        return HandleResult(result, "Reorder suggestions retrieved successfully");
    }

    [HttpPost("generate-po")]
    [AuthorizePermission("Reordering:GeneratePO:Create")]
    public async Task<IActionResult> GeneratePurchaseOrder([FromBody] GeneratePurchaseOrderFromSuggestionsCommand command)
    {
        var result = await Mediator.Send(command);
        return HandleResult(result, "Purchase order generated successfully");
    }
}

