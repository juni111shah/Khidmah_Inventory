using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Khidmah_Inventory.API.Attributes;
using Khidmah_Inventory.API.Constants;
using Khidmah_Inventory.Application.Features.ExchangeRates.Commands.CreateExchangeRate;
using Khidmah_Inventory.Application.Features.ExchangeRates.Queries.GetExchangeRates;

namespace Khidmah_Inventory.API.Controllers;

[Route(ApiRoutes.ExchangeRates.Base)]
[Authorize]
public class ExchangeRatesController : BaseController
{
    public ExchangeRatesController(IMediator mediator) : base(mediator)
    {
    }

    [HttpGet(ApiRoutes.ExchangeRates.List)]
    [ValidateApiCode(ApiValidationCodes.ExchangeRatesModuleCode.List)]
    [AuthorizeResource(AuthorizePermissions.ExchangeRatesPermissions.Controller, AuthorizePermissions.ExchangeRatesPermissions.Actions.List)]
    public async Task<IActionResult> GetExchangeRates(
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate,
        [FromQuery] Guid? fromCurrencyId,
        [FromQuery] Guid? toCurrencyId)
    {
        return await ExecuteRequest(new GetExchangeRatesQuery
        {
            FromDate = fromDate,
            ToDate = toDate,
            FromCurrencyId = fromCurrencyId,
            ToCurrencyId = toCurrencyId
        });
    }

    [HttpPost(ApiRoutes.ExchangeRates.Add)]
    [ValidateApiCode(ApiValidationCodes.ExchangeRatesModuleCode.Create)]
    [AuthorizeResource(AuthorizePermissions.ExchangeRatesPermissions.Controller, AuthorizePermissions.ExchangeRatesPermissions.Actions.Create)]
    public async Task<IActionResult> CreateExchangeRate([FromBody] CreateExchangeRateCommand command)
    {
        return await ExecuteRequest(command);
    }
}
