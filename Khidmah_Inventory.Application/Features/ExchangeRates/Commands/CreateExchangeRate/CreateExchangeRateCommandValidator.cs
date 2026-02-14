using FluentValidation;

namespace Khidmah_Inventory.Application.Features.ExchangeRates.Commands.CreateExchangeRate;

public class CreateExchangeRateCommandValidator : AbstractValidator<CreateExchangeRateCommand>
{
    public CreateExchangeRateCommandValidator()
    {
        RuleFor(x => x.FromCurrencyId).NotEmpty();
        RuleFor(x => x.ToCurrencyId).NotEmpty();
        RuleFor(x => x.Rate).GreaterThan(0);
        RuleFor(x => x.Date).NotEmpty();
    }
}
