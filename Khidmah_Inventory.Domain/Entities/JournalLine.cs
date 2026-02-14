using Khidmah_Inventory.Domain.Common;

namespace Khidmah_Inventory.Domain.Entities;

public class JournalLine : BaseEntity
{
    public Guid JournalEntryId { get; private set; }
    public Guid AccountId { get; private set; }
    public decimal Debit { get; private set; }
    public decimal Credit { get; private set; }
    public string? Memo { get; private set; }
    /// <summary>Amount in base currency (when multi-currency).</summary>
    public decimal? BaseDebit { get; private set; }
    public decimal? BaseCredit { get; private set; }

    public virtual JournalEntry JournalEntry { get; private set; } = null!;
    public virtual Account Account { get; private set; } = null!;

    private JournalLine() { }

    public JournalLine(
        Guid companyId,
        Guid journalEntryId,
        Guid accountId,
        decimal debit,
        decimal credit,
        Guid? createdBy = null,
        string? memo = null,
        decimal? baseDebit = null,
        decimal? baseCredit = null) : base(companyId, createdBy)
    {
        JournalEntryId = journalEntryId;
        AccountId = accountId;
        Debit = debit;
        Credit = credit;
        Memo = memo;
        BaseDebit = baseDebit;
        BaseCredit = baseCredit;
    }
}
