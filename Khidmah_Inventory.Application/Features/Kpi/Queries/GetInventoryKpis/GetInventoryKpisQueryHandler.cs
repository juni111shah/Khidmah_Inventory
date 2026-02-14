using MediatR;
using Khidmah_Inventory.Application.Common.Calculations;
using Khidmah_Inventory.Application.Common.Calculations.Dto;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;

namespace Khidmah_Inventory.Application.Features.Kpi.Queries.GetInventoryKpis;

public class GetInventoryKpisQueryHandler : IRequestHandler<GetInventoryKpisQuery, Result<InventoryKpisDto>>
{
    private readonly IKpiCalculator _kpiCalculator;
    private readonly ICurrentUserService _currentUser;

    public GetInventoryKpisQueryHandler(IKpiCalculator kpiCalculator, ICurrentUserService currentUser)
    {
        _kpiCalculator = kpiCalculator;
        _currentUser = currentUser;
    }

    public async Task<Result<InventoryKpisDto>> Handle(GetInventoryKpisQuery request, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId;
        if (!companyId.HasValue)
            return Result<InventoryKpisDto>.Failure("Company context is required");

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

        var dto = await _kpiCalculator.GetInventoryKpisAsync(context, cancellationToken);
        return Result<InventoryKpisDto>.Success(dto);
    }
}
