using MediatR;
using Microsoft.Extensions.Logging;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Settings.Models;

namespace Khidmah_Inventory.Application.Features.Settings.Commands.SaveCompanySettings;

public class SaveCompanySettingsCommandHandler : IRequestHandler<SaveCompanySettingsCommand, Result<CompanySettingsDto>>
{
    private readonly ISettingsRepository _settingsRepository;
    private readonly ICurrentUserService _currentUser;
    private readonly ILogger<SaveCompanySettingsCommandHandler> _logger;

    public SaveCompanySettingsCommandHandler(
        ISettingsRepository settingsRepository,
        ICurrentUserService currentUser,
        ILogger<SaveCompanySettingsCommandHandler> logger)
    {
        _settingsRepository = settingsRepository;
        _currentUser = currentUser;
        _logger = logger;
    }

    public async Task<Result<CompanySettingsDto>> Handle(SaveCompanySettingsCommand request, CancellationToken cancellationToken)
    {
        if (_currentUser.CompanyId == null)
        {
            return Result<CompanySettingsDto>.Failure("Company context is required");
        }

        if (request.Settings == null)
        {
            return Result<CompanySettingsDto>.Failure("Settings data is required");
        }

        await _settingsRepository.SaveSettingsAsync(
            _currentUser.CompanyId.Value,
            "Company",
            "company",
            request.Settings,
            "Company settings",
            cancellationToken);

        _logger.LogInformation("Company settings saved for company: {CompanyId}", _currentUser.CompanyId);
        return Result<CompanySettingsDto>.Success(request.Settings);
    }
}

