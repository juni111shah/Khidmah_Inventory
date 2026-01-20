using FluentValidation;

namespace Khidmah_Inventory.Application.Features.Settings.Commands.SaveCompanySettings;

public class SaveCompanySettingsCommandValidator : AbstractValidator<SaveCompanySettingsCommand>
{
    public SaveCompanySettingsCommandValidator()
    {
        RuleFor(x => x.Settings)
            .NotNull()
            .WithMessage("Settings data is required");

        RuleFor(x => x.Settings.Name)
            .NotEmpty()
            .WithMessage("Company name is required")
            .MaximumLength(200)
            .WithMessage("Company name must not exceed 200 characters");

        RuleFor(x => x.Settings.Currency)
            .NotEmpty()
            .WithMessage("Currency is required")
            .MaximumLength(10)
            .WithMessage("Currency code must not exceed 10 characters");

        RuleFor(x => x.Settings.TimeZone)
            .NotEmpty()
            .WithMessage("Time zone is required");
    }
}

