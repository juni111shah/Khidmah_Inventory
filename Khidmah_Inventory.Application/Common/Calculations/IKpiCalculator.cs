using Khidmah_Inventory.Application.Common.Calculations.Dto;

namespace Khidmah_Inventory.Application.Common.Calculations;

/// <summary>
/// Reusable KPI calculation engine for executive and operational metrics.
/// </summary>
public interface IKpiCalculator
{
    /// <summary>
    /// Executive KPIs: revenue, profit, low stock count, pending approvals, top products, dead inventory.
    /// </summary>
    Task<ExecutiveKpisDto> GetExecutiveKpisAsync(CalculationContext context, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sales KPIs: revenue, COGS, gross profit, margin %, AOV, sales growth.
    /// </summary>
    Task<SalesKpisDto> GetSalesKpisAsync(CalculationContext context, CancellationToken cancellationToken = default);

    /// <summary>
    /// Inventory KPIs: stock value, turnover, days of inventory, sell-through, dead stock, aging buckets.
    /// </summary>
    Task<InventoryKpisDto> GetInventoryKpisAsync(CalculationContext context, CancellationToken cancellationToken = default);

    /// <summary>
    /// Customer KPIs: CLV, repeat rate, customer count.
    /// </summary>
    Task<CustomerKpisDto> GetCustomerKpisAsync(CalculationContext context, CancellationToken cancellationToken = default);
}
