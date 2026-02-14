using MediatR;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.ExchangeRates.Models;

namespace Khidmah_Inventory.Application.Features.ExchangeRates.Queries.GetExchangeRates;

public class GetExchangeRatesQuery : IRequest<Result<GetExchangeRatesResult>>
{
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public Guid? FromCurrencyId { get; set; }
    public Guid? ToCurrencyId { get; set; }
}

public class GetExchangeRatesResult
{
    public List<ExchangeRateDto> Items { get; set; } = new();
}
