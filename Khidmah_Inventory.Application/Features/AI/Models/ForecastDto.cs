namespace Khidmah_Inventory.Application.Features.AI.Models;

public class ForecastDto
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ProductSKU { get; set; } = string.Empty;
    public List<ForecastDataPoint> ForecastData { get; set; } = new();
    public decimal? AverageDailyDemand { get; set; }
    public decimal? RecommendedReorderQuantity { get; set; }
    public DateTime? RecommendedReorderDate { get; set; }
    public string Confidence { get; set; } = string.Empty; // High, Medium, Low
    public List<string> Trends { get; set; } = new(); // Increasing, Decreasing, Stable, Seasonal
}

public class ForecastDataPoint
{
    public DateTime Date { get; set; }
    public decimal PredictedDemand { get; set; }
    public decimal? LowerBound { get; set; }
    public decimal? UpperBound { get; set; }
    public decimal? ActualDemand { get; set; } // For comparison after the fact
}

