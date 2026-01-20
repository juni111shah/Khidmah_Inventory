using FluentValidation;

namespace Khidmah_Inventory.Application.Features.Settings.Commands.SaveUserSettings;

public class SaveUserSettingsCommandValidator : AbstractValidator<SaveUserSettingsCommand>
{
    public SaveUserSettingsCommandValidator()
    {
        RuleFor(x => x.Settings)
            .NotNull()
            .WithMessage("Settings data is required");

        RuleFor(x => x.Settings.Email)
            .NotEmpty()
            .WithMessage("Email is required")
            .EmailAddress()
            .WithMessage("Invalid email format");
    }
}

