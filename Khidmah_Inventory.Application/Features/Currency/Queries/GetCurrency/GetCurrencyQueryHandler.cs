using MediatR;
using Microsoft.EntityFrameworkCore;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Currency.Models;

namespace Khidmah_Inventory.Application.Features.Currency.Queries.GetCurrency;

public class GetCurrencyQueryHandler : IRequestHandler<GetCurrencyQuery, Result<CurrencyDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetCurrencyQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<CurrencyDto>> Handle(GetCurrencyQuery request, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId;
        if (!companyId.HasValue)
            return Result<CurrencyDto>.Failure("Company context is required.");

        var currency = await _context.Currencies
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == request.Id && c.CompanyId == companyId && c.IsDeleted == false, cancellationToken);
        if (currency == null)
            return Result<CurrencyDto>.Failure("Currency not found.");

        return Result<CurrencyDto>.Success(new CurrencyDto
        {
            Id = currency.Id,
            CompanyId = currency.CompanyId,
            Code = currency.Code,
            Name = currency.Name,
            Symbol = currency.Symbol,
            IsBase = currency.IsBase
        });
    }
}
