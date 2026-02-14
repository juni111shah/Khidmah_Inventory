using MediatR;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Finance.Accounts.Models;

namespace Khidmah_Inventory.Application.Features.Finance.Accounts.Queries.GetAccountsTree;

public class GetAccountsTreeQuery : IRequest<Result<List<AccountDto>>>
{
    public bool IncludeInactive { get; set; }
}
