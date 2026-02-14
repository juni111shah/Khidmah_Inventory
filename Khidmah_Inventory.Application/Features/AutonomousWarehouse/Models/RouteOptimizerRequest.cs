namespace Khidmah_Inventory.Application.Features.AutonomousWarehouse.Models;

public class RouteOptimizerRequest
{
    /// <summary>Current X (e.g. meters). Optional if StartMapBinId set.</summary>
    public decimal? CurrentX { get; set; }
    /// <summary>Current Y.</summary>
    public decimal? CurrentY { get; set; }
    /// <summary>Current bin id (used to get x,y from map).</summary>
    public Guid? StartMapBinId { get; set; }
    /// <summary>Task ids to visit (will use task MapBinId or LocationCode to order).</summary>
    public List<Guid> TaskIds { get; set; } = new();
}
