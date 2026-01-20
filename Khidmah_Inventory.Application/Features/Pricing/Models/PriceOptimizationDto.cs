namespace Khidmah_Inventory.Application.Features.Pricing.Models;

public class PriceOptimizationDto
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ProductSKU { get; set; } = string.Empty;
    public decimal CurrentPrice { get; set; }
    public decimal? CompetitorPrice { get; set; }
    public decimal? RecommendedPrice { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public decimal CurrentMargin { get; set; }
    public decimal? RecommendedMargin { get; set; }
    public decimal? OptimalMargin { get; set; }
    public string Recommendation { get; set; } = string.Empty; // Increase, Decrease, Maintain
    public decimal? PriceChangeAmount { get; set; }
    public decimal? PriceChangePercentage { get; set; }
    public List<PriceHistoryDto> PriceHistory { get; set; } = new();
}

public class PriceHistoryDto
{
    public DateTime Date { get; set; }
    public decimal Price { get; set; }
    public string Type { get; set; } = string.Empty; // Purchase, Sale
}

