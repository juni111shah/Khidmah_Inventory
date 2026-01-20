using MediatR;
using Microsoft.Extensions.Logging;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Settings.Models;

namespace Khidmah_Inventory.Application.Features.Settings.Queries.GetUserSettings;

public class GetUserSettingsQueryHandler : IRequestHandler<GetUserSettingsQuery, Result<UserSettingsDto>>
{
    private readonly ISettingsRepository _settingsRepository;
    private readonly ICurrentUserService _currentUser;
    private readonly ILogger<GetUserSettingsQueryHandler> _logger;

    public GetUserSettingsQueryHandler(
        ISettingsRepository settingsRepository,
        ICurrentUserService currentUser,
        ILogger<GetUserSettingsQueryHandler> logger)
    {
        _settingsRepository = settingsRepository;
        _currentUser = currentUser;
        _logger = logger;
    }

    public async Task<Result<UserSettingsDto>> Handle(GetUserSettingsQuery request, CancellationToken cancellationToken)
    {
        if (_currentUser.UserId == null || _currentUser.CompanyId == null)
        {
            return Result<UserSettingsDto>.Failure("User context is required");
        }

        var settingsKey = $"user-{_currentUser.UserId}";
        var settings = await _settingsRepository.GetSettingsAsync<UserSettingsDto>(
            _currentUser.CompanyId.Value,
            "User",
            settingsKey,
            cancellationToken);

        if (settings == null)
        {
            settings = new UserSettingsDto();
        }

        _logger.LogDebug("User settings retrieved for user: {UserId}", _currentUser.UserId);
        return Result<UserSettingsDto>.Success(settings);
    }
}

