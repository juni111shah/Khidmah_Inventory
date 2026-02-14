using MediatR;
using Microsoft.EntityFrameworkCore;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Finance.Statements.Models;

namespace Khidmah_Inventory.Application.Features.Finance.Statements.Queries.GetCashFlow;

public class GetCashFlowQueryHandler : IRequestHandler<GetCashFlowQuery, Result<CashFlowDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetCashFlowQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<CashFlowDto>> Handle(GetCashFlowQuery request, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId;
        if (!companyId.HasValue)
            return Result<CashFlowDto>.Failure("Company context is required.");

        var from = request.FromDate.Date;
        var to = request.ToDate.Date;

        var cashAccountIds = await _context.Accounts
            .AsNoTracking()
            .Where(a => a.CompanyId == companyId && !a.IsDeleted && a.Code == "CASH")
            .Select(a => a.Id)
            .ToListAsync(cancellationToken);
        if (cashAccountIds.Count == 0)
        {
            return Result<CashFlowDto>.Success(new CashFlowDto { FromDate = from, ToDate = to });
        }

        var movements = await _context.JournalLines.AsNoTracking()
            .Where(l => l.IsDeleted == false && cashAccountIds.Contains(l.AccountId))
            .Join(_context.JournalEntries.AsNoTracking(), l => l.JournalEntryId, j => j.Id, (l, j) => new { l, j })
            .Where(x => x.j.CompanyId == companyId && x.j.IsDeleted == false && x.j.Date >= from && x.j.Date <= to)
            .Select(x => new { x.l.Debit, x.l.Credit, x.j.SourceModule })
            .ToListAsync(cancellationToken);

        decimal operatingIn = 0, operatingOut = 0, investingIn = 0, investingOut = 0, financingIn = 0, financingOut = 0;
        foreach (var m in movements)
        {
            var net = m.Debit - m.Credit;
            switch (m.SourceModule)
            {
                case "Sale":
                case "POS":
                case "Payment" when net > 0:
                    if (net > 0) operatingIn += net; else operatingOut += -net;
                    break;
                case "Purchase":
                case "Adjustment":
                    if (net > 0) operatingIn += net; else operatingOut += -net;
                    break;
                default:
                    if (net > 0) operatingIn += net; else operatingOut += -net;
                    break;
            }
        }

        var dto = new CashFlowDto
        {
            FromDate = from,
            ToDate = to,
            OperatingInflow = operatingIn,
            OperatingOutflow = operatingOut,
            InvestingInflow = investingIn,
            InvestingOutflow = investingOut,
            FinancingInflow = financingIn,
            FinancingOutflow = financingOut
        };
        return Result<CashFlowDto>.Success(dto);
    }
}
