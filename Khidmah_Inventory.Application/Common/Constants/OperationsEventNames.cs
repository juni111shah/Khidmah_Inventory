namespace Khidmah_Inventory.Application.Common.Constants;

/// <summary>
/// SignalR / real-time event names. Must match frontend and hub method names.
/// </summary>
public static class OperationsEventNames
{
    public const string EntityChanged = "EntityChanged";
    public const string EntityDeleted = "EntityDeleted";
    public const string ProductCreated = "ProductCreated";
    public const string ProductDeleted = "ProductDeleted";
    public const string OrderUpdated = "OrderUpdated";
    public const string OrderStatusChanged = "OrderStatusChanged";
    public const string CustomerUpdated = "CustomerUpdated";
    public const string SupplierUpdated = "SupplierUpdated";
    public const string FinancePosted = "FinancePosted";
    public const string StockChanged = "StockChanged";
    public const string ProductUpdated = "ProductUpdated";
    public const string OrderCreated = "OrderCreated";
    public const string OrderApproved = "OrderApproved";
    public const string PurchaseCreated = "PurchaseCreated";
    public const string SaleCompleted = "SaleCompleted";
    public const string LowStockDetected = "LowStockDetected";
    public const string BatchExpiring = "BatchExpiring";
    public const string CommentAdded = "CommentAdded";
    public const string ActivityCreated = "ActivityCreated";
    public const string NotificationRaised = "NotificationRaised";
    public const string DashboardUpdated = "DashboardUpdated";
}
