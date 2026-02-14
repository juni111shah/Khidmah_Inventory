using MediatR;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.AutonomousWarehouse.Models;

namespace Khidmah_Inventory.Application.Features.AutonomousWarehouse.Commands.PlanWorkTasks;

public class PlanWorkTasksCommandHandler : IRequestHandler<PlanWorkTasksCommand, Result<TaskPlanResult>>
{
    private readonly ITaskPlanner _taskPlanner;
    private readonly ICurrentUserService _currentUser;

    public PlanWorkTasksCommandHandler(ITaskPlanner taskPlanner, ICurrentUserService currentUser)
    {
        _taskPlanner = taskPlanner;
        _currentUser = currentUser;
    }

    public async Task<Result<TaskPlanResult>> Handle(PlanWorkTasksCommand request, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId;
        if (!companyId.HasValue)
            return Result<TaskPlanResult>.Failure("Company context is required.");

        var result = await _taskPlanner.PlanFromOrdersAsync(companyId.Value, request.WarehouseId, request.Request, cancellationToken);
        return Result<TaskPlanResult>.Success(result);
    }
}
