using MediatR;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Finance.Accounts.Models;
using Khidmah_Inventory.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Khidmah_Inventory.Application.Features.Finance.Accounts.Commands.UpdateAccount;

public class UpdateAccountCommandHandler : IRequestHandler<UpdateAccountCommand, Result<AccountDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public UpdateAccountCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<AccountDto>> Handle(UpdateAccountCommand request, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId;
        if (!companyId.HasValue)
            return Result<AccountDto>.Failure("Company context is required.");

        var account = await _context.Accounts
            .FirstOrDefaultAsync(a => a.Id == request.Id && a.CompanyId == companyId && !a.IsDeleted, cancellationToken);
        if (account == null)
            return Result<AccountDto>.Failure("Account not found.");

        var duplicateCode = await _context.Accounts
            .AnyAsync(a => a.CompanyId == companyId && !a.IsDeleted && a.Code == request.Code && a.Id != request.Id, cancellationToken);
        if (duplicateCode)
            return Result<AccountDto>.Failure("Another account with this code already exists.");

        account.Update(request.Code, request.Name, request.Type, request.IsActive, _currentUser.UserId);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<AccountDto>.Success(new AccountDto
        {
            Id = account.Id,
            CompanyId = account.CompanyId,
            Code = account.Code,
            Name = account.Name,
            Type = account.Type,
            TypeName = account.Type.ToString(),
            ParentAccountId = account.ParentAccountId,
            IsActive = account.IsActive
        });
    }
}
