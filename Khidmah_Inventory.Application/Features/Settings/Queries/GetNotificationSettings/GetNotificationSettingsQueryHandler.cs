using MediatR;
using Microsoft.Extensions.Logging;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Settings.Models;

namespace Khidmah_Inventory.Application.Features.Settings.Queries.GetNotificationSettings;

public class GetNotificationSettingsQueryHandler : IRequestHandler<GetNotificationSettingsQuery, Result<NotificationSettingsDto>>
{
    private readonly ISettingsRepository _settingsRepository;
    private readonly ICurrentUserService _currentUser;
    private readonly ILogger<GetNotificationSettingsQueryHandler> _logger;

    public GetNotificationSettingsQueryHandler(
        ISettingsRepository settingsRepository,
        ICurrentUserService currentUser,
        ILogger<GetNotificationSettingsQueryHandler> logger)
    {
        _settingsRepository = settingsRepository;
        _currentUser = currentUser;
        _logger = logger;
    }

    public async Task<Result<NotificationSettingsDto>> Handle(GetNotificationSettingsQuery request, CancellationToken cancellationToken)
    {
        if (_currentUser.CompanyId == null)
        {
            return Result<NotificationSettingsDto>.Failure("Company context is required");
        }

        var settings = await _settingsRepository.GetSettingsAsync<NotificationSettingsDto>(
            _currentUser.CompanyId.Value,
            "Notification",
            "notification",
            cancellationToken);

        if (settings == null)
        {
            settings = new NotificationSettingsDto();
        }

        _logger.LogDebug("Notification settings retrieved for company: {CompanyId}", _currentUser.CompanyId);
        return Result<NotificationSettingsDto>.Success(settings);
    }
}

