namespace Khidmah_Inventory.Application.Features.Intelligence.Models;

/// <summary>Intelligence for product detail: velocity, days remaining, reorder risk, margin trend, ABC, actions.</summary>
public class ProductIntelligenceDto
{
    public Guid ProductId { get; set; }
    /// <summary>Units sold per day (e.g. last 30 days average).</summary>
    public decimal SalesVelocity { get; set; }
    /// <summary>Estimated days until stock runs out at current velocity.</summary>
    public int? StockDaysRemaining { get; set; }
    /// <summary>Low, Medium, High, Critical.</summary>
    public string ReorderRisk { get; set; } = string.Empty;
    /// <summary>Trend: Up, Down, Stable (vs previous period).</summary>
    public string MarginTrend { get; set; } = string.Empty;
    public decimal CurrentMarginPercent { get; set; }
    public decimal? PreviousMarginPercent { get; set; }
    /// <summary>Last N price points (e.g. from sales/purchase history).</summary>
    public List<PriceHistoryPointDto> PriceHistory { get; set; } = new();
    /// <summary>Forecast quantity (simple projection) vs actual in last period.</summary>
    public decimal? ForecastVsActualVariance { get; set; }
    /// <summary>A, B, or C based on revenue contribution.</summary>
    public string AbcClassification { get; set; } = string.Empty;
    public decimal AbcRevenueSharePercent { get; set; }
    /// <summary>Recommended actions (e.g. Reorder, Review price, Promote).</summary>
    public List<string> RecommendedActions { get; set; } = new();
}

public class PriceHistoryPointDto
{
    public DateTime Date { get; set; }
    public decimal SalePrice { get; set; }
    public decimal? CostPrice { get; set; }
}
