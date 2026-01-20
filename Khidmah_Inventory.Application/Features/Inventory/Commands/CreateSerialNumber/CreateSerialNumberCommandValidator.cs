using FluentValidation;

namespace Khidmah_Inventory.Application.Features.Inventory.Commands.CreateSerialNumber;

public class CreateSerialNumberCommandValidator : AbstractValidator<CreateSerialNumberCommand>
{
    public CreateSerialNumberCommandValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty().WithMessage("Product is required.");

        RuleFor(x => x.WarehouseId)
            .NotEmpty().WithMessage("Warehouse is required.");

        RuleFor(x => x.SerialNumberValue)
            .NotEmpty().WithMessage("Serial number is required.")
            .MaximumLength(100).WithMessage("Serial number must not exceed 100 characters.");

        RuleFor(x => x.BatchNumber)
            .MaximumLength(100).WithMessage("Batch number must not exceed 100 characters.");

        RuleFor(x => x.WarrantyExpiryDate)
            .MaximumLength(50).WithMessage("Warranty expiry date must not exceed 50 characters.");

        RuleFor(x => x.Notes)
            .MaximumLength(1000).WithMessage("Notes must not exceed 1000 characters.");

        RuleFor(x => x.ExpiryDate)
            .GreaterThan(x => x.ManufactureDate)
            .When(x => x.ManufactureDate.HasValue && x.ExpiryDate.HasValue)
            .WithMessage("Expiry date must be after manufacture date.");
    }
}

