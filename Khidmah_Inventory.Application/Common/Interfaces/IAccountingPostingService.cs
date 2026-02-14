namespace Khidmah_Inventory.Application.Common.Interfaces;

/// <summary>
/// Double-entry accounting posting. Call from handlers when sales, purchases, adjustments, or payments occur.
/// </summary>
public interface IAccountingPostingService
{
    /// <summary>
    /// Post sale: Dr Receivable/Cash, Cr Revenue, Cr Tax (if any).
    /// </summary>
    Task<Guid?> PostSaleAsync(
        Guid companyId,
        Guid sourceId,
        string reference,
        DateTime date,
        decimal totalAmount,
        decimal taxAmount,
        bool isCashSale,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Post purchase: Dr Inventory/Expense, Cr Accounts Payable.
    /// </summary>
    Task<Guid?> PostPurchaseAsync(
        Guid companyId,
        Guid sourceId,
        string reference,
        DateTime date,
        decimal totalAmount,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Post stock adjustment loss: Dr Expense, Cr Inventory.
    /// </summary>
    Task<Guid?> PostAdjustmentLossAsync(
        Guid companyId,
        Guid? sourceId,
        string reference,
        DateTime date,
        decimal lossAmount,
        string sourceModule,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Post payment: Dr Cash, Cr Accounts Receivable.
    /// </summary>
    Task<Guid?> PostPaymentAsync(
        Guid companyId,
        Guid? sourceId,
        string reference,
        DateTime date,
        decimal amount,
        CancellationToken cancellationToken = default);
}
