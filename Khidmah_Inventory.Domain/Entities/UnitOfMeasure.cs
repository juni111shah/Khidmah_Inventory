using Khidmah_Inventory.Domain.Common;

namespace Khidmah_Inventory.Domain.Entities;

public class UnitOfMeasure : Entity
{
    public string Name { get; private set; } = string.Empty;
    public string Code { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public bool IsBaseUnit { get; private set; } = false;
    public decimal? ConversionFactor { get; private set; } // For conversions to base unit
    public Guid? BaseUnitId { get; private set; }

    // Navigation properties
    public virtual UnitOfMeasure? BaseUnit { get; private set; }

    private UnitOfMeasure() { }

    public UnitOfMeasure(
        Guid companyId,
        string name,
        string code,
        bool isBaseUnit = false,
        Guid? createdBy = null) : base(companyId, createdBy)
    {
        Name = name;
        Code = code;
        IsBaseUnit = isBaseUnit;
    }

    public void Update(
        string name,
        string code,
        string? description,
        bool isBaseUnit,
        decimal? conversionFactor,
        Guid? baseUnitId,
        Guid? updatedBy = null)
    {
        Name = name;
        Code = code;
        Description = description;
        IsBaseUnit = isBaseUnit;
        ConversionFactor = conversionFactor;
        BaseUnitId = baseUnitId;
        UpdateAuditInfo(updatedBy);
    }
}

