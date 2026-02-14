using MediatR;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Finance.Statements.Models;

namespace Khidmah_Inventory.Application.Features.Finance.Statements.Queries.GetBalanceSheet;

public class GetBalanceSheetQuery : IRequest<Result<BalanceSheetDto>>
{
    public DateTime AsOfDate { get; set; }
}
