using Microsoft.EntityFrameworkCore;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Domain.Entities;
using Khidmah_Inventory.Infrastructure.Data;

namespace Khidmah_Inventory.Infrastructure.Services;

public class CurrencyConversionService : ICurrencyConversionService
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public CurrencyConversionService(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<decimal?> ConvertAsync(
        decimal amount,
        string fromCurrencyCode,
        string toCurrencyCode,
        DateTime? asOfDate = null,
        CancellationToken cancellationToken = default)
    {
        var rate = await GetRateAsync(fromCurrencyCode, toCurrencyCode, asOfDate, cancellationToken);
        return rate.HasValue ? amount * rate.Value : null;
    }

    public async Task<decimal?> GetRateAsync(
        string fromCurrencyCode,
        string toCurrencyCode,
        DateTime? asOfDate = null,
        CancellationToken cancellationToken = default)
    {
        var companyId = _currentUser.CompanyId;
        if (!companyId.HasValue) return null;

        var fromCode = fromCurrencyCode.Trim().ToUpperInvariant();
        var toCode = toCurrencyCode.Trim().ToUpperInvariant();
        if (fromCode == toCode) return 1m;

        IQueryable<ExchangeRate> query = _context.ExchangeRates
            .AsNoTracking()
            .Where(r => r.CompanyId == companyId && r.IsDeleted == false)
            .Include(r => r.FromCurrency)
            .Include(r => r.ToCurrency);

        if (asOfDate.HasValue)
            query = query.Where(r => r.Date <= asOfDate.Value.Date);

        // Direct: From -> To
        var direct = await query
            .Where(r => r.FromCurrency!.Code == fromCode && r.ToCurrency!.Code == toCode)
            .OrderByDescending(r => r.Date)
            .FirstOrDefaultAsync(cancellationToken);
        if (direct != null) return direct.Rate;

        // Inverse: To -> From, rate = 1/Rate
        var inverse = await query
            .Where(r => r.FromCurrency!.Code == toCode && r.ToCurrency!.Code == fromCode)
            .OrderByDescending(r => r.Date)
            .FirstOrDefaultAsync(cancellationToken);
        if (inverse != null) return 1m / inverse.Rate;

        return null;
    }

    public async Task<(string Code, Guid Id)?> GetBaseCurrencyAsync(CancellationToken cancellationToken = default)
    {
        var companyId = _currentUser.CompanyId;
        if (!companyId.HasValue) return null;

        var baseCurrency = await _context.Currencies
            .AsNoTracking()
            .Where(c => c.CompanyId == companyId && c.IsDeleted == false && c.IsBase)
            .Select(c => new { c.Code, c.Id })
            .FirstOrDefaultAsync(cancellationToken);

        return baseCurrency != null ? (baseCurrency.Code, baseCurrency.Id) : null;
    }

    public async Task<decimal?> ToBaseCurrencyAsync(
        decimal amount,
        string transactionCurrencyCode,
        DateTime? asOfDate = null,
        CancellationToken cancellationToken = default)
    {
        var baseInfo = await GetBaseCurrencyAsync(cancellationToken);
        if (!baseInfo.HasValue) return null;
        return await ConvertAsync(amount, transactionCurrencyCode, baseInfo.Value.Code, asOfDate, cancellationToken);
    }
}
