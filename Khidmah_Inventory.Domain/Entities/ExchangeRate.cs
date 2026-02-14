using Khidmah_Inventory.Domain.Common;

namespace Khidmah_Inventory.Domain.Entities;

public class ExchangeRate : BaseEntity
{
    public Guid FromCurrencyId { get; private set; }
    public Guid ToCurrencyId { get; private set; }
    public decimal Rate { get; private set; }
    public DateTime Date { get; private set; }

    public virtual Currency FromCurrency { get; private set; } = null!;
    public virtual Currency ToCurrency { get; private set; } = null!;

    private ExchangeRate() { }

    public ExchangeRate(
        Guid companyId,
        Guid fromCurrencyId,
        Guid toCurrencyId,
        decimal rate,
        DateTime date,
        Guid? createdBy = null) : base(companyId, createdBy)
    {
        FromCurrencyId = fromCurrencyId;
        ToCurrencyId = toCurrencyId;
        Rate = rate;
        Date = date;
    }

    public void UpdateRate(decimal rate, Guid? updatedBy = null)
    {
        Rate = rate;
        UpdateAuditInfo(updatedBy);
    }
}
