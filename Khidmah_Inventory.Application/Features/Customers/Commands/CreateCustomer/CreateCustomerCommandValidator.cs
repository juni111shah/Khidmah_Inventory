using FluentValidation;

namespace Khidmah_Inventory.Application.Features.Customers.Commands.CreateCustomer;

public class CreateCustomerCommandValidator : AbstractValidator<CreateCustomerCommand>
{
    public CreateCustomerCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Customer name is required.")
            .MaximumLength(200).WithMessage("Customer name cannot exceed 200 characters.");
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

