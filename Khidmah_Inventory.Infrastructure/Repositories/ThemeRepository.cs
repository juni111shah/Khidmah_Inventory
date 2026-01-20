using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Features.Theme.Models;
using System.Text.Json;

namespace Khidmah_Inventory.Infrastructure.Repositories;

public class ThemeRepository : IThemeRepository
{
    private readonly IWebHostEnvironment _environment;
    private readonly IFileStorageService _fileStorageService;
    private readonly IFileValidationService _fileValidationService;
    private readonly string _themesPath;
    private const string USER_THEME_FILE = "user-theme.json";
    private const string GLOBAL_THEME_FILE = "global-theme.json";
    private const string LOGOS_SUBDIRECTORY = "logos";

    public ThemeRepository(
        IWebHostEnvironment environment,
        IFileStorageService fileStorageService,
        IFileValidationService fileValidationService)
    {
        _environment = environment;
        _fileStorageService = fileStorageService;
        _fileValidationService = fileValidationService;
        _themesPath = Path.Combine(_environment.ContentRootPath, "Data", "Themes");
        Directory.CreateDirectory(_themesPath);
    }

    public Task<ThemeDto> GetUserThemeAsync(CancellationToken cancellationToken = default)
    {
        var theme = LoadTheme(Path.Combine(_themesPath, USER_THEME_FILE));
        return Task.FromResult(theme);
    }

    public Task<ThemeDto> GetGlobalThemeAsync(CancellationToken cancellationToken = default)
    {
        var theme = LoadTheme(Path.Combine(_themesPath, GLOBAL_THEME_FILE));
        return Task.FromResult(theme);
    }

    public Task SaveUserThemeAsync(ThemeDto theme, CancellationToken cancellationToken = default)
    {
        SaveTheme(theme, Path.Combine(_themesPath, USER_THEME_FILE));
        return Task.CompletedTask;
    }

    public Task SaveGlobalThemeAsync(ThemeDto theme, CancellationToken cancellationToken = default)
    {
        SaveTheme(theme, Path.Combine(_themesPath, GLOBAL_THEME_FILE));
        return Task.CompletedTask;
    }

    public async Task<string> SaveLogoAsync(IFormFile file, CancellationToken cancellationToken = default)
    {
        var allowedExtensions = _fileValidationService.GetAllowedImageExtensions();
        var maxSize = _fileValidationService.GetMaxFileSizeInBytes();
        
        _fileValidationService.ValidateFile(file, maxSize, allowedExtensions);

        return await _fileStorageService.SaveFileAsync(file, LOGOS_SUBDIRECTORY, cancellationToken);
    }

    private ThemeDto LoadTheme(string filePath)
    {
        if (!File.Exists(filePath))
        {
            return new ThemeDto(); // Return default theme
        }

        try
        {
            var json = File.ReadAllText(filePath);
            var theme = JsonSerializer.Deserialize<ThemeDto>(json);
            return theme ?? new ThemeDto();
        }
        catch
        {
            return new ThemeDto(); // Return default theme on error
        }
    }

    private void SaveTheme(ThemeDto theme, string filePath)
    {
        try
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            var json = JsonSerializer.Serialize(theme, options);
            File.WriteAllText(filePath, json);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to save theme: {ex.Message}", ex);
        }
    }
}

