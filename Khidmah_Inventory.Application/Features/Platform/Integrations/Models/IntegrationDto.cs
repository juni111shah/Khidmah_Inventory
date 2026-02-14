namespace Khidmah_Inventory.Application.Features.Platform.Integrations.Models;

public class IntegrationDto
{
    public string IntegrationType { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsEnabled { get; set; }
    public bool IsConfigured { get; set; }
}

public class AvailableIntegrationTypeDto
{
    public string Type { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string? Description { get; set; }
}
