using MediatR;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Currency.Models;

namespace Khidmah_Inventory.Application.Features.Currency.Queries.GetCurrency;

public class GetCurrencyQuery : IRequest<Result<CurrencyDto>>
{
    public Guid Id { get; set; }
}
