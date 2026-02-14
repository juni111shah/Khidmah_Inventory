using MediatR;
using Khidmah_Inventory.Application.Common.Calculations;
using Khidmah_Inventory.Application.Common.Calculations.Dto;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;

namespace Khidmah_Inventory.Application.Features.Kpi.Queries.GetCustomerKpis;

public class GetCustomerKpisQueryHandler : IRequestHandler<GetCustomerKpisQuery, Result<CustomerKpisDto>>
{
    private readonly IKpiCalculator _kpiCalculator;
    private readonly ICurrentUserService _currentUser;

    public GetCustomerKpisQueryHandler(IKpiCalculator kpiCalculator, ICurrentUserService currentUser)
    {
        _kpiCalculator = kpiCalculator;
        _currentUser = currentUser;
    }

    public async Task<Result<CustomerKpisDto>> Handle(GetCustomerKpisQuery request, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId;
        if (!companyId.HasValue)
            return Result<CustomerKpisDto>.Failure("Company context is required");

        var context = new CalculationContext
        {
            CompanyId = companyId.Value,
            DateFrom = request.DateFrom,
            DateTo = request.DateTo
        };

        var dto = await _kpiCalculator.GetCustomerKpisAsync(context, cancellationToken);
        return Result<CustomerKpisDto>.Success(dto);
    }
}
