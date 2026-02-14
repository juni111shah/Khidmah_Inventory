using Microsoft.EntityFrameworkCore;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Domain.Entities;

namespace Khidmah_Inventory.Infrastructure.Services;

/// <summary>
/// Standard account codes used for auto-posting. Companies should have accounts with these codes (e.g. from template import).
/// </summary>
public static class StandardAccountCodes
{
    public const string Cash = "CASH";
    public const string AccountsReceivable = "AR";
    public const string Revenue = "REVENUE";
    public const string TaxPayable = "TAX";
    public const string Inventory = "INVENTORY";
    public const string AccountsPayable = "AP";
    public const string Expense = "EXPENSE";
}

public class AccountingPostingService : IAccountingPostingService
{
    private readonly IApplicationDbContext _context;

    public AccountingPostingService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Guid?> PostSaleAsync(
        Guid companyId,
        Guid sourceId,
        string reference,
        DateTime date,
        decimal totalAmount,
        decimal taxAmount,
        bool isCashSale,
        CancellationToken cancellationToken = default)
    {
        var revenueAmount = totalAmount - taxAmount;
        Guid? cashOrArId = await GetAccountIdByCodeAsync(companyId, isCashSale ? StandardAccountCodes.Cash : StandardAccountCodes.AccountsReceivable, cancellationToken);
        var revenueId = await GetAccountIdByCodeAsync(companyId, StandardAccountCodes.Revenue, cancellationToken);
        Guid? taxId = taxAmount > 0 ? await GetAccountIdByCodeAsync(companyId, StandardAccountCodes.TaxPayable, cancellationToken) : null;

        if (!cashOrArId.HasValue || !revenueId.HasValue)
            return null;

        var entry = new JournalEntry(companyId, date, reference, "Sale", sourceId, $"Sale {reference}");
        entry.AddLine(cashOrArId.Value, totalAmount, 0);
        entry.AddLine(revenueId.Value, 0, revenueAmount);
        if (taxId.HasValue && taxAmount > 0)
            entry.AddLine(taxId.Value, 0, taxAmount);

        if (!entry.IsBalanced())
            return null;

        _context.JournalEntries.Add(entry);
        await _context.SaveChangesAsync(cancellationToken);
        return entry.Id;
    }

    public async Task<Guid?> PostPurchaseAsync(
        Guid companyId,
        Guid sourceId,
        string reference,
        DateTime date,
        decimal totalAmount,
        CancellationToken cancellationToken = default)
    {
        var inventoryId = await GetAccountIdByCodeAsync(companyId, StandardAccountCodes.Inventory, cancellationToken);
        var apId = await GetAccountIdByCodeAsync(companyId, StandardAccountCodes.AccountsPayable, cancellationToken);
        if (!inventoryId.HasValue || !apId.HasValue)
            return null;

        var entry = new JournalEntry(companyId, date, reference, "Purchase", sourceId, $"Purchase {reference}");
        entry.AddLine(inventoryId.Value, totalAmount, 0);
        entry.AddLine(apId.Value, 0, totalAmount);

        if (!entry.IsBalanced())
            return null;

        _context.JournalEntries.Add(entry);
        await _context.SaveChangesAsync(cancellationToken);
        return entry.Id;
    }

    public async Task<Guid?> PostAdjustmentLossAsync(
        Guid companyId,
        Guid? sourceId,
        string reference,
        DateTime date,
        decimal lossAmount,
        string sourceModule,
        CancellationToken cancellationToken = default)
    {
        var expenseId = await GetAccountIdByCodeAsync(companyId, StandardAccountCodes.Expense, cancellationToken);
        var inventoryId = await GetAccountIdByCodeAsync(companyId, StandardAccountCodes.Inventory, cancellationToken);
        if (!expenseId.HasValue || !inventoryId.HasValue)
            return null;

        var entry = new JournalEntry(companyId, date, reference, sourceModule, sourceId, $"Adjustment {reference}");
        entry.AddLine(expenseId.Value, lossAmount, 0);
        entry.AddLine(inventoryId.Value, 0, lossAmount);

        if (!entry.IsBalanced())
            return null;

        _context.JournalEntries.Add(entry);
        await _context.SaveChangesAsync(cancellationToken);
        return entry.Id;
    }

    public async Task<Guid?> PostPaymentAsync(
        Guid companyId,
        Guid? sourceId,
        string reference,
        DateTime date,
        decimal amount,
        CancellationToken cancellationToken = default)
    {
        var cashId = await GetAccountIdByCodeAsync(companyId, StandardAccountCodes.Cash, cancellationToken);
        var arId = await GetAccountIdByCodeAsync(companyId, StandardAccountCodes.AccountsReceivable, cancellationToken);
        if (!cashId.HasValue || !arId.HasValue)
            return null;

        var entry = new JournalEntry(companyId, date, reference, "Payment", sourceId, $"Payment {reference}");
        entry.AddLine(cashId.Value, amount, 0);
        entry.AddLine(arId.Value, 0, amount);

        if (!entry.IsBalanced())
            return null;

        _context.JournalEntries.Add(entry);
        await _context.SaveChangesAsync(cancellationToken);
        return entry.Id;
    }

    private async Task<Guid?> GetAccountIdByCodeAsync(Guid companyId, string code, CancellationToken cancellationToken)
    {
        var id = await _context.Accounts
            .AsNoTracking()
            .Where(a => a.CompanyId == companyId && !a.IsDeleted && a.IsActive && a.Code == code)
            .Select(a => (Guid?)a.Id)
            .FirstOrDefaultAsync(cancellationToken);
        return id;
    }
}
