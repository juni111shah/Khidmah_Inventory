using MediatR;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.HandsFree.Models;

namespace Khidmah_Inventory.Application.Features.HandsFree.Queries.GetHandsFreeTasks;

public class GetHandsFreeTasksQuery : IRequest<Result<HandsFreeTasksResult>>
{
    public Guid WarehouseId { get; set; }
    /// <summary>Max tasks to return (default 50). Built from stock levels with available qty.</summary>
    public int MaxTasks { get; set; } = 50;
}
