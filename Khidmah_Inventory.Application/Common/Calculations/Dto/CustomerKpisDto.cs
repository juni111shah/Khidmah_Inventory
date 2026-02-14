using Khidmah_Inventory.Application.Common.Calculations;

namespace Khidmah_Inventory.Application.Common.Calculations.Dto;

public class CustomerKpisDto
{
    public KpiValueDto CustomerCount { get; set; } = null!;
    public KpiValueDto RepeatRatePercent { get; set; } = null!;
    public KpiValueDto AverageLifetimeValue { get; set; } = null!;
}
