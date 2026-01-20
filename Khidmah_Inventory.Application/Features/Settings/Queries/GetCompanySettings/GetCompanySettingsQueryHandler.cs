using MediatR;
using Microsoft.Extensions.Logging;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Settings.Models;

namespace Khidmah_Inventory.Application.Features.Settings.Queries.GetCompanySettings;

public class GetCompanySettingsQueryHandler : IRequestHandler<GetCompanySettingsQuery, Result<CompanySettingsDto>>
{
    private readonly ISettingsRepository _settingsRepository;
    private readonly ICurrentUserService _currentUser;
    private readonly ILogger<GetCompanySettingsQueryHandler> _logger;

    public GetCompanySettingsQueryHandler(
        ISettingsRepository settingsRepository,
        ICurrentUserService currentUser,
        ILogger<GetCompanySettingsQueryHandler> logger)
    {
        _settingsRepository = settingsRepository;
        _currentUser = currentUser;
        _logger = logger;
    }

    public async Task<Result<CompanySettingsDto>> Handle(GetCompanySettingsQuery request, CancellationToken cancellationToken)
    {
        if (_currentUser.CompanyId == null)
        {
            return Result<CompanySettingsDto>.Failure("Company context is required");
        }

        var settings = await _settingsRepository.GetSettingsAsync<CompanySettingsDto>(
            _currentUser.CompanyId.Value,
            "Company",
            "company",
            cancellationToken);

        if (settings == null)
        {
            // Return default settings if not found
            settings = new CompanySettingsDto();
        }

        _logger.LogDebug("Company settings retrieved for company: {CompanyId}", _currentUser.CompanyId);
        return Result<CompanySettingsDto>.Success(settings);
    }
}

