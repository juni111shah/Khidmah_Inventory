using Khidmah_Inventory.Domain.Common;

namespace Khidmah_Inventory.Domain.Entities;

public class ProductVariant : Entity
{
    public Guid ProductId { get; private set; }
    public string VariantType { get; private set; } = string.Empty; // Size, Color, etc.
    public string VariantValue { get; private set; } = string.Empty; // Small, Red, etc.
    public string? SKU { get; private set; }
    public string? Barcode { get; private set; }
    public decimal? AdditionalPrice { get; private set; }
    public bool IsActive { get; private set; } = true;

    // Navigation properties
    public virtual Product Product { get; private set; } = null!;

    private ProductVariant() { }

    public ProductVariant(
        Guid companyId,
        Guid productId,
        string variantType,
        string variantValue,
        Guid? createdBy = null) : base(companyId, createdBy)
    {
        ProductId = productId;
        VariantType = variantType;
        VariantValue = variantValue;
    }

    public void Update(
        string variantType,
        string variantValue,
        string? sku,
        string? barcode,
        decimal? additionalPrice,
        Guid? updatedBy = null)
    {
        VariantType = variantType;
        VariantValue = variantValue;
        SKU = sku;
        Barcode = barcode;
        AdditionalPrice = additionalPrice;
        UpdateAuditInfo(updatedBy);
    }

    public void Activate(Guid? updatedBy = null)
    {
        IsActive = true;
        UpdateAuditInfo(updatedBy);
    }

    public void Deactivate(Guid? updatedBy = null)
    {
        IsActive = false;
        UpdateAuditInfo(updatedBy);
    }
}

