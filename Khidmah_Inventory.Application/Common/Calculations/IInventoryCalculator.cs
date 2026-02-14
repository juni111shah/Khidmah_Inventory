using Khidmah_Inventory.Application.Common.Calculations.Dto;

namespace Khidmah_Inventory.Application.Common.Calculations;

/// <summary>
/// Inventory-specific calculations: stock value, turnover, aging, dead stock, sell-through.
/// </summary>
public interface IInventoryCalculator
{
    Task<decimal> GetStockValueAsync(CalculationContext context, CancellationToken cancellationToken = default);
    Task<decimal?> GetInventoryTurnoverAsync(CalculationContext context, CancellationToken cancellationToken = default);
    Task<decimal?> GetDaysOfInventoryAsync(CalculationContext context, CancellationToken cancellationToken = default);
    Task<decimal?> GetSellThroughRateAsync(CalculationContext context, CancellationToken cancellationToken = default);
    Task<int> GetDeadStockCountAsync(CalculationContext context, CancellationToken cancellationToken = default);
    Task<StockAgingBucketsDto> GetStockAgingBucketsAsync(CalculationContext context, CancellationToken cancellationToken = default);
}
