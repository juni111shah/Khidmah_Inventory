using FluentValidation;

namespace Khidmah_Inventory.Application.Features.Suppliers.Commands.UpdateSupplier;

public class UpdateSupplierCommandValidator : AbstractValidator<UpdateSupplierCommand>
{
    public UpdateSupplierCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Supplier ID is required.");
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Supplier name is required.")
            .MaximumLength(200).WithMessage("Supplier name cannot exceed 200 characters.");
        RuleFor(x => x.Code)
            .MaximumLength(50).WithMessage("Code cannot exceed 50 characters.");
        RuleFor(x => x.Email)
            .EmailAddress().When(x => !string.IsNullOrWhiteSpace(x.Email))
            .WithMessage("Invalid email address.");
        RuleFor(x => x.CreditLimit)
            .GreaterThanOrEqualTo(0).When(x => x.CreditLimit.HasValue)
            .WithMessage("Credit limit must be greater than or equal to 0.");
    }
}

