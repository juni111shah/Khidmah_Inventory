namespace Khidmah_Inventory.Application.Common.Interfaces;

/// <summary>
/// Converts amounts between currencies using company exchange rates.
/// Supports reporting in transaction currency or base currency.
/// </summary>
public interface ICurrencyConversionService
{
    /// <summary>
    /// Converts amount from one currency to another using the rate effective on the given date.
    /// </summary>
    /// <param name="amount">Amount in fromCurrency</param>
    /// <param name="fromCurrencyCode">Source currency code (e.g. USD)</param>
    /// <param name="toCurrencyCode">Target currency code</param>
    /// <param name="asOfDate">Date for rate lookup; null = latest</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Converted amount in toCurrency, or null if rate not found</returns>
    Task<decimal?> ConvertAsync(
        decimal amount,
        string fromCurrencyCode,
        string toCurrencyCode,
        DateTime? asOfDate = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the exchange rate from fromCurrency to toCurrency (e.g. 1 USD = X EUR).
    /// </summary>
    Task<decimal?> GetRateAsync(
        string fromCurrencyCode,
        string toCurrencyCode,
        DateTime? asOfDate = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the base currency for the current company (IsBase = true).
    /// </summary>
    Task<(string Code, Guid Id)?> GetBaseCurrencyAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Converts amount to base currency using company's base currency and rates.
    /// </summary>
    Task<decimal?> ToBaseCurrencyAsync(
        decimal amount,
        string transactionCurrencyCode,
        DateTime? asOfDate = null,
        CancellationToken cancellationToken = default);
}
