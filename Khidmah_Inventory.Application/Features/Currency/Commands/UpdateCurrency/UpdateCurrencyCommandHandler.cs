using MediatR;
using Microsoft.EntityFrameworkCore;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Currency.Models;

namespace Khidmah_Inventory.Application.Features.Currency.Commands.UpdateCurrency;

public class UpdateCurrencyCommandHandler : IRequestHandler<UpdateCurrencyCommand, Result<CurrencyDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public UpdateCurrencyCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<CurrencyDto>> Handle(UpdateCurrencyCommand request, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId;
        if (!companyId.HasValue)
            return Result<CurrencyDto>.Failure("Company context is required.");

        var currency = await _context.Currencies
            .FirstOrDefaultAsync(c => c.Id == request.Id && c.CompanyId == companyId && c.IsDeleted == false, cancellationToken);
        if (currency == null)
            return Result<CurrencyDto>.Failure("Currency not found.");

        var codeUpper = request.Code.Trim().ToUpperInvariant();
        var duplicate = await _context.Currencies
            .AnyAsync(c => c.CompanyId == companyId && c.IsDeleted == false && c.Code == codeUpper && c.Id != request.Id, cancellationToken);
        if (duplicate)
            return Result<CurrencyDto>.Failure("Another currency with this code already exists.");

        if (request.IsBase && !currency.IsBase)
        {
            var currentBase = await _context.Currencies
                .Where(c => c.CompanyId == companyId && c.IsDeleted == false && c.IsBase)
                .ToListAsync(cancellationToken);
            foreach (var c in currentBase)
                c.UnsetAsBase(_currentUser.UserId);
        }

        currency.Update(codeUpper, request.Name.Trim(), request.Symbol.Trim(), request.IsBase, _currentUser.UserId);
        await _context.SaveChangesAsync(cancellationToken);

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
