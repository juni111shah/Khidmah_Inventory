using MediatR;
using Microsoft.EntityFrameworkCore;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;

namespace Khidmah_Inventory.Application.Features.Platform.ScheduledReports.Commands.DeleteScheduledReport;

public class DeleteScheduledReportCommandHandler : IRequestHandler<DeleteScheduledReportCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public DeleteScheduledReportCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(DeleteScheduledReportCommand request, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId;
        if (!companyId.HasValue)
            return Result.Failure("Company context is required");

        var report = await _context.ScheduledReports
            .FirstOrDefaultAsync(r => r.Id == request.Id && r.CompanyId == companyId.Value && !r.IsDeleted, cancellationToken);
        if (report == null)
            return Result.Failure("Scheduled report not found");

        report.MarkAsDeleted(_currentUser.UserId);
        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
