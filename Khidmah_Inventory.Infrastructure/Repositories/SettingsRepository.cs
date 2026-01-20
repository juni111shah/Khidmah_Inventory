using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Features.Settings.Models;
using Khidmah_Inventory.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Khidmah_Inventory.Infrastructure.Repositories;

public class SettingsRepository : ISettingsRepository
{
    private readonly IApplicationDbContext _context;

    public SettingsRepository(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<T?> GetSettingsAsync<T>(Guid companyId, string settingsType, string settingsKey, CancellationToken cancellationToken = default) where T : class
    {
        var settings = await _context.Settings
            .Where(s => s.CompanyId == companyId 
                && s.SettingsType == settingsType 
                && s.SettingsKey == settingsKey 
                && !s.IsDeleted)
            .FirstOrDefaultAsync(cancellationToken);

        if (settings == null)
            return null;

        try
        {
            return JsonSerializer.Deserialize<T>(settings.JsonData, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }
        catch
        {
            return null;
        }
    }

    public async Task SaveSettingsAsync<T>(Guid companyId, string settingsType, string settingsKey, T settings, string? description = null, CancellationToken cancellationToken = default) where T : class
    {
        var jsonData = JsonSerializer.Serialize(settings, new JsonSerializerOptions
        {
            WriteIndented = false
        });

        var existingSettings = await _context.Settings
            .Where(s => s.CompanyId == companyId 
                && s.SettingsType == settingsType 
                && s.SettingsKey == settingsKey 
                && !s.IsDeleted)
            .FirstOrDefaultAsync(cancellationToken);

        if (existingSettings != null)
        {
            existingSettings.UpdateData(jsonData, description);
            existingSettings.UpdateAuditInfo();
        }
        else
        {
            var newSettings = new Settings(companyId, settingsType, settingsKey, jsonData);
            if (!string.IsNullOrEmpty(description))
            {
                newSettings.UpdateData(jsonData, description);
            }
            _context.Settings.Add(newSettings);
        }

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> DeleteSettingsAsync(Guid companyId, string settingsType, string settingsKey, CancellationToken cancellationToken = default)
    {
        var settings = await _context.Settings
            .Where(s => s.CompanyId == companyId 
                && s.SettingsType == settingsType 
                && s.SettingsKey == settingsKey 
                && !s.IsDeleted)
            .FirstOrDefaultAsync(cancellationToken);

        if (settings == null)
            return false;

        settings.MarkAsDeleted();
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<List<SettingsInfoDto>> GetAllSettingsAsync(Guid companyId, string? settingsType = null, CancellationToken cancellationToken = default)
    {
        var query = _context.Settings
            .Where(s => s.CompanyId == companyId && !s.IsDeleted);

        if (!string.IsNullOrEmpty(settingsType))
        {
            query = query.Where(s => s.SettingsType == settingsType);
        }

        return await query
            .Select(s => new SettingsInfoDto
            {
                Id = s.Id,
                SettingsType = s.SettingsType,
                SettingsKey = s.SettingsKey,
                Description = s.Description,
                CreatedAt = s.CreatedAt,
                UpdatedAt = s.UpdatedAt
            })
            .ToListAsync(cancellationToken);
    }
}

