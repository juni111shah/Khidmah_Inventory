using MediatR;
using Microsoft.EntityFrameworkCore;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Platform.Integrations.Models;

namespace Khidmah_Inventory.Application.Features.Platform.Integrations.Queries.GetIntegrationsList;

public class GetIntegrationsListQueryHandler : IRequestHandler<GetIntegrationsListQuery, Result<List<IntegrationDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    private static readonly List<AvailableIntegrationTypeDto> AllTypes = new()
    {
        new() { Type = "Email", DisplayName = "Email Service", Description = "Send transactional emails" },
        new() { Type = "SMS", DisplayName = "SMS", Description = "SMS notifications" },
        new() { Type = "WhatsApp", DisplayName = "WhatsApp", Description = "WhatsApp Business API" },
        new() { Type = "Payment", DisplayName = "Payment Providers", Description = "Stripe, PayPal, etc." },
        new() { Type = "BI", DisplayName = "External BI", Description = "Power BI, Tableau, etc." },
        new() { Type = "Ecommerce", DisplayName = "E-commerce", Description = "Shopify, WooCommerce, etc." }
    };

    public GetIntegrationsListQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<List<IntegrationDto>>> Handle(GetIntegrationsListQuery request, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId;
        if (!companyId.HasValue)
            return Result<List<IntegrationDto>>.Failure("Company context is required");

        var existing = await _context.CompanyIntegrations
            .Where(i => i.CompanyId == companyId.Value && !i.IsDeleted)
            .ToDictionaryAsync(i => i.IntegrationType, cancellationToken);

        var list = AllTypes.Select(t => new IntegrationDto
        {
            IntegrationType = t.Type,
            DisplayName = t.DisplayName,
            Description = t.Description,
            IsEnabled = existing.TryGetValue(t.Type, out var e) && e.IsEnabled,
            IsConfigured = existing.TryGetValue(t.Type, out var ex) && !string.IsNullOrEmpty(ex.ConfigJson)
        }).ToList();

        return Result<List<IntegrationDto>>.Success(list);
    }
}
