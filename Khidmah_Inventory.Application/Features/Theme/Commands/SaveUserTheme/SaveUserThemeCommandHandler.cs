using MediatR;
using Microsoft.Extensions.Logging;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Theme.Models;

namespace Khidmah_Inventory.Application.Features.Theme.Commands.SaveUserTheme;

public class SaveUserThemeCommandHandler : IRequestHandler<SaveUserThemeCommand, Result<ThemeDto>>
{
    private readonly IThemeRepository _themeRepository;
    private readonly ILogger<SaveUserThemeCommandHandler> _logger;

    public SaveUserThemeCommandHandler(
        IThemeRepository themeRepository,
        ILogger<SaveUserThemeCommandHandler> logger)
    {
        _themeRepository = themeRepository;
        _logger = logger;
    }

    public async Task<Result<ThemeDto>> Handle(SaveUserThemeCommand request, CancellationToken cancellationToken)
    {
        if (request.Theme == null)
        {
            return Result<ThemeDto>.Failure("Theme data is required");
        }

        await _themeRepository.SaveUserThemeAsync(request.Theme, cancellationToken);
        _logger.LogInformation("User theme saved successfully");
        
        return Result<ThemeDto>.Success(request.Theme);
    }
}

