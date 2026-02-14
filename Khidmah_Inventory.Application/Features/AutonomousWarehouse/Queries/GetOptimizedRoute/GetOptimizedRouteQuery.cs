using MediatR;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.AutonomousWarehouse.Models;

namespace Khidmah_Inventory.Application.Features.AutonomousWarehouse.Queries.GetOptimizedRoute;

public class GetOptimizedRouteQuery : IRequest<Result<OptimizedRouteResult>>
{
    public Guid WarehouseId { get; set; }
    public RouteOptimizerRequest Request { get; set; } = new();
}
