namespace Khidmah_Inventory.Application.Features.Finance.Statements.Models;

public class CashFlowDto
{
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public decimal OperatingInflow { get; set; }
    public decimal OperatingOutflow { get; set; }
    public decimal NetOperating => OperatingInflow - OperatingOutflow;
    public decimal InvestingInflow { get; set; }
    public decimal InvestingOutflow { get; set; }
    public decimal NetInvesting => InvestingInflow - InvestingOutflow;
    public decimal FinancingInflow { get; set; }
    public decimal FinancingOutflow { get; set; }
    public decimal NetFinancing => FinancingInflow - FinancingOutflow;
    public decimal NetCashChange => NetOperating + NetInvesting + NetFinancing;
}
