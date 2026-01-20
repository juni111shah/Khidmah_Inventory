using FluentValidation;

namespace Khidmah_Inventory.Application.Features.SalesOrders.Commands.CreateSalesOrder;

public class CreateSalesOrderCommandValidator : AbstractValidator<CreateSalesOrderCommand>
{
    public CreateSalesOrderCommandValidator()
    {
        RuleFor(x => x.CustomerId)
            .NotEmpty().WithMessage("Customer is required.");
        RuleFor(x => x.OrderDate)
            .NotEmpty().WithMessage("Order date is required.");
        RuleFor(x => x.Items)
            .NotEmpty().WithMessage("At least one item is required.")
            .Must(items => items.Count > 0).WithMessage("At least one item is required.");
        RuleForEach(x => x.Items)
            .SetValidator(new CreateSalesOrderItemDtoValidator());
    }
}

public class CreateSalesOrderItemDtoValidator : AbstractValidator<CreateSalesOrderItemDto>
{
    public CreateSalesOrderItemDtoValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty().WithMessage("Product is required.");
        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be greater than 0.");
        RuleFor(x => x.UnitPrice)
            .GreaterThanOrEqualTo(0).WithMessage("Unit price must be greater than or equal to 0.");
        RuleFor(x => x.DiscountPercent)
            .InclusiveBetween(0, 100).WithMessage("Discount percent must be between 0 and 100.");
        RuleFor(x => x.TaxPercent)
            .InclusiveBetween(0, 100).WithMessage("Tax percent must be between 0 and 100.");
    }
}

