using MediatR;
using Khidmah_Inventory.Application.Common.Models;

namespace Khidmah_Inventory.Application.Features.Finance.Accounts.Commands.DeleteAccount;

public class DeleteAccountCommand : IRequest<Result>
{
    public Guid Id { get; set; }
}
