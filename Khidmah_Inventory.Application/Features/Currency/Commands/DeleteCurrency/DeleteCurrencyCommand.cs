using MediatR;
using Khidmah_Inventory.Application.Common.Models;

namespace Khidmah_Inventory.Application.Features.Currency.Commands.DeleteCurrency;

public class DeleteCurrencyCommand : IRequest<Result>
{
    public Guid Id { get; set; }
}
