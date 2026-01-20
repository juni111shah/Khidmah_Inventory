using Khidmah_Inventory.Application.Features.Settings.Models;

namespace Khidmah_Inventory.Application.Common.Interfaces;

public interface ISettingsRepository
{
    Task<T?> GetSettingsAsync<T>(Guid companyId, string settingsType, string settingsKey, CancellationToken cancellationToken = default) where T : class;
    Task SaveSettingsAsync<T>(Guid companyId, string settingsType, string settingsKey, T settings, string? description = null, CancellationToken cancellationToken = default) where T : class;
    Task<bool> DeleteSettingsAsync(Guid companyId, string settingsType, string settingsKey, CancellationToken cancellationToken = default);
    Task<List<SettingsInfoDto>> GetAllSettingsAsync(Guid companyId, string? settingsType = null, CancellationToken cancellationToken = default);
}

