using MediatR;
using Microsoft.EntityFrameworkCore;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Platform.ScheduledReports.Models;

namespace Khidmah_Inventory.Application.Features.Platform.ScheduledReports.Commands.UpdateScheduledReport;

public class UpdateScheduledReportCommandHandler : IRequestHandler<UpdateScheduledReportCommand, Result<ScheduledReportDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public UpdateScheduledReportCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<ScheduledReportDto>> Handle(UpdateScheduledReportCommand request, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId;
        if (!companyId.HasValue)
            return Result<ScheduledReportDto>.Failure("Company context is required");

        var report = await _context.ScheduledReports
            .FirstOrDefaultAsync(r => r.Id == request.Id && r.CompanyId == companyId.Value && !r.IsDeleted, cancellationToken);
        if (report == null)
            return Result<ScheduledReportDto>.Failure("Scheduled report not found");

        report.Update(request.Name.Trim(), request.ReportType?.Trim() ?? "", request.Frequency?.Trim() ?? "Daily",
            request.RecipientsJson?.Trim() ?? "[]", request.CronExpression?.Trim(), request.IsActive, _currentUser.UserId);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<ScheduledReportDto>.Success(new ScheduledReportDto
        {
            Id = report.Id,
            Name = report.Name,
            ReportType = report.ReportType,
            Frequency = report.Frequency,
            CronExpression = report.CronExpression,
            RecipientsJson = report.RecipientsJson,
            LastRunAt = report.LastRunAt,
            NextRunAt = report.NextRunAt,
            IsActive = report.IsActive,
            CreatedAt = report.CreatedAt
        });
    }
}
