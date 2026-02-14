using MediatR;
using Microsoft.EntityFrameworkCore;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.ExchangeRates.Models;
using Khidmah_Inventory.Domain.Entities;

namespace Khidmah_Inventory.Application.Features.ExchangeRates.Commands.CreateExchangeRate;

public class CreateExchangeRateCommandHandler : IRequestHandler<CreateExchangeRateCommand, Result<ExchangeRateDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public CreateExchangeRateCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<ExchangeRateDto>> Handle(CreateExchangeRateCommand request, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId;
        if (!companyId.HasValue)
            return Result<ExchangeRateDto>.Failure("Company context is required.");

        var from = await _context.Currencies
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == request.FromCurrencyId && c.CompanyId == companyId && c.IsDeleted == false, cancellationToken);
        var to = await _context.Currencies
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == request.ToCurrencyId && c.CompanyId == companyId && c.IsDeleted == false, cancellationToken);
        if (from == null || to == null)
            return Result<ExchangeRateDto>.Failure("One or both currencies not found.");
        if (request.FromCurrencyId == request.ToCurrencyId)
            return Result<ExchangeRateDto>.Failure("From and to currency must be different.");
        if (request.Rate <= 0)
            return Result<ExchangeRateDto>.Failure("Rate must be greater than zero.");

        var rate = new ExchangeRate(
            companyId.Value,
            request.FromCurrencyId,
            request.ToCurrencyId,
            request.Rate,
            request.Date.Date,
            _currentUser.UserId);
        _context.ExchangeRates.Add(rate);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<ExchangeRateDto>.Success(new ExchangeRateDto
        {
            Id = rate.Id,
            FromCurrencyId = rate.FromCurrencyId,
            FromCurrencyCode = from.Code,
            ToCurrencyId = rate.ToCurrencyId,
            ToCurrencyCode = to.Code,
            Rate = rate.Rate,
            Date = rate.Date
        });
    }
}
