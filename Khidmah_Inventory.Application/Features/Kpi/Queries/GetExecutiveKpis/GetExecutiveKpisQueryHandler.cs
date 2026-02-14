using MediatR;
using Khidmah_Inventory.Application.Common.Calculations;
using Khidmah_Inventory.Application.Common.Calculations.Dto;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;

namespace Khidmah_Inventory.Application.Features.Kpi.Queries.GetExecutiveKpis;

public class GetExecutiveKpisQueryHandler : IRequestHandler<GetExecutiveKpisQuery, Result<ExecutiveKpisDto>>
{
    private readonly IKpiCalculator _kpiCalculator;
    private readonly ICurrentUserService _currentUser;

    public GetExecutiveKpisQueryHandler(IKpiCalculator kpiCalculator, ICurrentUserService currentUser)
    {
        _kpiCalculator = kpiCalculator;
        _currentUser = currentUser;
    }

    public async Task<Result<ExecutiveKpisDto>> Handle(GetExecutiveKpisQuery request, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId;
        if (!companyId.HasValue)
            return Result<ExecutiveKpisDto>.Failure("Company context is required");

        var context = new CalculationContext
        {
            CompanyId = companyId.Value,
            DateFrom = request.DateFrom,
            DateTo = request.DateTo,
            WarehouseId = request.WarehouseId,
            ProductId = request.ProductId,
            CategoryId = request.CategoryId,
            DeadStockDays = request.DeadStockDays
        };

        var dto = await _kpiCalculator.GetExecutiveKpisAsync(context, cancellationToken);
        return Result<ExecutiveKpisDto>.Success(dto);
    }
}
