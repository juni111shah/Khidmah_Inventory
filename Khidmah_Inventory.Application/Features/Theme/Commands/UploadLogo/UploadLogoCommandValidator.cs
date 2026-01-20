using FluentValidation;

namespace Khidmah_Inventory.Application.Features.Theme.Commands.UploadLogo;

public class UploadLogoCommandValidator : AbstractValidator<UploadLogoCommand>
{
    public UploadLogoCommandValidator()
    {
        RuleFor(x => x.File)
            .NotNull()
            .WithMessage("File is required");

        RuleFor(x => x.File)
            .Must(file => file != null && file.Length > 0)
            .When(x => x.File != null)
            .WithMessage("File cannot be empty");

        RuleFor(x => x.File)
            .Must(file => file != null && file.Length <= 5 * 1024 * 1024)
            .When(x => x.File != null)
            .WithMessage("File size must not exceed 5MB");

        RuleFor(x => x.File)
            .Must(file => file != null && IsValidFileType(file!.FileName))
            .When(x => x.File != null)
            .WithMessage("Invalid file type. Allowed types: jpg, jpeg, png, gif, svg, webp");
    }

    private static bool IsValidFileType(string fileName)
    {
        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".svg", ".webp" };
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        return allowedExtensions.Contains(extension);
    }
}

