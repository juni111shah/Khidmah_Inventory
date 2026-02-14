using MediatR;
using Microsoft.EntityFrameworkCore;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Finance.Accounts.Models;
using Khidmah_Inventory.Domain.Entities;

namespace Khidmah_Inventory.Application.Features.Finance.Accounts.Queries.GetAccount;

public class GetAccountQueryHandler : IRequestHandler<GetAccountQuery, Result<AccountDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetAccountQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<AccountDto>> Handle(GetAccountQuery request, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId;
        if (!companyId.HasValue)
            return Result<AccountDto>.Failure("Company context is required.");

        var account = await _context.Accounts
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == request.Id && a.CompanyId == companyId && !a.IsDeleted, cancellationToken);
        if (account == null)
            return Result<AccountDto>.Failure("Account not found.");

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
