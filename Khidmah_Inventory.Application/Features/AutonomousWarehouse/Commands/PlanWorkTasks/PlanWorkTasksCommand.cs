using MediatR;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.AutonomousWarehouse.Models;

namespace Khidmah_Inventory.Application.Features.AutonomousWarehouse.Commands.PlanWorkTasks;

public class PlanWorkTasksCommand : IRequest<Result<TaskPlanResult>>
{
    public Guid WarehouseId { get; set; }
    public OrderTaskRequest Request { get; set; } = new();
}
