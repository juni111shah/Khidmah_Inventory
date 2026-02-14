namespace Khidmah_Inventory.Application.Features.Currency.Models;

public class CurrencyDto
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Symbol { get; set; } = string.Empty;
    public bool IsBase { get; set; }
}
