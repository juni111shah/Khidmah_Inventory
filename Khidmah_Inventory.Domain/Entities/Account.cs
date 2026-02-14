using Khidmah_Inventory.Domain.Common;

namespace Khidmah_Inventory.Domain.Entities;

public class Account : Entity
{
    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public AccountType Type { get; private set; }
    public Guid? ParentAccountId { get; private set; }
    public bool IsActive { get; private set; } = true;

    public virtual Account? Parent { get; private set; }
    public virtual ICollection<Account> Children { get; private set; } = new List<Account>();
    public virtual ICollection<JournalLine> JournalLines { get; private set; } = new List<JournalLine>();

    private Account() { }

    public Account(
        Guid companyId,
        string code,
        string name,
        AccountType type,
        Guid? parentAccountId = null,
        Guid? createdBy = null) : base(companyId, createdBy)
    {
        Code = code;
        Name = name;
        Type = type;
        ParentAccountId = parentAccountId;
    }

    public void Update(string code, string name, AccountType type, bool isActive, Guid? updatedBy = null)
    {
        Code = code;
        Name = name;
        Type = type;
        IsActive = isActive;
        UpdateAuditInfo(updatedBy);
    }

    public void Deactivate(Guid? updatedBy = null)
    {
        IsActive = false;
        UpdateAuditInfo(updatedBy);
    }

    public void Activate(Guid? updatedBy = null)
    {
        IsActive = true;
        UpdateAuditInfo(updatedBy);
    }
}
