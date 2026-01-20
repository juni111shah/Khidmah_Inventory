using MediatR;
using Microsoft.Extensions.Logging;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Settings.Models;

namespace Khidmah_Inventory.Application.Features.Settings.Commands.SaveNotificationSettings;

public class SaveNotificationSettingsCommandHandler : IRequestHandler<SaveNotificationSettingsCommand, Result<NotificationSettingsDto>>
{
    private readonly ISettingsRepository _settingsRepository;
    private readonly ICurrentUserService _currentUser;
    private readonly ILogger<SaveNotificationSettingsCommandHandler> _logger;

    public SaveNotificationSettingsCommandHandler(
        ISettingsRepository settingsRepository,
        ICurrentUserService currentUser,
        ILogger<SaveNotificationSettingsCommandHandler> logger)
    {
        _settingsRepository = settingsRepository;
        _currentUser = currentUser;
        _logger = logger;
    }

    public async Task<Result<NotificationSettingsDto>> Handle(SaveNotificationSettingsCommand request, CancellationToken cancellationToken)
    {
        if (_currentUser.CompanyId == null)
        {
            return Result<NotificationSettingsDto>.Failure("Company context is required");
        }

        if (request.Settings == null)
        {
            return Result<NotificationSettingsDto>.Failure("Settings data is required");
        }

        await _settingsRepository.SaveSettingsAsync(
            _currentUser.CompanyId.Value,
            "Notification",
            "notification",
            request.Settings,
            "Notification settings",
            cancellationToken);

        _logger.LogInformation("Notification settings saved for company: {CompanyId}", _currentUser.CompanyId);
        return Result<NotificationSettingsDto>.Success(request.Settings);
    }
}

