using FluentValidation;

namespace Khidmah_Inventory.Application.Features.Theme.Commands.SaveGlobalTheme;

public class SaveGlobalThemeCommandValidator : AbstractValidator<SaveGlobalThemeCommand>
{
    public SaveGlobalThemeCommandValidator()
    {
        RuleFor(x => x.Theme)
            .NotNull()
            .WithMessage("Theme data is required");
    }
}

