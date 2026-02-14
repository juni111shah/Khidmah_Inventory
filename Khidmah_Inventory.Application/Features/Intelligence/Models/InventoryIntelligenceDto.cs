namespace Khidmah_Inventory.Application.Features.Intelligence.Models;

public class InventoryIntelligenceDto
{
    public List<InflowOutflowPointDto> InflowOutflowChart { get; set; } = new();
    public List<AgingBucketDto> AgingBuckets { get; set; } = new();
    public List<DeadStockItemDto> DeadStockItems { get; set; } = new();
    public bool ShrinkageWarning { get; set; }
    public string? ShrinkageWarningMessage { get; set; }
    public decimal CarryingCostEstimate { get; set; }
    public string CarryingCostPeriod { get; set; } = "Monthly";
}

public class InflowOutflowPointDto
{
    public string Label { get; set; } = string.Empty;
    public DateTime PeriodStart { get; set; }
    public decimal InflowQuantity { get; set; }
    public decimal OutflowQuantity { get; set; }
    public decimal InflowValue { get; set; }
    public decimal OutflowValue { get; set; }
}

public class AgingBucketDto
{
    public string Bucket { get; set; } = string.Empty;
    public int ProductCount { get; set; }
    public decimal Quantity { get; set; }
    public decimal Value { get; set; }
}

public class DeadStockItemDto
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ProductSKU { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal Value { get; set; }
    public int DaysSinceLastMovement { get; set; }
}
