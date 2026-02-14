using MediatR;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;
using Microsoft.EntityFrameworkCore;

namespace Khidmah_Inventory.Application.Features.Finance.Accounts.Commands.DeleteAccount;

public class DeleteAccountCommandHandler : IRequestHandler<DeleteAccountCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public DeleteAccountCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(DeleteAccountCommand request, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId;
        if (!companyId.HasValue)
            return Result.Failure("Company context is required.");

        var account = await _context.Accounts
            .FirstOrDefaultAsync(a => a.Id == request.Id && a.CompanyId == companyId && !a.IsDeleted, cancellationToken);
        if (account == null)
            return Result.Failure("Account not found.");

        var hasChildren = await _context.Accounts.AnyAsync(a => a.ParentAccountId == request.Id && !a.IsDeleted, cancellationToken);
        if (hasChildren)
            return Result.Failure("Cannot delete an account that has child accounts. Remove or reassign children first.");

        account.MarkAsDeleted(_currentUser.UserId);
        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
