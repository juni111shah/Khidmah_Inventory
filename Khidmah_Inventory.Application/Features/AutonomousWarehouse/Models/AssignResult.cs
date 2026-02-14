namespace Khidmah_Inventory.Application.Features.AutonomousWarehouse.Models;

public class AssignResult
{
    public int AssignedCount { get; set; }
    public List<Guid> AssignedTaskIds { get; set; } = new();
    public List<string> Errors { get; set; } = new();
}
