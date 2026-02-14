using MediatR;
using Microsoft.EntityFrameworkCore;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Platform.Integrations.Models;
using Khidmah_Inventory.Domain.Entities;

namespace Khidmah_Inventory.Application.Features.Platform.Integrations.Commands.ToggleIntegration;

public class ToggleIntegrationCommandHandler : IRequestHandler<ToggleIntegrationCommand, Result<IntegrationDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public ToggleIntegrationCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<IntegrationDto>> Handle(ToggleIntegrationCommand request, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId;
        if (!companyId.HasValue)
            return Result<IntegrationDto>.Failure("Company context is required");

        var type = request.IntegrationType?.Trim() ?? "";
        if (string.IsNullOrEmpty(type))
            return Result<IntegrationDto>.Failure("Integration type is required");

        var existing = await _context.CompanyIntegrations
            .FirstOrDefaultAsync(i => i.CompanyId == companyId.Value && i.IntegrationType == type && !i.IsDeleted, cancellationToken);

        if (existing != null)
        {
            existing.SetEnabled(request.IsEnabled, _currentUser.UserId);
        }
        else
        {
            var created = new CompanyIntegration(companyId.Value, type, request.IsEnabled, null, type, null, _currentUser.UserId);
            _context.CompanyIntegrations.Add(created);
        }

        await _context.SaveChangesAsync(cancellationToken);

        var updated = await _context.CompanyIntegrations
            .FirstOrDefaultAsync(i => i.CompanyId == companyId.Value && i.IntegrationType == type && !i.IsDeleted, cancellationToken);

        return Result<IntegrationDto>.Success(new IntegrationDto
        {
            IntegrationType = type,
            DisplayName = updated?.DisplayName ?? type,
            Description = updated?.Description,
            IsEnabled = updated?.IsEnabled ?? request.IsEnabled,
            IsConfigured = !string.IsNullOrEmpty(updated?.ConfigJson)
        });
    }
}
