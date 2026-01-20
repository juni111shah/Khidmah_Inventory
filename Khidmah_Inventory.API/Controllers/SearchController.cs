using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Khidmah_Inventory.API.Attributes;
using Khidmah_Inventory.Application.Features.Search.Queries.GlobalSearch;

namespace Khidmah_Inventory.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SearchController : BaseApiController
{
    [HttpGet]
    [AuthorizePermission("Search:Global:Read")]
    public async Task<IActionResult> GlobalSearch([FromQuery] GlobalSearchQuery query)
    {
        var result = await Mediator.Send(query);
        return HandleResult(result, "Search completed successfully");
    }
}

