using MediatR;
using Microsoft.EntityFrameworkCore;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;

namespace Khidmah_Inventory.Application.Features.Currency.Commands.DeleteCurrency;

public class DeleteCurrencyCommandHandler : IRequestHandler<DeleteCurrencyCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public DeleteCurrencyCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(DeleteCurrencyCommand request, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId;
        if (!companyId.HasValue)
            return Result.Failure("Company context is required.");

        var currency = await _context.Currencies
            .FirstOrDefaultAsync(c => c.Id == request.Id && c.CompanyId == companyId && c.IsDeleted == false, cancellationToken);
        if (currency == null)
            return Result.Failure("Currency not found.");

        var hasRates = await _context.ExchangeRates
            .AnyAsync(r => (r.FromCurrencyId == request.Id || r.ToCurrencyId == request.Id) && r.IsDeleted == false, cancellationToken);
        if (hasRates)
            return Result.Failure("Cannot delete currency that has exchange rates. Remove rates first.");

        currency.MarkAsDeleted(_currentUser.UserId);
        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
