using MediatR;
using Microsoft.EntityFrameworkCore;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Currency.Models;

namespace Khidmah_Inventory.Application.Features.Currency.Queries.GetCurrenciesList;

public class GetCurrenciesListQueryHandler : IRequestHandler<GetCurrenciesListQuery, Result<GetCurrenciesListResult>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetCurrenciesListQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<GetCurrenciesListResult>> Handle(GetCurrenciesListQuery request, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId;
        if (!companyId.HasValue)
            return Result<GetCurrenciesListResult>.Failure("Company context is required.");

        var query = _context.Currencies.AsNoTracking().Where(c => c.CompanyId == companyId && c.IsDeleted == false);

        var list = await query
            .OrderBy(c => c.Code)
            .Select(c => new CurrencyDto
            {
                Id = c.Id,
                CompanyId = c.CompanyId,
                Code = c.Code,
                Name = c.Name,
                Symbol = c.Symbol,
                IsBase = c.IsBase
            })
            .ToListAsync(cancellationToken);

        return Result<GetCurrenciesListResult>.Success(new GetCurrenciesListResult { Items = list });
    }
}
