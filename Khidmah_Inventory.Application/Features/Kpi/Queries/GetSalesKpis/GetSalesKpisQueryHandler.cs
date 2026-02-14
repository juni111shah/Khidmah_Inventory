using MediatR;
using Khidmah_Inventory.Application.Common.Calculations;
using Khidmah_Inventory.Application.Common.Calculations.Dto;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;

namespace Khidmah_Inventory.Application.Features.Kpi.Queries.GetSalesKpis;

public class GetSalesKpisQueryHandler : IRequestHandler<GetSalesKpisQuery, Result<SalesKpisDto>>
{
    private readonly IKpiCalculator _kpiCalculator;
    private readonly ICurrentUserService _currentUser;

    public GetSalesKpisQueryHandler(IKpiCalculator kpiCalculator, ICurrentUserService currentUser)
    {
        _kpiCalculator = kpiCalculator;
        _currentUser = currentUser;
    }

    public async Task<Result<SalesKpisDto>> Handle(GetSalesKpisQuery request, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId;
        if (!companyId.HasValue)
            return Result<SalesKpisDto>.Failure("Company context is required");

        var context = new CalculationContext
        {
            CompanyId = companyId.Value,
            DateFrom = request.DateFrom,
            DateTo = request.DateTo,
            WarehouseId = request.WarehouseId,
            ProductId = request.ProductId,
            CategoryId = request.CategoryId
        };

        var dto = await _kpiCalculator.GetSalesKpisAsync(context, cancellationToken);
        return Result<SalesKpisDto>.Success(dto);
    }
}
