using System.Linq;
using Khidmah_Inventory.Domain.Common;

namespace Khidmah_Inventory.Domain.Entities;

/// <summary>
/// Source module for auto-posted journal entries.
/// </summary>
public static class JournalSourceModule
{
    public const string Sale = "Sale";
    public const string Purchase = "Purchase";
    public const string Pos = "POS";
    public const string Adjustment = "Adjustment";
    public const string Payment = "Payment";
    public const string StockIn = "StockIn";
    public const string StockOut = "StockOut";
}

public class JournalEntry : Entity
{
    public DateTime Date { get; private set; }
    public string Reference { get; private set; } = string.Empty;
    public string SourceModule { get; private set; } = string.Empty;
    public Guid? SourceId { get; private set; }
    public string? Description { get; private set; }

    /// <summary>Transaction currency; null means base currency.</summary>
    public Guid? TransactionCurrencyId { get; private set; }
    /// <summary>Base (reporting) currency for the company.</summary>
    public Guid? BaseCurrencyId { get; private set; }
    /// <summary>Rate to convert transaction amount to base (1.0 when same currency).</summary>
    public decimal ConversionRateToBase { get; private set; } = 1m;

    public virtual ICollection<JournalLine> Lines { get; private set; } = new List<JournalLine>();

    private JournalEntry() { }

    public JournalEntry(
        Guid companyId,
        DateTime date,
        string reference,
        string sourceModule,
        Guid? sourceId = null,
        string? description = null,
        Guid? createdBy = null) : base(companyId, createdBy)
    {
        Date = date;
        Reference = reference;
        SourceModule = sourceModule;
        SourceId = sourceId;
        Description = description;
    }

    public void SetCurrencyInfo(Guid? transactionCurrencyId, Guid? baseCurrencyId, decimal conversionRateToBase)
    {
        TransactionCurrencyId = transactionCurrencyId;
        BaseCurrencyId = baseCurrencyId;
        ConversionRateToBase = conversionRateToBase;
    }

    public void AddLine(Guid accountId, decimal debit, decimal credit)
    {
        if (debit < 0 || credit < 0)
            throw new ArgumentException("Debit and credit must be non-negative.");
        var baseDebit = TransactionCurrencyId.HasValue && ConversionRateToBase != 0 ? debit * ConversionRateToBase : (decimal?)null;
        var baseCredit = TransactionCurrencyId.HasValue && ConversionRateToBase != 0 ? credit * ConversionRateToBase : (decimal?)null;
        Lines.Add(new JournalLine(CompanyId, Id, accountId, debit, credit, CreatedBy, null, baseDebit, baseCredit));
    }

    public void AddLine(Guid accountId, decimal debit, decimal credit, decimal? baseDebit, decimal? baseCredit, string? memo = null)
    {
        if (debit < 0 || credit < 0)
            throw new ArgumentException("Debit and credit must be non-negative.");
        Lines.Add(new JournalLine(CompanyId, Id, accountId, debit, credit, CreatedBy, memo, baseDebit, baseCredit));
    }

    /// <summary>
    /// Validates that total debits equal total credits.
    /// </summary>
    public bool IsBalanced()
    {
        var totalDebit = Lines.Sum(l => l.Debit);
        var totalCredit = Lines.Sum(l => l.Credit);
        return totalDebit == totalCredit;
    }
}
