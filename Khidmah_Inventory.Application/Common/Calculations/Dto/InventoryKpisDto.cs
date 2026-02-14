using Khidmah_Inventory.Application.Common.Calculations;

namespace Khidmah_Inventory.Application.Common.Calculations.Dto;

public class InventoryKpisDto
{
    public KpiValueDto StockValue { get; set; } = null!;
    public KpiValueDto InventoryTurnover { get; set; } = null!;
    public KpiValueDto DaysOfInventory { get; set; } = null!;
    public KpiValueDto SellThroughRate { get; set; } = null!;
    public KpiValueDto DeadStockCount { get; set; } = null!;
    public StockAgingBucketsDto AgingBuckets { get; set; } = null!;
}

public class StockAgingBucketsDto
{
    public decimal Days0To30 { get; set; }
    public decimal Days30To60 { get; set; }
    public decimal Days60To90 { get; set; }
    public decimal Days90Plus { get; set; }
}
