namespace Khidmah_Inventory.Application.Features.Settings.Models;

public class SettingsInfoDto
{
    public Guid Id { get; set; }
    public string SettingsType { get; set; } = string.Empty;
    public string SettingsKey { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

