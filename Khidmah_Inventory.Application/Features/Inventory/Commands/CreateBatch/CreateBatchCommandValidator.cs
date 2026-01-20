using FluentValidation;

namespace Khidmah_Inventory.Application.Features.Inventory.Commands.CreateBatch;

public class CreateBatchCommandValidator : AbstractValidator<CreateBatchCommand>
{
    public CreateBatchCommandValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty().WithMessage("Product is required.");

        RuleFor(x => x.WarehouseId)
            .NotEmpty().WithMessage("Warehouse is required.");

        RuleFor(x => x.BatchNumber)
            .NotEmpty().WithMessage("Batch number is required.")
            .MaximumLength(100).WithMessage("Batch number must not exceed 100 characters.");

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be greater than 0.");

        RuleFor(x => x.LotNumber)
            .MaximumLength(100).WithMessage("Lot number must not exceed 100 characters.");

        RuleFor(x => x.SupplierName)
            .MaximumLength(200).WithMessage("Supplier name must not exceed 200 characters.");

        RuleFor(x => x.SupplierBatchNumber)
            .MaximumLength(100).WithMessage("Supplier batch number must not exceed 100 characters.");

        RuleFor(x => x.Notes)
            .MaximumLength(1000).WithMessage("Notes must not exceed 1000 characters.");

        RuleFor(x => x.ExpiryDate)
            .GreaterThan(x => x.ManufactureDate)
            .When(x => x.ManufactureDate.HasValue && x.ExpiryDate.HasValue)
            .WithMessage("Expiry date must be after manufacture date.");
    }
}

