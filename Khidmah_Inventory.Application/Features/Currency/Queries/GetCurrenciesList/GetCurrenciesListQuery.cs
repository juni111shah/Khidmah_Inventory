using MediatR;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Currency.Models;

namespace Khidmah_Inventory.Application.Features.Currency.Queries.GetCurrenciesList;

public class GetCurrenciesListQuery : IRequest<Result<GetCurrenciesListResult>>
{
    public bool IncludeInactive { get; set; }
}

public class GetCurrenciesListResult
{
    public List<CurrencyDto> Items { get; set; } = new();
}
