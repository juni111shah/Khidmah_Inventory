using MediatR;
using Microsoft.Extensions.Logging;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Theme.Models;

namespace Khidmah_Inventory.Application.Features.Theme.Commands.SaveGlobalTheme;

public class SaveGlobalThemeCommandHandler : IRequestHandler<SaveGlobalThemeCommand, Result<ThemeDto>>
{
    private readonly IThemeRepository _themeRepository;
    private readonly ILogger<SaveGlobalThemeCommandHandler> _logger;

    public SaveGlobalThemeCommandHandler(
        IThemeRepository themeRepository,
        ILogger<SaveGlobalThemeCommandHandler> logger)
    {
        _themeRepository = themeRepository;
        _logger = logger;
    }

    public async Task<Result<ThemeDto>> Handle(SaveGlobalThemeCommand request, CancellationToken cancellationToken)
    {
        if (request.Theme == null)
        {
            return Result<ThemeDto>.Failure("Theme data is required");
        }

        await _themeRepository.SaveGlobalThemeAsync(request.Theme, cancellationToken);
        _logger.LogInformation("Global theme saved successfully");
        
        return Result<ThemeDto>.Success(request.Theme);
    }
}

