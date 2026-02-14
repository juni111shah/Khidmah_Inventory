using MediatR;
using Microsoft.EntityFrameworkCore;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Finance.Journals.Models;
using Khidmah_Inventory.Domain.Entities;

namespace Khidmah_Inventory.Application.Features.Finance.Journals.Queries.GetJournalEntries;

public class GetJournalEntriesQueryHandler : IRequestHandler<GetJournalEntriesQuery, Result<GetJournalEntriesResult>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetJournalEntriesQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<GetJournalEntriesResult>> Handle(GetJournalEntriesQuery request, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId;
        if (!companyId.HasValue)
            return Result<GetJournalEntriesResult>.Failure("Company context is required.");

        IQueryable<JournalEntry> query = _context.JournalEntries
            .AsNoTracking()
            .Where(j => j.CompanyId == companyId && !j.IsDeleted)
            .Include(j => j.Lines)
            .ThenInclude(l => l.Account);

        if (request.DateFrom.HasValue)
            query = query.Where(j => j.Date >= request.DateFrom.Value);
        if (request.DateTo.HasValue)
            query = query.Where(j => j.Date <= request.DateTo.Value);
        if (!string.IsNullOrWhiteSpace(request.SourceModule))
            query = query.Where(j => j.SourceModule == request.SourceModule);

        var totalCount = await query.CountAsync(cancellationToken);
        var entries = await query
            .OrderByDescending(j => j.Date)
            .ThenByDescending(j => j.CreatedAt)
            .Skip((request.PageNo - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        var items = entries.Select(j => new JournalEntryDto
        {
            Id = j.Id,
            CompanyId = j.CompanyId,
            Date = j.Date,
            Reference = j.Reference,
            SourceModule = j.SourceModule,
            SourceId = j.SourceId,
            Description = j.Description,
            TotalDebit = j.Lines.Sum(l => l.Debit),
            TotalCredit = j.Lines.Sum(l => l.Credit),
            Lines = j.Lines.Select(l => new JournalLineDto
            {
                Id = l.Id,
                AccountId = l.AccountId,
                AccountCode = l.Account?.Code ?? "",
                AccountName = l.Account?.Name ?? "",
                Debit = l.Debit,
                Credit = l.Credit,
                Memo = l.Memo
            }).ToList()
        }).ToList();

        return Result<GetJournalEntriesResult>.Success(new GetJournalEntriesResult
        {
            Items = items,
            TotalCount = totalCount,
            PageNo = request.PageNo,
            PageSize = request.PageSize
        });
    }
}
