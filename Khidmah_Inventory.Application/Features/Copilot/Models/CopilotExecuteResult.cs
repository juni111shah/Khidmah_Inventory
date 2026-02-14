namespace Khidmah_Inventory.Application.Features.Copilot.Models;

public class CopilotExecuteResult
{
    public bool Success { get; set; }
    public string? Action { get; set; }
    public string? Reply { get; set; }
    public string? NextQuestion { get; set; }
    public string? ConfirmationMessage { get; set; }
    public bool Completed { get; set; }
    public bool Cancelled { get; set; }
    public CopilotConversationState? SessionState { get; set; }
    public object? Result { get; set; }
    public string[]? Errors { get; set; }
}
