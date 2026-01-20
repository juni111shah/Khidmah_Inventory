using MediatR;
using Microsoft.Extensions.Logging;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Settings.Models;

namespace Khidmah_Inventory.Application.Features.Settings.Queries.GetReportSettings;

public class GetReportSettingsQueryHandler : IRequestHandler<GetReportSettingsQuery, Result<ReportSettingsDto>>
{
    private readonly ISettingsRepository _settingsRepository;
    private readonly ICurrentUserService _currentUser;
    private readonly ILogger<GetReportSettingsQueryHandler> _logger;

    public GetReportSettingsQueryHandler(
        ISettingsRepository settingsRepository,
        ICurrentUserService currentUser,
        ILogger<GetReportSettingsQueryHandler> logger)
    {
        _settingsRepository = settingsRepository;
        _currentUser = currentUser;
        _logger = logger;
    }

    public async Task<Result<ReportSettingsDto>> Handle(GetReportSettingsQuery request, CancellationToken cancellationToken)
    {
        if (_currentUser.CompanyId == null)
        {
            return Result<ReportSettingsDto>.Failure("Company context is required");
        }

        var settings = await _settingsRepository.GetSettingsAsync<ReportSettingsDto>(
            _currentUser.CompanyId.Value,
            "Report",
            "report",
            cancellationToken);

        if (settings == null)
        {
            settings = new ReportSettingsDto();
        }

        _logger.LogDebug("Report settings retrieved for company: {CompanyId}", _currentUser.CompanyId);
        return Result<ReportSettingsDto>.Success(settings);
    }
}

