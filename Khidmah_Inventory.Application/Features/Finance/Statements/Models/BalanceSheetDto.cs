namespace Khidmah_Inventory.Application.Features.Finance.Statements.Models;

public class BalanceSheetDto
{
    public DateTime AsOfDate { get; set; }
    public decimal TotalAssets { get; set; }
    public decimal TotalLiabilities { get; set; }
    public decimal TotalEquity { get; set; }
    public decimal TotalLiabilitiesAndEquity => TotalLiabilities + TotalEquity;
    public bool IsBalanced => Math.Abs(TotalAssets - TotalLiabilitiesAndEquity) < 0.01m;
    public List<AccountLineDto> AssetLines { get; set; } = new();
    public List<AccountLineDto> LiabilityLines { get; set; } = new();
    public List<AccountLineDto> EquityLines { get; set; } = new();
}
