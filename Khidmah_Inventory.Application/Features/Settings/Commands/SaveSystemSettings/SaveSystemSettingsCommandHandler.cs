using MediatR;
using Microsoft.Extensions.Logging;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Settings.Models;

namespace Khidmah_Inventory.Application.Features.Settings.Commands.SaveSystemSettings;

public class SaveSystemSettingsCommandHandler : IRequestHandler<SaveSystemSettingsCommand, Result<SystemSettingsDto>>
{
    private readonly ISettingsRepository _settingsRepository;
    private readonly ICurrentUserService _currentUser;
    private readonly ILogger<SaveSystemSettingsCommandHandler> _logger;

    public SaveSystemSettingsCommandHandler(
        ISettingsRepository settingsRepository,
        ICurrentUserService currentUser,
        ILogger<SaveSystemSettingsCommandHandler> logger)
    {
        _settingsRepository = settingsRepository;
        _currentUser = currentUser;
        _logger = logger;
    }

    public async Task<Result<SystemSettingsDto>> Handle(SaveSystemSettingsCommand request, CancellationToken cancellationToken)
    {
        if (_currentUser.CompanyId == null)
        {
            return Result<SystemSettingsDto>.Failure("Company context is required");
        }

        if (request.Settings == null)
        {
            return Result<SystemSettingsDto>.Failure("Settings data is required");
        }

        await _settingsRepository.SaveSettingsAsync(
            _currentUser.CompanyId.Value,
            "System",
            "system",
            request.Settings,
            "System settings",
            cancellationToken);

        _logger.LogInformation("System settings saved for company: {CompanyId}", _currentUser.CompanyId);
        return Result<SystemSettingsDto>.Success(request.Settings);
    }
}

