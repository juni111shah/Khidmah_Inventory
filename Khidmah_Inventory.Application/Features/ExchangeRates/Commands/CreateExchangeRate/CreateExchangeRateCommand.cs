using MediatR;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.ExchangeRates.Models;

namespace Khidmah_Inventory.Application.Features.ExchangeRates.Commands.CreateExchangeRate;

public class CreateExchangeRateCommand : IRequest<Result<ExchangeRateDto>>
{
    public Guid FromCurrencyId { get; set; }
    public Guid ToCurrencyId { get; set; }
    public decimal Rate { get; set; }
    public DateTime Date { get; set; }
}
