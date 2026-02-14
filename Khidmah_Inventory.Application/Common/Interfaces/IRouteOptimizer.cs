using Khidmah_Inventory.Application.Features.AutonomousWarehouse.Models;

namespace Khidmah_Inventory.Application.Common.Interfaces;

/// <summary>
/// Optimizes task sequence for routing (e.g. nearest neighbor). Replaceable with advanced AI later.
/// </summary>
public interface IRouteOptimizer
{
    /// <summary>
    /// Returns an optimal sequence of tasks from current position. Input: current (x,y) or mapBinId, and task ids or task locations.
    /// </summary>
    Task<OptimizedRouteResult> GetOptimalSequenceAsync(
        Guid companyId,
        Guid warehouseId,
        RouteOptimizerRequest request,
        CancellationToken cancellationToken = default);
}
