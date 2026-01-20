using MediatR;
using Microsoft.Extensions.Logging;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Theme.Models;

namespace Khidmah_Inventory.Application.Features.Theme.Queries.GetUserTheme;

public class GetUserThemeQueryHandler : IRequestHandler<GetUserThemeQuery, Result<ThemeDto>>
{
    private readonly IThemeRepository _themeRepository;
    private readonly ILogger<GetUserThemeQueryHandler> _logger;

    public GetUserThemeQueryHandler(
        IThemeRepository themeRepository,
        ILogger<GetUserThemeQueryHandler> logger)
    {
        _themeRepository = themeRepository;
        _logger = logger;
    }

    public async Task<Result<ThemeDto>> Handle(GetUserThemeQuery request, CancellationToken cancellationToken)
    {
        var theme = await _themeRepository.GetUserThemeAsync(cancellationToken);
        _logger.LogDebug("User theme retrieved successfully");
        
        return Result<ThemeDto>.Success(theme);
    }
}

