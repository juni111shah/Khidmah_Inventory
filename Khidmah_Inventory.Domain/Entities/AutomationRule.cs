using Khidmah_Inventory.Domain.Common;

namespace Khidmah_Inventory.Domain.Entities;

public class AutomationRule : BaseEntity
{
    public string Name { get; private set; } = string.Empty;
    public string Trigger { get; private set; } = string.Empty; // StockBelowThreshold, POApproved, SaleCreated
    public string? ConditionJson { get; private set; }
    public string ActionJson { get; private set; } = string.Empty;
    public bool IsActive { get; private set; } = true;

    private AutomationRule() { }

    public AutomationRule(
        Guid companyId,
        string name,
        string trigger,
        string? conditionJson,
        string actionJson,
        Guid? createdBy = null) : base(companyId, createdBy)
    {
        Name = name;
        Trigger = trigger;
        ConditionJson = conditionJson;
        ActionJson = actionJson;
    }

    public void Update(string name, string? conditionJson, string actionJson, bool isActive, Guid? updatedBy = null)
    {
        Name = name;
        ConditionJson = conditionJson;
        ActionJson = actionJson;
        IsActive = isActive;
        UpdateAuditInfo(updatedBy);
    }

    public void SetActive(bool active, Guid? updatedBy = null)
    {
        IsActive = active;
        UpdateAuditInfo(updatedBy);
    }
}
