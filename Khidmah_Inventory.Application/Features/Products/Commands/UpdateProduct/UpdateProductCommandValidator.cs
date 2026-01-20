using FluentValidation;

namespace Khidmah_Inventory.Application.Features.Products.Commands.UpdateProduct;

public class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
{
    public UpdateProductCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Product ID is required.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Product name is required.")
            .MaximumLength(200).WithMessage("Product name cannot exceed 200 characters.");

        RuleFor(x => x.SKU)
            .NotEmpty().WithMessage("SKU is required.")
            .MaximumLength(100).WithMessage("SKU cannot exceed 100 characters.");

        RuleFor(x => x.UnitOfMeasureId)
            .NotEmpty().WithMessage("Unit of measure is required.");

        RuleFor(x => x.PurchasePrice)
            .GreaterThanOrEqualTo(0).WithMessage("Purchase price must be greater than or equal to 0.");

        RuleFor(x => x.SalePrice)
            .GreaterThanOrEqualTo(0).WithMessage("Sale price must be greater than or equal to 0.");

        RuleFor(x => x.Barcode)
            .MaximumLength(100).WithMessage("Barcode cannot exceed 100 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Description cannot exceed 1000 characters.");

        RuleFor(x => x.MinStockLevel)
            .GreaterThanOrEqualTo(0).When(x => x.MinStockLevel.HasValue)
            .WithMessage("Minimum stock level must be greater than or equal to 0.");

        RuleFor(x => x.MaxStockLevel)
            .GreaterThan(x => x.MinStockLevel).When(x => x.MaxStockLevel.HasValue && x.MinStockLevel.HasValue)
            .WithMessage("Maximum stock level must be greater than minimum stock level.");

        RuleFor(x => x.ReorderPoint)
            .GreaterThanOrEqualTo(0).When(x => x.ReorderPoint.HasValue)
            .WithMessage("Reorder point must be greater than or equal to 0.");
    }
}

