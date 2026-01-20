using FluentValidation;

namespace Khidmah_Inventory.Application.Features.Inventory.Commands.CreateStockTransaction;

public class CreateStockTransactionCommandValidator : AbstractValidator<CreateStockTransactionCommand>
{
    public CreateStockTransactionCommandValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty().WithMessage("Product is required.");

        RuleFor(x => x.WarehouseId)
            .NotEmpty().WithMessage("Warehouse is required.");

        RuleFor(x => x.TransactionType)
            .NotEmpty().WithMessage("Transaction type is required.")
            .Must(t => new[] { "StockIn", "StockOut", "Adjustment", "Transfer" }.Contains(t))
            .WithMessage("Invalid transaction type. Must be StockIn, StockOut, Adjustment, or Transfer.");

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be greater than 0.");

        RuleFor(x => x.UnitCost)
            .GreaterThanOrEqualTo(0).When(x => x.UnitCost.HasValue)
            .WithMessage("Unit cost must be greater than or equal to 0.");
    }
}

