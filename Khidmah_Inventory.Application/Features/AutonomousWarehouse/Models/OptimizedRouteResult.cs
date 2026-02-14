namespace Khidmah_Inventory.Application.Features.AutonomousWarehouse.Models;

public class OptimizedRouteResult
{
    /// <summary>Task ids in optimal visit order.</summary>
    public List<Guid> OrderedTaskIds { get; set; } = new();
    /// <summary>Estimated total distance (units same as map x,y).</summary>
    public decimal EstimatedTotalDistance { get; set; }
}
