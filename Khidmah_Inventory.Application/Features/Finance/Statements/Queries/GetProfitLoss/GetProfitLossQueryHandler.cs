using MediatR;
using Microsoft.EntityFrameworkCore;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Finance.Statements.Models;
using Khidmah_Inventory.Domain.Entities;

namespace Khidmah_Inventory.Application.Features.Finance.Statements.Queries.GetProfitLoss;

public class GetProfitLossQueryHandler : IRequestHandler<GetProfitLossQuery, Result<ProfitLossDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetProfitLossQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<ProfitLossDto>> Handle(GetProfitLossQuery request, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId;
        if (!companyId.HasValue)
            return Result<ProfitLossDto>.Failure("Company context is required.");

        var from = request.FromDate.Date;
        var to = request.ToDate.Date;

        var lines = await _context.JournalLines.AsNoTracking()
            .Where(l => l.IsDeleted == false)
            .Join(_context.JournalEntries.AsNoTracking(), l => l.JournalEntryId, j => j.Id, (l, j) => new { l, j })
            .Where(x => x.j.CompanyId == companyId && x.j.IsDeleted == false && x.j.Date >= from && x.j.Date <= to)
            .Join(_context.Accounts.AsNoTracking(), x => x.l.AccountId, a => a.Id, (x, a) => new { x.l, x.j, a })
            .Where(x => x.a.CompanyId == companyId && x.a.IsDeleted == false)
            .Select(x => new { x.a.Id, x.a.Code, x.a.Name, x.a.Type, x.l.Debit, x.l.Credit })
            .ToListAsync(cancellationToken);

        var revenueAccounts = lines.Where(x => x.Type == AccountType.Revenue).GroupBy(x => new { x.Id, x.Code, x.Name });
        var expenseAccounts = lines.Where(x => x.Type == AccountType.Expense).GroupBy(x => new { x.Id, x.Code, x.Name });

        decimal revenue = 0;
        var revenueLines = new List<AccountLineDto>();
        foreach (var g in revenueAccounts)
        {
            var amount = g.Sum(x => x.Credit - x.Debit);
            if (amount <= 0) continue;
            revenue += amount;
            revenueLines.Add(new AccountLineDto { AccountId = g.Key.Id, Code = g.Key.Code, Name = g.Key.Name, Amount = amount });
        }

        decimal cogs = 0;
        decimal expenses = 0;
        var expenseLines = new List<AccountLineDto>();
        foreach (var g in expenseAccounts)
        {
            var amount = g.Sum(x => x.Debit - x.Credit);
            if (amount <= 0) continue;
            expenses += amount;
            expenseLines.Add(new AccountLineDto { AccountId = g.Key.Id, Code = g.Key.Code, Name = g.Key.Name, Amount = amount });
        }

        var dto = new ProfitLossDto
        {
            FromDate = from,
            ToDate = to,
            Revenue = revenue,
            Cogs = cogs,
            Expenses = expenses,
            RevenueLines = revenueLines.OrderByDescending(x => x.Amount).ToList(),
            ExpenseLines = expenseLines.OrderByDescending(x => x.Amount).ToList()
        };
        return Result<ProfitLossDto>.Success(dto);
    }
}
