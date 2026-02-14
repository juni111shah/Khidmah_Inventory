using MediatR;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Finance.Accounts.Models;

namespace Khidmah_Inventory.Application.Features.Finance.Accounts.Queries.GetAccount;

public class GetAccountQuery : IRequest<Result<AccountDto>>
{
    public Guid Id { get; set; }
}
