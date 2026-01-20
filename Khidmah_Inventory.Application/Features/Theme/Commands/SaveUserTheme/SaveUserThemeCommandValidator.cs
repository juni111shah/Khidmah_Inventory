using FluentValidation;

namespace Khidmah_Inventory.Application.Features.Theme.Commands.SaveUserTheme;

public class SaveUserThemeCommandValidator : AbstractValidator<SaveUserThemeCommand>
{
    public SaveUserThemeCommandValidator()
    {
        RuleFor(x => x.Theme)
            .NotNull()
            .WithMessage("Theme data is required");
    }
}

