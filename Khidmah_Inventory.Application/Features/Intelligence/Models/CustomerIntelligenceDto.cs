namespace Khidmah_Inventory.Application.Features.Intelligence.Models;

/// <summary>LTV, buying frequency, churn risk, preferred items.</summary>
public class CustomerIntelligenceDto
{
    public List<CustomerIntelligenceItemDto> Customers { get; set; } = new();
}

public class CustomerIntelligenceItemDto
{
    public Guid CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public decimal LifetimeValue { get; set; }
    public decimal OrdersPerMonth { get; set; }
    public int TotalOrders { get; set; }
    public string ChurnRisk { get; set; } = string.Empty; // Low, Medium, High
    public int? DaysSinceLastOrder { get; set; }
    public List<string> PreferredCategoryNames { get; set; } = new();
    public List<CustomerTopProductDto> TopProductsBought { get; set; } = new();
}

public class CustomerTopProductDto
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal QuantityBought { get; set; }
}
