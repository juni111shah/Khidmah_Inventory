using MediatR;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Platform.ScheduledReports.Models;
using Khidmah_Inventory.Domain.Entities;

namespace Khidmah_Inventory.Application.Features.Platform.ScheduledReports.Commands.CreateScheduledReport;

public class CreateScheduledReportCommandHandler : IRequestHandler<CreateScheduledReportCommand, Result<ScheduledReportDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public CreateScheduledReportCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<ScheduledReportDto>> Handle(CreateScheduledReportCommand request, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId;
        if (!companyId.HasValue)
            return Result<ScheduledReportDto>.Failure("Company context is required");

        var report = new ScheduledReport(
            companyId.Value,
            request.Name.Trim(),
            request.ReportType?.Trim() ?? "",
            request.Frequency?.Trim() ?? "Daily",
            request.RecipientsJson?.Trim() ?? "[]",
            request.CronExpression?.Trim(),
            _currentUser.UserId);
        report.SetNextRun(DateTime.UtcNow.AddDays(1), _currentUser.UserId);
        _context.ScheduledReports.Add(report);
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
