using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Khidmah_Inventory.API.Attributes;
using Khidmah_Inventory.API.Constants;
using Khidmah_Inventory.Application.Features.Pricing.Queries.GetPriceSuggestions;

namespace Khidmah_Inventory.API.Controllers;

[Route(ApiRoutes.Pricing.Base)]
[Authorize]
public class PricingController : BaseController
{
    public PricingController(IMediator mediator) : base(mediator)
    {
    }

    [HttpGet(ApiRoutes.Pricing.Suggestions)]
    [ValidateApiCode(ApiValidationCodes.PricingModuleCode.Suggestions)]
    [AuthorizeResource(AuthorizePermissions.PricingPermissions.Controller, AuthorizePermissions.PricingPermissions.Actions.Suggestions)]
    public async Task<IActionResult> GetPriceSuggestions([FromQuery] GetPriceSuggestionsQuery query)
    {
        return await ExecuteRequest(query);
    }
}

