namespace Khidmah_Inventory.Application.Common.Calculations;

/// <summary>
/// Reusable time-series grouping for charts. Groups by day / week / month / year.
/// </summary>
public interface ITimeSeriesHelper
{
    /// <summary>
    /// Groups data points by the given period and returns labeled points (e.g. for charts).
    /// </summary>
    IReadOnlyList<TimeSeriesPointDto> GroupByPeriod(
        IReadOnlyList<(DateTime Date, decimal Value)> data,
        TimeSeriesPeriod period,
        DateTime? rangeStart = null,
        DateTime? rangeEnd = null);

    /// <summary>
    /// Gets the period start for a given date (e.g. start of month for Month).
    /// </summary>
    DateTime GetPeriodStart(DateTime date, TimeSeriesPeriod period);

    /// <summary>
    /// Gets a display label for the period (e.g. "Jan 2025" for month).
    /// </summary>
    string GetPeriodLabel(DateTime date, TimeSeriesPeriod period);
}
