using Khidmah_Inventory.Domain.Common;

namespace Khidmah_Inventory.Domain.Entities;

public class AutomationRuleHistory : BaseEntity
{
    public Guid AutomationRuleId { get; private set; }
    public string Trigger { get; private set; } = string.Empty;
    public string? TriggerContextJson { get; private set; }
    public string ActionExecuted { get; private set; } = string.Empty;
    public bool Success { get; private set; }
    public string? ErrorMessage { get; private set; }

    public virtual AutomationRule? AutomationRule { get; private set; }

    private AutomationRuleHistory() { }

    public AutomationRuleHistory(
        Guid companyId,
        Guid automationRuleId,
        string trigger,
        string? triggerContextJson,
        string actionExecuted,
        bool success,
        string? errorMessage = null,
        Guid? createdBy = null) : base(companyId, createdBy)
    {
        AutomationRuleId = automationRuleId;
        Trigger = trigger;
        TriggerContextJson = triggerContextJson;
        ActionExecuted = actionExecuted;
        Success = success;
        ErrorMessage = errorMessage;
    }
}
