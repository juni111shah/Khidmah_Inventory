using MediatR;
using Microsoft.Extensions.Logging;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Settings.Models;

namespace Khidmah_Inventory.Application.Features.Settings.Queries.GetUISettings;

public class GetUISettingsQueryHandler : IRequestHandler<GetUISettingsQuery, Result<UISettingsDto>>
{
    private readonly ISettingsRepository _settingsRepository;
    private readonly ICurrentUserService _currentUser;
    private readonly ILogger<GetUISettingsQueryHandler> _logger;

    public GetUISettingsQueryHandler(
        ISettingsRepository settingsRepository,
        ICurrentUserService currentUser,
        ILogger<GetUISettingsQueryHandler> logger)
    {
        _settingsRepository = settingsRepository;
        _currentUser = currentUser;
        _logger = logger;
    }

    public async Task<Result<UISettingsDto>> Handle(GetUISettingsQuery request, CancellationToken cancellationToken)
    {
        if (_currentUser.UserId == null || _currentUser.CompanyId == null)
        {
            return Result<UISettingsDto>.Failure("User context is required");
        }

        var settingsKey = $"ui-{_currentUser.UserId}";
        var settings = await _settingsRepository.GetSettingsAsync<UISettingsDto>(
            _currentUser.CompanyId.Value,
            "UI",
            settingsKey,
            cancellationToken);

        if (settings == null)
        {
            settings = new UISettingsDto();
        }

        _logger.LogDebug("UI settings retrieved for user: {UserId}", _currentUser.UserId);
        return Result<UISettingsDto>.Success(settings);
    }
}

