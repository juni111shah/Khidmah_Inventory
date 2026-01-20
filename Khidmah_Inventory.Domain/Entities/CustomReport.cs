using Khidmah_Inventory.Domain.Common;

namespace Khidmah_Inventory.Domain.Entities;

public class CustomReport : Entity
{
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public string ReportType { get; private set; } = string.Empty; // Sales, Inventory, Purchase, etc.
    public string ReportDefinition { get; private set; } = string.Empty; // JSON string defining report structure
    public bool IsPublic { get; private set; } = false; // Can be shared with other users
    public Guid? CreatedByUserId { get; private set; }

    // Navigation properties
    public virtual User? CreatedByUser { get; private set; }

    private CustomReport() { }

    public CustomReport(
        Guid companyId,
        string name,
        string reportType,
        string reportDefinition,
        Guid? createdBy = null) : base(companyId, createdBy)
    {
        Name = name;
        ReportType = reportType;
        ReportDefinition = reportDefinition;
        CreatedByUserId = createdBy;
    }

    public void Update(string name, string description, string reportDefinition, bool isPublic, Guid? updatedBy = null)
    {
        Name = name;
        Description = description;
        ReportDefinition = reportDefinition;
        IsPublic = isPublic;
        UpdateAuditInfo(updatedBy);
    }
}

