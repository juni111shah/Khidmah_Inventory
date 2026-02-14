using MediatR;
using Microsoft.EntityFrameworkCore;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Domain.Enums;

namespace Khidmah_Inventory.Application.Features.AutonomousWarehouse.Commands.CompleteWorkTask;

public class CompleteWorkTaskCommandHandler : IRequestHandler<CompleteWorkTaskCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public CompleteWorkTaskCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(CompleteWorkTaskCommand request, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId;
        if (!companyId.HasValue)
            return Result.Failure("Company context is required.");

        var task = await _context.WorkTasks
            .FirstOrDefaultAsync(t => t.Id == request.TaskId && t.CompanyId == companyId.Value && !t.IsDeleted, cancellationToken);
        if (task == null)
            return Result.Failure("Task not found.");
        if (task.Status == WorkTaskStatus.Completed || task.Status == WorkTaskStatus.Cancelled)
            return Result.Failure("Task is already completed or cancelled.");

        task.Complete(request.Notes);
        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
