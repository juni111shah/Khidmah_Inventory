using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Khidmah_Inventory.API.Attributes;
using Khidmah_Inventory.API.Constants;
using Khidmah_Inventory.Application.Features.Search.Queries.GlobalSearch;

namespace Khidmah_Inventory.API.Controllers;

[Route(ApiRoutes.Search.Base)]
[Authorize]
public class SearchController : BaseController
{
    public SearchController(IMediator mediator) : base(mediator)
    {
    }

    [HttpGet(ApiRoutes.Search.Global)]
    [ValidateApiCode(ApiValidationCodes.SearchModuleCode.Global)]
    [AuthorizeResource(AuthorizePermissions.SearchPermissions.Controller, AuthorizePermissions.SearchPermissions.Actions.Global)]
    public async Task<IActionResult> GlobalSearch([FromQuery] GlobalSearchQuery query)
    {
        return await ExecuteRequest(query);
    }
}

