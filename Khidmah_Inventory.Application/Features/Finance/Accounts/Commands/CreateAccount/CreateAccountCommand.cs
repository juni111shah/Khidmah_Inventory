using MediatR;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Finance.Accounts.Models;
using Khidmah_Inventory.Domain.Entities;

namespace Khidmah_Inventory.Application.Features.Finance.Accounts.Commands.CreateAccount;

public class CreateAccountCommand : IRequest<Result<AccountDto>>
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public AccountType Type { get; set; }
    public Guid? ParentAccountId { get; set; }
}
