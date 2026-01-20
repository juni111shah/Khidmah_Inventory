using Khidmah_Inventory.Domain.Common;

namespace Khidmah_Inventory.Domain.Entities;

public class ProductImage : BaseEntity
{
    public Guid ProductId { get; private set; }
    public string ImageUrl { get; private set; } = string.Empty;
    public string? AltText { get; private set; }
    public int DisplayOrder { get; private set; } = 0;
    public bool IsPrimary { get; private set; } = false;

    // Navigation properties
    public virtual Product Product { get; private set; } = null!;

    private ProductImage() { }

    public ProductImage(
        Guid companyId,
        Guid productId,
        string imageUrl,
        Guid? createdBy = null) : base(companyId, createdBy)
    {
        ProductId = productId;
        ImageUrl = imageUrl;
    }

    public void Update(
        string imageUrl,
        string? altText,
        int displayOrder,
        bool isPrimary,
        Guid? updatedBy = null)
    {
        ImageUrl = imageUrl;
        AltText = altText;
        DisplayOrder = displayOrder;
        IsPrimary = isPrimary;
        UpdateAuditInfo(updatedBy);
    }
}

