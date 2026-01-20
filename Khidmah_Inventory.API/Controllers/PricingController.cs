using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Khidmah_Inventory.API.Attributes;
using Khidmah_Inventory.Application.Features.Pricing.Queries.GetPriceSuggestions;

namespace Khidmah_Inventory.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PricingController : BaseApiController
{
    [HttpGet("suggestions")]
    [AuthorizePermission("Pricing:Suggestions:Read")]
    public async Task<IActionResult> GetPriceSuggestions([FromQuery] GetPriceSuggestionsQuery query)
    {
        var result = await Mediator.Send(query);
        return HandleResult(result, "Price suggestions retrieved successfully");
    }
}

