using MediatR;
using Microsoft.Extensions.Logging;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Settings.Models;

namespace Khidmah_Inventory.Application.Features.Settings.Commands.SaveReportSettings;

public class SaveReportSettingsCommandHandler : IRequestHandler<SaveReportSettingsCommand, Result<ReportSettingsDto>>
{
    private readonly ISettingsRepository _settingsRepository;
    private readonly ICurrentUserService _currentUser;
    private readonly ILogger<SaveReportSettingsCommandHandler> _logger;

    public SaveReportSettingsCommandHandler(
        ISettingsRepository settingsRepository,
        ICurrentUserService currentUser,
        ILogger<SaveReportSettingsCommandHandler> logger)
    {
        _settingsRepository = settingsRepository;
        _currentUser = currentUser;
        _logger = logger;
    }

    public async Task<Result<ReportSettingsDto>> Handle(SaveReportSettingsCommand request, CancellationToken cancellationToken)
    {
        if (_currentUser.CompanyId == null)
        {
            return Result<ReportSettingsDto>.Failure("Company context is required");
        }

        if (request.Settings == null)
        {
            return Result<ReportSettingsDto>.Failure("Settings data is required");
        }

        await _settingsRepository.SaveSettingsAsync(
            _currentUser.CompanyId.Value,
            "Report",
            "report",
            request.Settings,
            "Report settings",
            cancellationToken);

        _logger.LogInformation("Report settings saved for company: {CompanyId}", _currentUser.CompanyId);
        return Result<ReportSettingsDto>.Success(request.Settings);
    }
}

