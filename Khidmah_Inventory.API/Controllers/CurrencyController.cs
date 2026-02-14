using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Khidmah_Inventory.API.Attributes;
using Khidmah_Inventory.API.Constants;
using Khidmah_Inventory.Application.Features.Currency.Commands.CreateCurrency;
using Khidmah_Inventory.Application.Features.Currency.Commands.UpdateCurrency;
using Khidmah_Inventory.Application.Features.Currency.Commands.DeleteCurrency;
using Khidmah_Inventory.Application.Features.Currency.Queries.GetCurrency;
using Khidmah_Inventory.Application.Features.Currency.Queries.GetCurrenciesList;

namespace Khidmah_Inventory.API.Controllers;

[Route(ApiRoutes.Currency.Base)]
[Authorize]
public class CurrencyController : BaseController
{
    public CurrencyController(IMediator mediator) : base(mediator)
    {
    }

    [HttpGet(ApiRoutes.Currency.List)]
    [ValidateApiCode(ApiValidationCodes.CurrencyModuleCode.List)]
    [AuthorizeResource(AuthorizePermissions.CurrencyPermissions.Controller, AuthorizePermissions.CurrencyPermissions.Actions.List)]
    public async Task<IActionResult> GetCurrenciesList([FromQuery] bool includeInactive = false)
    {
        return await ExecuteRequest(new GetCurrenciesListQuery { IncludeInactive = includeInactive });
    }

    [HttpGet(ApiRoutes.Currency.GetById)]
    [ValidateApiCode(ApiValidationCodes.CurrencyModuleCode.GetById)]
    [AuthorizeResource(AuthorizePermissions.CurrencyPermissions.Controller, AuthorizePermissions.CurrencyPermissions.Actions.Read)]
    public async Task<IActionResult> GetCurrency(Guid id)
    {
        return await ExecuteRequest(new GetCurrencyQuery { Id = id });
    }

    [HttpPost(ApiRoutes.Currency.Add)]
    [ValidateApiCode(ApiValidationCodes.CurrencyModuleCode.Create)]
    [AuthorizeResource(AuthorizePermissions.CurrencyPermissions.Controller, AuthorizePermissions.CurrencyPermissions.Actions.Create)]
    public async Task<IActionResult> CreateCurrency([FromBody] CreateCurrencyCommand command)
    {
        return await ExecuteRequest(command);
    }

    [HttpPut(ApiRoutes.Currency.Update)]
    [ValidateApiCode(ApiValidationCodes.CurrencyModuleCode.Update)]
    [AuthorizeResource(AuthorizePermissions.CurrencyPermissions.Controller, AuthorizePermissions.CurrencyPermissions.Actions.Update)]
    public async Task<IActionResult> UpdateCurrency(Guid id, [FromBody] UpdateCurrencyCommand command)
    {
        command.Id = id;
        return await ExecuteRequest(command);
    }

    [HttpDelete(ApiRoutes.Currency.Delete)]
    [ValidateApiCode(ApiValidationCodes.CurrencyModuleCode.Delete)]
    [AuthorizeResource(AuthorizePermissions.CurrencyPermissions.Controller, AuthorizePermissions.CurrencyPermissions.Actions.Delete)]
    public async Task<IActionResult> DeleteCurrency(Guid id)
    {
        return await ExecuteRequest(new DeleteCurrencyCommand { Id = id });
    }
}
