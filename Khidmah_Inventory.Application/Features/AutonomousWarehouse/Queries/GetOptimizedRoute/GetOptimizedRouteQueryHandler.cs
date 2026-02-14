using MediatR;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.AutonomousWarehouse.Models;

namespace Khidmah_Inventory.Application.Features.AutonomousWarehouse.Queries.GetOptimizedRoute;

public class GetOptimizedRouteQueryHandler : IRequestHandler<GetOptimizedRouteQuery, Result<OptimizedRouteResult>>
{
    private readonly IRouteOptimizer _routeOptimizer;
    private readonly ICurrentUserService _currentUser;

    public GetOptimizedRouteQueryHandler(IRouteOptimizer routeOptimizer, ICurrentUserService currentUser)
    {
        _routeOptimizer = routeOptimizer;
        _currentUser = currentUser;
    }

    public async Task<Result<OptimizedRouteResult>> Handle(GetOptimizedRouteQuery request, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId;
        if (!companyId.HasValue)
            return Result<OptimizedRouteResult>.Failure("Company context is required.");

        var result = await _routeOptimizer.GetOptimalSequenceAsync(companyId.Value, request.WarehouseId, request.Request, cancellationToken);
        return Result<OptimizedRouteResult>.Success(result);
    }
}
