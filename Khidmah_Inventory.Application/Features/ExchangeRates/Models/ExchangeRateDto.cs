namespace Khidmah_Inventory.Application.Features.ExchangeRates.Models;

public class ExchangeRateDto
{
    public Guid Id { get; set; }
    public Guid FromCurrencyId { get; set; }
    public string FromCurrencyCode { get; set; } = string.Empty;
    public Guid ToCurrencyId { get; set; }
    public string ToCurrencyCode { get; set; } = string.Empty;
    public decimal Rate { get; set; }
    public DateTime Date { get; set; }
}
