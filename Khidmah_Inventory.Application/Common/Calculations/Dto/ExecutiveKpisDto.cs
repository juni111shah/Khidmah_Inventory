using Khidmah_Inventory.Application.Common.Calculations;

namespace Khidmah_Inventory.Application.Common.Calculations.Dto;

public class ExecutiveKpisDto
{
    public KpiValueDto RevenueToday { get; set; } = null!;
    public KpiValueDto ProfitToday { get; set; } = null!;
    public KpiValueDto LowStockCount { get; set; } = null!;
    public KpiValueDto PendingApprovals { get; set; } = null!;
    public List<TopProductKpiDto> TopProducts { get; set; } = new();
    public KpiValueDto DeadInventoryCount { get; set; } = null!;
}

public class TopProductKpiDto
{
    public Guid ProductId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? SKU { get; set; }
    public decimal Revenue { get; set; }
    public decimal QuantitySold { get; set; }
}
