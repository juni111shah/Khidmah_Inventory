using Khidmah_Inventory.Application.Common.Calculations.Dto;

namespace Khidmah_Inventory.Application.Common.Calculations;

/// <summary>
/// Financial metrics: revenue, COGS, gross profit, margins, purchase spend, etc.
/// </summary>
public interface IFinanceCalculator
{
    Task<decimal> GetRevenueAsync(CalculationContext context, CancellationToken cancellationToken = default);
    Task<decimal> GetCogsAsync(CalculationContext context, CancellationToken cancellationToken = default);
    Task<decimal> GetGrossProfitAsync(CalculationContext context, CancellationToken cancellationToken = default);
    Task<decimal?> GetGrossMarginPercentAsync(CalculationContext context, CancellationToken cancellationToken = default);
    Task<decimal> GetPurchaseSpendAsync(CalculationContext context, CancellationToken cancellationToken = default);
    Task<decimal?> GetAverageOrderValueAsync(CalculationContext context, CancellationToken cancellationToken = default);
    Task<decimal?> GetSalesGrowthPercentAsync(CalculationContext currentPeriod, CalculationContext previousPeriod, CancellationToken cancellationToken = default);
}
