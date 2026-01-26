using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Khidmah_Inventory.API.Attributes;
using Khidmah_Inventory.API.Constants;
using Khidmah_Inventory.Application.Features.Reordering.Queries.GetReorderSuggestions;
using Khidmah_Inventory.Application.Features.Reordering.Commands.GeneratePurchaseOrderFromSuggestions;

namespace Khidmah_Inventory.API.Controllers;

[Route(ApiRoutes.Reordering.Base)]
[Authorize]
public class ReorderingController : BaseController
{
    public ReorderingController(IMediator mediator) : base(mediator)
    {
    }

    [HttpGet(ApiRoutes.Reordering.Suggestions)]
    [ValidateApiCode(ApiValidationCodes.ReorderingModuleCode.Suggestions)]
    [AuthorizeResource(AuthorizePermissions.ReorderingPermissions.Controller, AuthorizePermissions.ReorderingPermissions.Actions.Suggestions)]
    public async Task<IActionResult> GetReorderSuggestions([FromQuery] GetReorderSuggestionsQuery query)
    {
        return await ExecuteRequest(query);
    }

    [HttpPost(ApiRoutes.Reordering.GeneratePO)]
    [ValidateApiCode(ApiValidationCodes.ReorderingModuleCode.GeneratePO)]
    [AuthorizeResource(AuthorizePermissions.ReorderingPermissions.Controller, AuthorizePermissions.ReorderingPermissions.Actions.GeneratePO)]
    public async Task<IActionResult> GeneratePurchaseOrder([FromBody] GeneratePurchaseOrderFromSuggestionsCommand command)
    {
        return await ExecuteRequest(command);
    }
}

