using MediatR;
using Microsoft.EntityFrameworkCore;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Finance.Accounts.Models;
using Khidmah_Inventory.Domain.Entities;

namespace Khidmah_Inventory.Application.Features.Finance.Accounts.Queries.GetAccountsTree;

public class GetAccountsTreeQueryHandler : IRequestHandler<GetAccountsTreeQuery, Result<List<AccountDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetAccountsTreeQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<List<AccountDto>>> Handle(GetAccountsTreeQuery request, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId;
        if (!companyId.HasValue)
            return Result<List<AccountDto>>.Failure("Company context is required.");

        var query = _context.Accounts
            .AsNoTracking()
            .Where(a => a.CompanyId == companyId && !a.IsDeleted);
        if (!request.IncludeInactive)
            query = query.Where(a => a.IsActive);

        var list = await query.OrderBy(a => a.Code).ToListAsync(cancellationToken);
        var dtos = list.Select(MapToDto).ToList();
        var tree = BuildTree(dtos, null);
        return Result<List<AccountDto>>.Success(tree);
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

    private static List<AccountDto> BuildTree(List<AccountDto> all, Guid? parentId)
    {
        return all.Where(a => a.ParentAccountId == parentId).Select(a =>
        {
            a.Children = BuildTree(all, a.Id);
            return a;
        }).ToList();
    }
}
