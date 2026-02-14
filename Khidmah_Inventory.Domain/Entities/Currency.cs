using Khidmah_Inventory.Domain.Common;

namespace Khidmah_Inventory.Domain.Entities;

public class Currency : Entity
{
    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string Symbol { get; private set; } = string.Empty;
    public bool IsBase { get; private set; }

    private Currency() { }

    public Currency(
        Guid companyId,
        string code,
        string name,
        string symbol,
        bool isBase = false,
        Guid? createdBy = null) : base(companyId, createdBy)
    {
        Code = code;
        Name = name;
        Symbol = symbol;
        IsBase = isBase;
    }

    public void Update(string code, string name, string symbol, bool isBase, Guid? updatedBy = null)
    {
        Code = code;
        Name = name;
        Symbol = symbol;
        IsBase = isBase;
        UpdateAuditInfo(updatedBy);
    }

    public void SetAsBase(Guid? updatedBy = null)
    {
        IsBase = true;
        UpdateAuditInfo(updatedBy);
    }

    public void UnsetAsBase(Guid? updatedBy = null)
    {
        IsBase = false;
        UpdateAuditInfo(updatedBy);
    }
}
