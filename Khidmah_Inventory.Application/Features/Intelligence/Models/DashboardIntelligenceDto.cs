namespace Khidmah_Inventory.Application.Features.Intelligence.Models;

/// <summary>Predictions, anomalies, risks, opportunities for dashboard.</summary>
public class DashboardIntelligenceDto
{
    public List<PredictionDto> Predictions { get; set; } = new();
    public List<AnomalyDto> Anomalies { get; set; } = new();
    public List<RiskDto> Risks { get; set; } = new();
    public List<OpportunityDto> Opportunities { get; set; } = new();
}

public class PredictionDto
{
    public string Label { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string Trend { get; set; } = string.Empty; // Up, Down, Stable
    public string Period { get; set; } = string.Empty; // e.g. "Next 7 days"
}

public class AnomalyDto
{
    public string Metric { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty; // Low, Medium, High
    public DateTime DetectedAt { get; set; }
    public string? DrillDownRoute { get; set; }
}

public class RiskDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty;
    public string? ActionRoute { get; set; }
    public string? EntityId { get; set; }
}

public class OpportunityDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? ActionRoute { get; set; }
}
