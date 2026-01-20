using MediatR;
using Microsoft.Extensions.Logging;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Settings.Models;

namespace Khidmah_Inventory.Application.Features.Settings.Commands.SaveUserSettings;

public class SaveUserSettingsCommandHandler : IRequestHandler<SaveUserSettingsCommand, Result<UserSettingsDto>>
{
    private readonly ISettingsRepository _settingsRepository;
    private readonly ICurrentUserService _currentUser;
    private readonly ILogger<SaveUserSettingsCommandHandler> _logger;

    public SaveUserSettingsCommandHandler(
        ISettingsRepository settingsRepository,
        ICurrentUserService currentUser,
        ILogger<SaveUserSettingsCommandHandler> logger)
    {
        _settingsRepository = settingsRepository;
        _currentUser = currentUser;
        _logger = logger;
    }

    public async Task<Result<UserSettingsDto>> Handle(SaveUserSettingsCommand request, CancellationToken cancellationToken)
    {
        if (_currentUser.UserId == null || _currentUser.CompanyId == null)
        {
            return Result<UserSettingsDto>.Failure("User context is required");
        }

        if (request.Settings == null)
        {
            return Result<UserSettingsDto>.Failure("Settings data is required");
        }

        var settingsKey = $"user-{_currentUser.UserId}";
        await _settingsRepository.SaveSettingsAsync(
            _currentUser.CompanyId.Value,
            "User",
            settingsKey,
            request.Settings,
            "User settings",
            cancellationToken);

        _logger.LogInformation("User settings saved for user: {UserId}", _currentUser.UserId);
        return Result<UserSettingsDto>.Success(request.Settings);
    }
}

