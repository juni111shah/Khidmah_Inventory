namespace Khidmah_Inventory.Domain.Common;

public abstract class BaseEntity
{
    public Guid Id { get; protected set; } = Guid.NewGuid();
    public Guid CompanyId { get; protected set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public Guid? CreatedBy { get; set; }
    public Guid? UpdatedBy { get; set; }
    public bool IsDeleted { get; protected set; } = false;
    public DateTime? DeletedAt { get; protected set; }
    public Guid? DeletedBy { get; protected set; }

    protected BaseEntity() { }

    protected BaseEntity(Guid companyId, Guid? createdBy = null)
    {
        CompanyId = companyId;
        CreatedBy = createdBy;
    }

    public void SetCompanyId(Guid companyId)
    {
        CompanyId = companyId;
    }

    public void MarkAsDeleted(Guid? deletedBy = null)
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = deletedBy;
    }

    public void Restore()
    {
        IsDeleted = false;
        DeletedAt = null;
        DeletedBy = null;
    }

    public void UpdateAuditInfo(Guid? updatedBy = null)
    {
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
    }
}

