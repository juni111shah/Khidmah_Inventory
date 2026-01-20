namespace Khidmah_Inventory.Application.Common.Interfaces;

public class ForecastResult
{
    public DateTime Date { get; set; }
    public float PredictedValue { get; set; }
    public float LowerBound { get; set; }
    public float UpperBound { get; set; }
}

public interface IMachineLearningService
{
    Task<List<ForecastResult>> ForecastDemandAsync(Guid productId, int forecastDays);
}

