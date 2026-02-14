using MediatR;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.AutonomousWarehouse.Models;

namespace Khidmah_Inventory.Application.Features.AutonomousWarehouse.Commands.AssignWorkTasks;

public class AssignWorkTasksCommandHandler : IRequestHandler<AssignWorkTasksCommand, Result<AssignResult>>
{
    private readonly ITaskPlanner _taskPlanner;
    private readonly ICurrentUserService _currentUser;

    public AssignWorkTasksCommandHandler(ITaskPlanner taskPlanner, ICurrentUserService currentUser)
    {
        _taskPlanner = taskPlanner;
        _currentUser = currentUser;
    }

    public async Task<Result<AssignResult>> Handle(AssignWorkTasksCommand request, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId;
        if (!companyId.HasValue)
            return Result<AssignResult>.Failure("Company context is required.");

        var result = await _taskPlanner.AssignToAgentsAsync(companyId.Value, request.WarehouseId, request.TaskIds, cancellationToken);
        return Result<AssignResult>.Success(result);
    }
}
