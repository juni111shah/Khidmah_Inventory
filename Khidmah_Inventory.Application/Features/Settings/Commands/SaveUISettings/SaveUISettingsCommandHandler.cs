using MediatR;
using Microsoft.Extensions.Logging;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Settings.Models;

namespace Khidmah_Inventory.Application.Features.Settings.Commands.SaveUISettings;

public class SaveUISettingsCommandHandler : IRequestHandler<SaveUISettingsCommand, Result<UISettingsDto>>
{
    private readonly ISettingsRepository _settingsRepository;
    private readonly ICurrentUserService _currentUser;
    private readonly ILogger<SaveUISettingsCommandHandler> _logger;

    public SaveUISettingsCommandHandler(
        ISettingsRepository settingsRepository,
        ICurrentUserService currentUser,
        ILogger<SaveUISettingsCommandHandler> logger)
    {
        _settingsRepository = settingsRepository;
        _currentUser = currentUser;
        _logger = logger;
    }

    public async Task<Result<UISettingsDto>> Handle(SaveUISettingsCommand request, CancellationToken cancellationToken)
    {
        if (_currentUser.UserId == null || _currentUser.CompanyId == null)
        {
            return Result<UISettingsDto>.Failure("User context is required");
        }

        if (request.Settings == null)
        {
            return Result<UISettingsDto>.Failure("Settings data is required");
        }

        var settingsKey = $"ui-{_currentUser.UserId}";
        await _settingsRepository.SaveSettingsAsync(
            _currentUser.CompanyId.Value,
            "UI",
            settingsKey,
            request.Settings,
            "UI settings",
            cancellationToken);

        _logger.LogInformation("UI settings saved for user: {UserId}", _currentUser.UserId);
        return Result<UISettingsDto>.Success(request.Settings);
    }
}

