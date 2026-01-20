using MediatR;
using Microsoft.Extensions.Logging;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Settings.Models;

namespace Khidmah_Inventory.Application.Features.Settings.Queries.GetSystemSettings;

public class GetSystemSettingsQueryHandler : IRequestHandler<GetSystemSettingsQuery, Result<SystemSettingsDto>>
{
    private readonly ISettingsRepository _settingsRepository;
    private readonly ICurrentUserService _currentUser;
    private readonly ILogger<GetSystemSettingsQueryHandler> _logger;

    public GetSystemSettingsQueryHandler(
        ISettingsRepository settingsRepository,
        ICurrentUserService currentUser,
        ILogger<GetSystemSettingsQueryHandler> logger)
    {
        _settingsRepository = settingsRepository;
        _currentUser = currentUser;
        _logger = logger;
    }

    public async Task<Result<SystemSettingsDto>> Handle(GetSystemSettingsQuery request, CancellationToken cancellationToken)
    {
        if (_currentUser.CompanyId == null)
        {
            return Result<SystemSettingsDto>.Failure("Company context is required");
        }

        var settings = await _settingsRepository.GetSettingsAsync<SystemSettingsDto>(
            _currentUser.CompanyId.Value,
            "System",
            "system",
            cancellationToken);

        if (settings == null)
        {
            settings = new SystemSettingsDto();
        }

        _logger.LogDebug("System settings retrieved for company: {CompanyId}", _currentUser.CompanyId);
        return Result<SystemSettingsDto>.Success(settings);
    }
}

