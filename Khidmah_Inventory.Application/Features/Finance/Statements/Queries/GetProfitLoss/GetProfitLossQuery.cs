using MediatR;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Finance.Statements.Models;

namespace Khidmah_Inventory.Application.Features.Finance.Statements.Queries.GetProfitLoss;

public class GetProfitLossQuery : IRequest<Result<ProfitLossDto>>
{
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
}
