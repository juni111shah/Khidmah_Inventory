using MediatR;
using Microsoft.EntityFrameworkCore;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Finance.Accounts.Models;
using Khidmah_Inventory.Domain.Entities;

namespace Khidmah_Inventory.Application.Features.Finance.Accounts.Commands.CreateAccount;

public class CreateAccountCommandHandler : IRequestHandler<CreateAccountCommand, Result<AccountDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public CreateAccountCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<AccountDto>> Handle(CreateAccountCommand request, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId;
        if (!companyId.HasValue)
            return Result<AccountDto>.Failure("Company context is required.");

        var exists = await _context.Accounts
            .AnyAsync(a => a.CompanyId == companyId && !a.IsDeleted && a.Code == request.Code, cancellationToken);
        if (exists)
            return Result<AccountDto>.Failure("An account with this code already exists.");

        var account = new Account(companyId.Value, request.Code, request.Name, request.Type, request.ParentAccountId, _currentUser.UserId);
        _context.Accounts.Add(account);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<AccountDto>.Success(MapToDto(account));
    }

    private static AccountDto MapToDto(Account a)
    {
        return new AccountDto
        {
            Id = a.Id,
            CompanyId = a.CompanyId,
            Code = a.Code,
            Name = a.Name,
            Type = a.Type,
            TypeName = a.Type.ToString(),
            ParentAccountId = a.ParentAccountId,
            IsActive = a.IsActive
        };
    }
}
