using MediatR;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.AutonomousWarehouse.Models;

namespace Khidmah_Inventory.Application.Features.AutonomousWarehouse.Commands.AssignWorkTasks;

public class AssignWorkTasksCommand : IRequest<Result<AssignResult>>
{
    public Guid WarehouseId { get; set; }
    public List<Guid> TaskIds { get; set; } = new();
}
