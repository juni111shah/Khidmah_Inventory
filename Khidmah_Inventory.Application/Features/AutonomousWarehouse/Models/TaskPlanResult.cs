namespace Khidmah_Inventory.Application.Features.AutonomousWarehouse.Models;

public class TaskPlanResult
{
    public List<WorkTaskDto> CreatedTasks { get; set; } = new();
    public int TotalCount { get; set; }
}
