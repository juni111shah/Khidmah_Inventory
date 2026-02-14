namespace Khidmah_Inventory.Application.Features.Copilot.Models;

public class CopilotIntentResult
{
    public string Intent { get; set; } = string.Empty;
    public string? Action { get; set; }
    public Dictionary<string, object?> Parameters { get; set; } = new();
    public bool RequiresConfirmation { get; set; }
    public string? ConfirmationMessage { get; set; }
}
