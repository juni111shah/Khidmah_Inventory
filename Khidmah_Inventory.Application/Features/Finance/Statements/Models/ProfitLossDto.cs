namespace Khidmah_Inventory.Application.Features.Finance.Statements.Models;

public class ProfitLossDto
{
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public decimal Revenue { get; set; }
    public decimal Cogs { get; set; }
    public decimal GrossProfit => Revenue - Cogs;
    public decimal Expenses { get; set; }
    public decimal NetProfit => GrossProfit - Expenses;
    public decimal? GrossMarginPercent => Revenue != 0 ? (GrossProfit / Revenue) * 100 : null;
    public decimal? NetMarginPercent => Revenue != 0 ? (NetProfit / Revenue) * 100 : null;
    public List<AccountLineDto> RevenueLines { get; set; } = new();
    public List<AccountLineDto> ExpenseLines { get; set; } = new();
}

public class AccountLineDto
{
    public Guid AccountId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public decimal Amount { get; set; }
}
