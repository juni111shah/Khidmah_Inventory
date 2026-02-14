using Khidmah_Inventory.Application.Common.Calculations;

namespace Khidmah_Inventory.Application.Common.Calculations.Dto;

public class SalesKpisDto
{
    public KpiValueDto Revenue { get; set; } = null!;
    public KpiValueDto Cogs { get; set; } = null!;
    public KpiValueDto GrossProfit { get; set; } = null!;
    public KpiValueDto GrossMarginPercent { get; set; } = null!;
    public KpiValueDto AverageOrderValue { get; set; } = null!;
    public KpiValueDto OrderCount { get; set; } = null!;
    public KpiValueDto SalesGrowthPercent { get; set; } = null!;
}
