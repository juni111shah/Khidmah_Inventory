namespace Khidmah_Inventory.Application.Features.Platform.ScheduledReports.Models;

public class ScheduledReportDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ReportType { get; set; } = string.Empty;
    public string Frequency { get; set; } = string.Empty;
    public string? CronExpression { get; set; }
    public string RecipientsJson { get; set; } = "[]";
    public DateTime? LastRunAt { get; set; }
    public DateTime? NextRunAt { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}
