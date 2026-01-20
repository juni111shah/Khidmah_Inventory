using Khidmah_Inventory.Domain.Common;

namespace Khidmah_Inventory.Domain.Entities;

public class Settings : BaseEntity
{
    public Settings(Guid companyId, string settingsType, string settingsKey, string jsonData)
    {
        CompanyId = companyId;
        SettingsType = settingsType;
        SettingsKey = settingsKey;
        JsonData = jsonData;
    }

    /// <summary>
    /// Type of settings: Company, User, System, Notification, UI, Report, Component
    /// </summary>
    public string SettingsType { get; private set; }

    /// <summary>
    /// Key to identify the settings (e.g., "company", "user-{userId}", "system")
    /// </summary>
    public string SettingsKey { get; private set; }

    /// <summary>
    /// JSON data containing the settings
    /// </summary>
    public string JsonData { get; private set; }

    /// <summary>
    /// Description of the settings
    /// </summary>
    public string? Description { get; private set; }

    /// <summary>
    /// Update the settings data
    /// </summary>
    public void UpdateData(string jsonData, string? description = null)
    {
        JsonData = jsonData;
        if (description != null)
        {
            Description = description;
        }
    }
}

