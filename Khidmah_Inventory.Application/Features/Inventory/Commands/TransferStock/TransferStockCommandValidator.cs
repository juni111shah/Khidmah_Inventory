using FluentValidation;

namespace Khidmah_Inventory.Application.Features.Inventory.Commands.TransferStock;

public class TransferStockCommandValidator : AbstractValidator<TransferStockCommand>
{
    public TransferStockCommandValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty().WithMessage("Product is required.");

        RuleFor(x => x.FromWarehouseId)
            .NotEmpty().WithMessage("Source warehouse is required.");

        RuleFor(x => x.ToWarehouseId)
            .NotEmpty().WithMessage("Destination warehouse is required.")
            .NotEqual(x => x.FromWarehouseId)
            .WithMessage("Source and destination warehouses must be different.");

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be greater than 0.");
    }
}

