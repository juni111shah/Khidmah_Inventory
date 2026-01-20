using Khidmah_Inventory.Domain.Common;

namespace Khidmah_Inventory.Domain.Entities;

public class WarehouseZone : Entity
{
    public Guid WarehouseId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string? Code { get; private set; }
    public string? Description { get; private set; }
    public int DisplayOrder { get; private set; } = 0;

    // Navigation properties
    public virtual Warehouse Warehouse { get; private set; } = null!;
    public virtual ICollection<Bin> Bins { get; private set; } = new List<Bin>();

    private WarehouseZone() { }

    public WarehouseZone(
        Guid companyId,
        Guid warehouseId,
        string name,
        string? code = null,
        Guid? createdBy = null) : base(companyId, createdBy)
    {
        WarehouseId = warehouseId;
        Name = name;
        Code = code;
    }

    public void Update(
        string name,
        string? code,
        string? description,
        int displayOrder,
        Guid? updatedBy = null)
    {
        Name = name;
        Code = code;
        Description = description;
        DisplayOrder = displayOrder;
        UpdateAuditInfo(updatedBy);
    }
}

