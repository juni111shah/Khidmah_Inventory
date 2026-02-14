namespace Khidmah_Inventory.Application.Common.Calculations;

/// <summary>
/// Flexible costing strategy for COGS and inventory valuation.
/// Implementations: LastPurchasePrice, AverageCost. Later: FIFO, LIFO.
/// </summary>
public interface ICostingStrategy
{
    string Name { get; }

    /// <summary>
    /// Get unit cost for a product at a point in time (optional warehouse).
    /// Used for COGS and stock value calculations.
    /// </summary>
    Task<decimal?> GetUnitCostAsync(
        Guid companyId,
        Guid productId,
        Guid? warehouseId,
        DateTime? asOfDate,
        CancellationToken cancellationToken = default);
}
