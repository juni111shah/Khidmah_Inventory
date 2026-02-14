namespace Khidmah_Inventory.Application.Common.Interfaces;

/// <summary>
/// Evaluates automation rules when triggers fire and executes actions (create notification, create PO draft, send webhook).
/// </summary>
public interface IAutomationExecutor
{
    /// <summary>
    /// Trigger: StockBelowThreshold. Context: ProductId, WarehouseId, CurrentQuantity, MinStockLevel.
    /// </summary>
    Task ExecuteStockBelowThresholdAsync(Guid companyId, Guid productId, Guid? warehouseId, decimal currentQuantity, decimal minStockLevel, CancellationToken cancellationToken = default);

    /// <summary>
    /// Trigger: POApproved. Context: PurchaseOrderId.
    /// </summary>
    Task ExecutePOApprovedAsync(Guid companyId, Guid purchaseOrderId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Trigger: SaleCreated. Context: SalesOrderId.
    /// </summary>
    Task ExecuteSaleCreatedAsync(Guid companyId, Guid salesOrderId, CancellationToken cancellationToken = default);
}
