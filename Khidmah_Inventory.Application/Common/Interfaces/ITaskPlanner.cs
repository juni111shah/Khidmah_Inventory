using Khidmah_Inventory.Application.Features.AutonomousWarehouse.Models;

namespace Khidmah_Inventory.Application.Common.Interfaces;

/// <summary>
/// Plans work tasks: break orders into tasks, prioritize, and assign to agents.
/// </summary>
public interface ITaskPlanner
{
    /// <summary>
    /// Breaks one or more orders into work tasks (pick/putaway/transfer).
    /// </summary>
    Task<TaskPlanResult> PlanFromOrdersAsync(Guid companyId, Guid warehouseId, OrderTaskRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Prioritizes a set of task ids (e.g. by due time, priority field, location).
    /// </summary>
    Task<List<WorkTaskDto>> PrioritizeAsync(Guid companyId, IEnumerable<Guid> taskIds, CancellationToken cancellationToken = default);

    /// <summary>
    /// Assigns tasks to available agents (nearest free worker or robot). Caller can pass task ids and optional agent filter.
    /// </summary>
    Task<AssignResult> AssignToAgentsAsync(Guid companyId, Guid warehouseId, IReadOnlyList<Guid> taskIds, CancellationToken cancellationToken = default);
}
