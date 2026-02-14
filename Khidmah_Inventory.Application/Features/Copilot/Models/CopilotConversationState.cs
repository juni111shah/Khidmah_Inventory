namespace Khidmah_Inventory.Application.Features.Copilot.Models;

public class CopilotConversationState
{
    public Guid SessionId { get; set; } = Guid.NewGuid();
    public string? CurrentTask { get; set; } // SalesOrder, PurchaseOrder, InventoryUpdate, PriceUpdate, StockQuery
    public Dictionary<string, string?> Fields { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    public int StepIndex { get; set; }
    public bool AwaitingConfirmation { get; set; }
    public string? LastQuestion { get; set; }
    public string? LastAssistantMessage { get; set; }
}
