namespace Khidmah_Inventory.Application.Features.Intelligence.Models;

public class AiRecommendationDto
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ProductSKU { get; set; } = string.Empty;
    public string RiskLevel { get; set; } = "Low";
    public decimal SuggestedReorderQuantity { get; set; }
    public decimal SuggestedSalePrice { get; set; }
    public string RecommendedSupplierName { get; set; } = "N/A";
    public decimal StockoutProbability { get; set; }
    public bool AbnormalSalesDetected { get; set; }
    public string Reasoning { get; set; } = string.Empty;
}
