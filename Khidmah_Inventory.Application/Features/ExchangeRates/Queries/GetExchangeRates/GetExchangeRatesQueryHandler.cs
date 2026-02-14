using MediatR;
using Microsoft.EntityFrameworkCore;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.ExchangeRates.Models;
using Khidmah_Inventory.Domain.Entities;

namespace Khidmah_Inventory.Application.Features.ExchangeRates.Queries.GetExchangeRates;

public class GetExchangeRatesQueryHandler : IRequestHandler<GetExchangeRatesQuery, Result<GetExchangeRatesResult>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetExchangeRatesQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<GetExchangeRatesResult>> Handle(GetExchangeRatesQuery request, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId;
        if (!companyId.HasValue)
            return Result<GetExchangeRatesResult>.Failure("Company context is required.");

        IQueryable<ExchangeRate> query = _context.ExchangeRates
            .AsNoTracking()
            .Where(r => r.CompanyId == companyId && r.IsDeleted == false)
            .Include(r => r.FromCurrency)
            .Include(r => r.ToCurrency);

        if (request.FromDate.HasValue)
            query = query.Where(r => r.Date >= request.FromDate.Value.Date);
        if (request.ToDate.HasValue)
            query = query.Where(r => r.Date <= request.ToDate.Value.Date);
        if (request.FromCurrencyId.HasValue)
            query = query.Where(r => r.FromCurrencyId == request.FromCurrencyId.Value);
        if (request.ToCurrencyId.HasValue)
            query = query.Where(r => r.ToCurrencyId == request.ToCurrencyId.Value);

        var list = await query
            .OrderByDescending(r => r.Date)
            .ThenBy(r => r.FromCurrency!.Code)
            .Select(r => new ExchangeRateDto
            {
                Id = r.Id,
                FromCurrencyId = r.FromCurrencyId,
                FromCurrencyCode = r.FromCurrency!.Code,
                ToCurrencyId = r.ToCurrencyId,
                ToCurrencyCode = r.ToCurrency!.Code,
                Rate = r.Rate,
                Date = r.Date
            })
            .ToListAsync(cancellationToken);

        return Result<GetExchangeRatesResult>.Success(new GetExchangeRatesResult { Items = list });
    }
}
