using Microsoft.EntityFrameworkCore;
using Khidmah_Inventory.Application.Common.Interfaces;

namespace Khidmah_Inventory.Infrastructure.Services;

public class MachineLearningService : IMachineLearningService
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public MachineLearningService(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<List<ForecastResult>> ForecastDemandAsync(Guid productId, int forecastDays)
    {
        var companyId = _currentUser.CompanyId;
        if (!companyId.HasValue)
            return new List<ForecastResult>();

        // Get historical sales data (last 90 days)
        var historicalData = await _context.SalesOrderItems
            .Include(item => item.SalesOrder)
            .Where(item => item.ProductId == productId &&
                item.SalesOrder.CompanyId == companyId.Value &&
                !item.SalesOrder.IsDeleted &&
                item.SalesOrder.OrderDate >= DateTime.UtcNow.AddDays(-90))
            .GroupBy(item => item.SalesOrder.OrderDate.Date)
            .Select(g => new
            {
                Date = g.Key,
                Quantity = (float)g.Sum(i => i.Quantity)
            })
            .OrderBy(x => x.Date)
            .ToListAsync();

        if (!historicalData.Any())
        {
            // Return zero forecast if no historical data
            var emptyResults = new List<ForecastResult>();
            for (int i = 1; i <= forecastDays; i++)
            {
                emptyResults.Add(new ForecastResult
                {
                    Date = DateTime.UtcNow.AddDays(i).Date,
                    PredictedValue = 0,
                    LowerBound = 0,
                    UpperBound = 0
                });
            }
            return emptyResults;
        }

        // Calculate statistics for forecasting
        var values = historicalData.Select(x => x.Quantity).ToList();
        var average = values.Average();
        var standardDeviation = CalculateStandardDeviation(values);
        var trend = CalculateTrend(values);

        // Simple moving average with trend
        var lastValue = values.Last();
        var results = new List<ForecastResult>();

        for (int i = 1; i <= forecastDays; i++)
        {
            // Apply trend and add some randomness based on historical variance
            var predicted = lastValue + (trend * i);
            predicted = Math.Max(0, predicted); // Ensure non-negative

            // Calculate confidence bounds (95% confidence interval)
            var margin = standardDeviation * 1.96f;
            var lowerBound = Math.Max(0, predicted - margin);
            var upperBound = predicted + margin;

            results.Add(new ForecastResult
            {
                Date = DateTime.UtcNow.AddDays(i).Date,
                PredictedValue = predicted,
                LowerBound = lowerBound,
                UpperBound = upperBound
            });
        }

        return results;
    }

    private float CalculateStandardDeviation(List<float> values)
    {
        if (values.Count < 2) return 0;

        var average = values.Average();
        var sumOfSquares = values.Sum(x => (x - average) * (x - average));
        return (float)Math.Sqrt(sumOfSquares / values.Count);
    }

    private float CalculateTrend(List<float> values)
    {
        if (values.Count < 2) return 0;

        // Simple linear regression to calculate trend
        var n = values.Count;
        var xValues = Enumerable.Range(1, n).Select(x => (float)x).ToList();
        var xMean = xValues.Average();
        var yMean = values.Average();

        var numerator = xValues.Zip(values, (x, y) => (x - xMean) * (y - yMean)).Sum();
        var denominator = xValues.Sum(x => (x - xMean) * (x - xMean));

        return denominator != 0 ? numerator / denominator : 0;
    }
}

