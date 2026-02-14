using MediatR;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Platform.ScheduledReports.Models;

namespace Khidmah_Inventory.Application.Features.Platform.ScheduledReports.Commands.UpdateScheduledReport;

public class UpdateScheduledReportCommand : IRequest<Result<ScheduledReportDto>>
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ReportType { get; set; } = string.Empty;
    public string Frequency { get; set; } = "Daily";
    public string RecipientsJson { get; set; } = "[]";
    public string? CronExpression { get; set; }
    public bool IsActive { get; set; }
}
