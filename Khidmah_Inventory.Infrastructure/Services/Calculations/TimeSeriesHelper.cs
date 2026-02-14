using System.Globalization;
using Khidmah_Inventory.Application.Common.Calculations;

namespace Khidmah_Inventory.Infrastructure.Services.Calculations;

public class TimeSeriesHelper : ITimeSeriesHelper
{
    public IReadOnlyList<TimeSeriesPointDto> GroupByPeriod(
        IReadOnlyList<(DateTime Date, decimal Value)> data,
        TimeSeriesPeriod period,
        DateTime? rangeStart = null,
        DateTime? rangeEnd = null)
    {
        var start = rangeStart ?? (data.Count > 0 ? data.Min(x => x.Date) : DateTime.UtcNow.Date);
        var end = rangeEnd ?? (data.Count > 0 ? data.Max(x => x.Date) : DateTime.UtcNow.Date);

        var grouped = data
            .GroupBy(x => GetPeriodStart(x.Date, period))
            .ToDictionary(g => g.Key, g => g.Sum(x => x.Value));

        var points = new List<TimeSeriesPointDto>();
        var current = GetPeriodStart(start, period);
        var endPeriod = GetPeriodStart(end, period);

        while (current <= endPeriod)
        {
            points.Add(new TimeSeriesPointDto
            {
                PeriodStart = current,
                PeriodLabel = GetPeriodLabel(current, period),
                Value = grouped.TryGetValue(current, out var v) ? v : 0
            });
            current = AdvancePeriod(current, period);
        }

        return points;
    }

    public DateTime GetPeriodStart(DateTime date, TimeSeriesPeriod period)
    {
        var d = date.Kind == DateTimeKind.Utc ? date : DateTime.SpecifyKind(date, DateTimeKind.Utc);
        return period switch
        {
            TimeSeriesPeriod.Day => d.Date,
            TimeSeriesPeriod.Week => d.Date.AddDays(-(int)d.DayOfWeek),
            TimeSeriesPeriod.Month => new DateTime(d.Year, d.Month, 1, 0, 0, 0, d.Kind),
            TimeSeriesPeriod.Year => new DateTime(d.Year, 1, 1, 0, 0, 0, d.Kind),
            _ => d.Date
        };
    }

    public string GetPeriodLabel(DateTime date, TimeSeriesPeriod period)
    {
        return period switch
        {
            TimeSeriesPeriod.Day => date.ToString("MMM d", CultureInfo.InvariantCulture),
            TimeSeriesPeriod.Week => $"W{GetWeekNumber(date)} {date.Year}",
            TimeSeriesPeriod.Month => date.ToString("MMM yyyy", CultureInfo.InvariantCulture),
            TimeSeriesPeriod.Year => date.Year.ToString(),
            _ => date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)
        };
    }

    private static DateTime AdvancePeriod(DateTime current, TimeSeriesPeriod period)
    {
        return period switch
        {
            TimeSeriesPeriod.Day => current.AddDays(1),
            TimeSeriesPeriod.Week => current.AddDays(7),
            TimeSeriesPeriod.Month => current.AddMonths(1),
            TimeSeriesPeriod.Year => current.AddYears(1),
            _ => current.AddDays(1)
        };
    }

    private static int GetWeekNumber(DateTime date)
    {
        var ci = CultureInfo.CurrentCulture;
        return ci.Calendar.GetWeekOfYear(date, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
    }
}
