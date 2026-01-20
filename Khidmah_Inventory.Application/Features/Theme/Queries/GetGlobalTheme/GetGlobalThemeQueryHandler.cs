using MediatR;
using Microsoft.Extensions.Logging;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Theme.Models;

namespace Khidmah_Inventory.Application.Features.Theme.Queries.GetGlobalTheme;

public class GetGlobalThemeQueryHandler : IRequestHandler<GetGlobalThemeQuery, Result<ThemeDto>>
{
    private readonly IThemeRepository _themeRepository;
    private readonly ILogger<GetGlobalThemeQueryHandler> _logger;

    public GetGlobalThemeQueryHandler(
        IThemeRepository themeRepository,
        ILogger<GetGlobalThemeQueryHandler> logger)
    {
        _themeRepository = themeRepository;
        _logger = logger;
    }

    public async Task<Result<ThemeDto>> Handle(GetGlobalThemeQuery request, CancellationToken cancellationToken)
    {
        var theme = await _themeRepository.GetGlobalThemeAsync(cancellationToken);
        _logger.LogDebug("Global theme retrieved successfully");
        
        return Result<ThemeDto>.Success(theme);
    }
}

