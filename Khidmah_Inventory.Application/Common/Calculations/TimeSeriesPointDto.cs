namespace Khidmah_Inventory.Application.Common.Calculations;

/// <summary>
/// Single point in a time series (e.g. revenue by month). Reusable for charts and exports.
/// </summary>
public class TimeSeriesPointDto
{
    public DateTime PeriodStart { get; set; }
    public string PeriodLabel { get; set; } = string.Empty;
    public decimal Value { get; set; }
    public string? GroupKey { get; set; }
}
