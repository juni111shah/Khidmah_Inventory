using FluentValidation;

namespace Khidmah_Inventory.Application.Features.Settings.Commands.SaveSystemSettings;

public class SaveSystemSettingsCommandValidator : AbstractValidator<SaveSystemSettingsCommand>
{
    public SaveSystemSettingsCommandValidator()
    {
        RuleFor(x => x.Settings)
            .NotNull()
            .WithMessage("Settings data is required");

        RuleFor(x => x.Settings.DefaultTaxRate)
            .GreaterThanOrEqualTo(0)
            .LessThanOrEqualTo(100)
            .WithMessage("Tax rate must be between 0 and 100");

        RuleFor(x => x.Settings.LowStockThreshold)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Low stock threshold must be greater than or equal to 0");

        RuleFor(x => x.Settings.SessionTimeout)
            .GreaterThan(0)
            .WithMessage("Session timeout must be greater than 0");

        RuleFor(x => x.Settings.PasswordMinLength)
            .GreaterThanOrEqualTo(6)
            .WithMessage("Password minimum length must be at least 6");
    }
}

