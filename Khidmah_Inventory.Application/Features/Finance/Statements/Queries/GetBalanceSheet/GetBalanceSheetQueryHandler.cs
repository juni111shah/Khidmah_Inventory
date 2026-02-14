using MediatR;
using Microsoft.EntityFrameworkCore;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Finance.Statements.Models;
using Khidmah_Inventory.Domain.Entities;

namespace Khidmah_Inventory.Application.Features.Finance.Statements.Queries.GetBalanceSheet;

public class GetBalanceSheetQueryHandler : IRequestHandler<GetBalanceSheetQuery, Result<BalanceSheetDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetBalanceSheetQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<BalanceSheetDto>> Handle(GetBalanceSheetQuery request, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId;
        if (!companyId.HasValue)
            return Result<BalanceSheetDto>.Failure("Company context is required.");

        var asOf = request.AsOfDate.Date;

        var lines = await (
            from l in _context.JournalLines.AsNoTracking()
            join j in _context.JournalEntries.AsNoTracking() on l.JournalEntryId equals j.Id
            join a in _context.Accounts.AsNoTracking() on l.AccountId equals a.Id
            where j.CompanyId == companyId && !j.IsDeleted && !l.IsDeleted && a.CompanyId == companyId && !a.IsDeleted
                && j.Date <= asOf
            select new { a.Id, a.Code, a.Name, a.Type, l.Debit, l.Credit }
        ).ToListAsync(cancellationToken);

        decimal Bal(AccountType type, decimal debit, decimal credit) =>
            type == AccountType.Asset || type == AccountType.Expense ? debit - credit : credit - debit;

        var assetLines = lines.Where(x => x.Type == AccountType.Asset).GroupBy(x => new { x.Id, x.Code, x.Name })
            .Select(g => new AccountLineDto { AccountId = g.Key.Id, Code = g.Key.Code, Name = g.Key.Name, Amount = g.Sum(x => x.Debit - x.Credit) })
            .Where(x => x.Amount > 0).ToList();
        var liabilityLines = lines.Where(x => x.Type == AccountType.Liability).GroupBy(x => new { x.Id, x.Code, x.Name })
            .Select(g => new AccountLineDto { AccountId = g.Key.Id, Code = g.Key.Code, Name = g.Key.Name, Amount = g.Sum(x => x.Credit - x.Debit) })
            .Where(x => x.Amount > 0).ToList();
        var equityLines = lines.Where(x => x.Type == AccountType.Equity).GroupBy(x => new { x.Id, x.Code, x.Name })
            .Select(g => new AccountLineDto { AccountId = g.Key.Id, Code = g.Key.Code, Name = g.Key.Name, Amount = g.Sum(x => x.Credit - x.Debit) })
            .Where(x => x.Amount != 0).ToList();

        var revenueBalance = lines.Where(x => x.Type == AccountType.Revenue).Sum(x => x.Credit - x.Debit);
        var expenseBalance = lines.Where(x => x.Type == AccountType.Expense).Sum(x => x.Debit - x.Credit);
        var netIncome = revenueBalance - expenseBalance;
        equityLines.Add(new AccountLineDto { AccountId = Guid.Empty, Code = "R/E", Name = "Retained Earnings (Net Income)", Amount = netIncome });

        var totalAssets = assetLines.Sum(x => x.Amount);
        var totalLiabilities = liabilityLines.Sum(x => x.Amount);
        var totalEquity = equityLines.Sum(x => x.Amount);

        var dto = new BalanceSheetDto
        {
            AsOfDate = asOf,
            TotalAssets = totalAssets,
            TotalLiabilities = totalLiabilities,
            TotalEquity = totalEquity,
            AssetLines = assetLines,
            LiabilityLines = liabilityLines,
            EquityLines = equityLines
        };
        return Result<BalanceSheetDto>.Success(dto);
    }
}
