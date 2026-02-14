using Khidmah_Inventory.Domain.Common;

namespace Khidmah_Inventory.Domain.Entities;

/// <summary>
/// Per-company integration toggle and config (Email, SMS, WhatsApp, Payment, BI, Ecommerce).
/// </summary>
public class CompanyIntegration : BaseEntity
{
    /// <summary>e.g. Email, SMS, WhatsApp, Payment, BI, Ecommerce.</summary>
    public string IntegrationType { get; private set; } = string.Empty;
    public bool IsEnabled { get; private set; }
    /// <summary>JSON config (API keys, endpoints - encrypted or stored securely in production).</summary>
    public string? ConfigJson { get; private set; }
    public string? DisplayName { get; private set; }
    public string? Description { get; private set; }

    private CompanyIntegration() { }

    public CompanyIntegration(Guid companyId, string integrationType, bool isEnabled, string? configJson = null, string? displayName = null, string? description = null, Guid? createdBy = null)
        : base(companyId, createdBy)
    {
        IntegrationType = integrationType;
        IsEnabled = isEnabled;
        ConfigJson = configJson;
        DisplayName = displayName ?? integrationType;
        Description = description;
    }

    public void SetEnabled(bool enabled, Guid? updatedBy = null)
    {
        IsEnabled = enabled;
        UpdateAuditInfo(updatedBy);
    }

    public void UpdateConfig(string? configJson, string? displayName, string? description, Guid? updatedBy = null)
    {
        ConfigJson = configJson;
        if (displayName != null) DisplayName = displayName;
        if (description != null) Description = description;
        UpdateAuditInfo(updatedBy);
    }
}
