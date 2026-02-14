namespace Khidmah_Inventory.Application.Features.Intelligence.Models;

/// <summary>Upsell suggestion, stock warning, bundle ideas for POS.</summary>
public class PosHintsDto
{
    public List<UpsellSuggestionDto> UpsellSuggestions { get; set; } = new();
    public List<StockWarningDto> StockWarnings { get; set; } = new();
    public List<BundleSuggestionDto> BundleIdeas { get; set; } = new();
}

public class UpsellSuggestionDto
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty; // e.g. "Often bought with current cart"
    public decimal SalePrice { get; set; }
}

public class StockWarningDto
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal CurrentStock { get; set; }
    public string Severity { get; set; } = string.Empty; // Low, Critical
}

public class BundleSuggestionDto
{
    public string Title { get; set; } = string.Empty;
    public List<Guid> ProductIds { get; set; } = new();
    public decimal? BundlePrice { get; set; }
    public string Reason { get; set; } = string.Empty;
}
