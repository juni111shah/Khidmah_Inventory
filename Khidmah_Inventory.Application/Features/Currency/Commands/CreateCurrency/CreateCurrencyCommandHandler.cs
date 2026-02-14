using MediatR;
using Microsoft.EntityFrameworkCore;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Currency.Models;
using Khidmah_Inventory.Domain.Entities;

namespace Khidmah_Inventory.Application.Features.Currency.Commands.CreateCurrency;

public class CreateCurrencyCommandHandler : IRequestHandler<CreateCurrencyCommand, Result<CurrencyDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public CreateCurrencyCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<CurrencyDto>> Handle(CreateCurrencyCommand request, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId;
        if (!companyId.HasValue)
            return Result<CurrencyDto>.Failure("Company context is required.");

        var codeUpper = request.Code.Trim().ToUpperInvariant();
        var exists = await _context.Currencies
            .AnyAsync(c => c.CompanyId == companyId && c.IsDeleted == false && c.Code == codeUpper, cancellationToken);
        if (exists)
            return Result<CurrencyDto>.Failure("A currency with this code already exists.");

        if (request.IsBase)
        {
            var currentBase = await _context.Currencies
                .Where(c => c.CompanyId == companyId && c.IsDeleted == false && c.IsBase)
                .ToListAsync(cancellationToken);
            foreach (var c in currentBase)
                c.UnsetAsBase(_currentUser.UserId);
        }

        var currency = new Domain.Entities.Currency(
            companyId.Value,
            codeUpper,
            request.Name.Trim(),
            request.Symbol.Trim(),
            request.IsBase,
            _currentUser.UserId);
        _context.Currencies.Add(currency);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<CurrencyDto>.Success(Map(currency));
    }

    private static CurrencyDto Map(Domain.Entities.Currency c)
    {
        return new CurrencyDto
        {
            Id = c.Id,
            CompanyId = c.CompanyId,
            Code = c.Code,
            Name = c.Name,
            Symbol = c.Symbol,
            IsBase = c.IsBase
        };
    }
}
