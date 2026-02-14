using Khidmah_Inventory.Domain.Common;

namespace Khidmah_Inventory.Domain.Entities;

/// <summary>
/// Scheduled report: type, frequency, recipients; sent automatically.
/// </summary>
public class ScheduledReport : BaseEntity
{
    public string Name { get; private set; } = string.Empty;
    /// <summary>e.g. SalesSummary, Inventory, LowStock.</summary>
    public string ReportType { get; private set; } = string.Empty;
    /// <summary>Daily, Weekly, Monthly.</summary>
    public string Frequency { get; private set; } = string.Empty;
    /// <summary>Cron expression or null if using Frequency.</summary>
    public string? CronExpression { get; private set; }
    /// <summary>JSON array of email addresses.</summary>
    public string RecipientsJson { get; private set; } = "[]";
    public DateTime? LastRunAt { get; private set; }
    public DateTime? NextRunAt { get; private set; }
    public bool IsActive { get; private set; } = true;
    public string? Format { get; private set; } = "PDF";

    private ScheduledReport() { }

    public ScheduledReport(Guid companyId, string name, string reportType, string frequency, string recipientsJson, string? cronExpression = null, Guid? createdBy = null)
        : base(companyId, createdBy)
    {
        Name = name;
        ReportType = reportType;
        Frequency = frequency;
        RecipientsJson = recipientsJson;
        CronExpression = cronExpression;
    }

    public void SetNextRun(DateTime? nextRun, Guid? updatedBy = null)
    {
        NextRunAt = nextRun;
        UpdateAuditInfo(updatedBy);
    }

    public void RecordRun(DateTime runAt, Guid? updatedBy = null)
    {
        LastRunAt = runAt;
        UpdateAuditInfo(updatedBy);
    }

    public void SetActive(bool active, Guid? updatedBy = null)
    {
        IsActive = active;
        UpdateAuditInfo(updatedBy);
    }

    public void Update(string name, string reportType, string frequency, string recipientsJson, string? cronExpression, bool isActive, Guid? updatedBy = null)
    {
        Name = name;
        ReportType = reportType;
        Frequency = frequency;
        RecipientsJson = recipientsJson;
        CronExpression = cronExpression;
        IsActive = isActive;
        UpdateAuditInfo(updatedBy);
    }
}
