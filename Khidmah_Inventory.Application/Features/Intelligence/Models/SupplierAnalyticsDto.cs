namespace Khidmah_Inventory.Application.Features.Intelligence.Models;

/// <summary>On-time score, lead time, price change trend per supplier.</summary>
public class SupplierAnalyticsDto
{
    public List<SupplierAnalyticsItemDto> Suppliers { get; set; } = new();
}

public class SupplierAnalyticsItemDto
{
    public Guid SupplierId { get; set; }
    public string SupplierName { get; set; } = string.Empty;
    public decimal OnTimeDeliveryPercent { get; set; }
    public int TotalOrders { get; set; }
    public int OnTimeOrders { get; set; }
    public double? AverageLeadTimeDays { get; set; }
    public string PriceTrend { get; set; } = string.Empty; // Up, Down, Stable
    public decimal? LastPriceChangePercent { get; set; }
}
