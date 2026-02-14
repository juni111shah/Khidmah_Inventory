using MediatR;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.AutonomousWarehouse.Models;

namespace Khidmah_Inventory.Application.Features.AutonomousWarehouse.Queries.GetWorkTasks;

public class GetWorkTasksQuery : IRequest<Result<List<WorkTaskDto>>>
{
    public Guid WarehouseId { get; set; }
    public Guid? AssignedToId { get; set; }
    public int? Status { get; set; } // WorkTaskStatus enum value
    public int? Type { get; set; }   // WorkTaskType enum value
    public int MaxCount { get; set; } = 100;
}
