using Microsoft.AspNetCore.Http;
using Khidmah_Inventory.Application.Features.Theme.Models;

namespace Khidmah_Inventory.Application.Common.Interfaces;

public interface IThemeRepository
{
    Task<ThemeDto> GetUserThemeAsync(CancellationToken cancellationToken = default);
    Task<ThemeDto> GetGlobalThemeAsync(CancellationToken cancellationToken = default);
    Task SaveUserThemeAsync(ThemeDto theme, CancellationToken cancellationToken = default);
    Task SaveGlobalThemeAsync(ThemeDto theme, CancellationToken cancellationToken = default);
    Task<string> SaveLogoAsync(IFormFile file, CancellationToken cancellationToken = default);
}

