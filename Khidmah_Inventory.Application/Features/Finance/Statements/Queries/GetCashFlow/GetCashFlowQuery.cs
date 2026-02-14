using MediatR;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Finance.Statements.Models;

namespace Khidmah_Inventory.Application.Features.Finance.Statements.Queries.GetCashFlow;

public class GetCashFlowQuery : IRequest<Result<CashFlowDto>>
{
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
}
