using FluentValidation;

namespace Khidmah_Inventory.Application.Features.Reports.Queries.GetSalesReport;

public class GetSalesReportQueryValidator : AbstractValidator<GetSalesReportQuery>
{
    public GetSalesReportQueryValidator()
    {
        RuleFor(x => x.FromDate)
            .NotEmpty().WithMessage("From date is required.")
            .LessThanOrEqualTo(x => x.ToDate).WithMessage("From date must be less than or equal to To date.");

        RuleFor(x => x.ToDate)
            .NotEmpty().WithMessage("To date is required.")
            .GreaterThanOrEqualTo(x => x.FromDate).WithMessage("To date must be greater than or equal to From date.")
            .LessThanOrEqualTo(DateTime.UtcNow.AddDays(1)).WithMessage("To date cannot be in the future.");
    }
}