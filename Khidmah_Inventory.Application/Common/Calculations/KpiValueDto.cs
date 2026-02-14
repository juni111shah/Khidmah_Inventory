namespace Khidmah_Inventory.Application.Common.Calculations;

/// <summary>
/// Single KPI with current, previous, change and trend for reuse across dashboards and exports.
/// </summary>
public class KpiValueDto
{
    public string Key { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public decimal CurrentValue { get; set; }
    public decimal? PreviousValue { get; set; }
    public decimal? PercentageChange { get; set; }
    public string TrendIndicator { get; set; } = "neutral"; // up, down, neutral
    public string? Unit { get; set; }
    public string? Format { get; set; } // e.g. "currency", "percent", "number"
}
