using Khidmah_Inventory.Domain.Common;

namespace Khidmah_Inventory.Domain.Entities;

public class Category : Entity
{
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public string? Code { get; private set; }
    public Guid? ParentCategoryId { get; private set; }
    public int DisplayOrder { get; private set; } = 0;
    public string? ImageUrl { get; private set; }

    // Navigation properties
    public virtual Category? ParentCategory { get; private set; }
    public virtual ICollection<Category> SubCategories { get; private set; } = new List<Category>();
    public virtual ICollection<Product> Products { get; private set; } = new List<Product>();

    private Category() { }

    public Category(
        Guid companyId,
        string name,
        string? code = null,
        Guid? parentCategoryId = null,
        Guid? createdBy = null) : base(companyId, createdBy)
    {
        Name = name;
        Code = code;
        ParentCategoryId = parentCategoryId;
    }

    public void Update(
        string name,
        string? description,
        string? code,
        Guid? parentCategoryId,
        int displayOrder,
        Guid? updatedBy = null)
    {
        Name = name;
        Description = description;
        Code = code;
        ParentCategoryId = parentCategoryId;
        DisplayOrder = displayOrder;
        UpdateAuditInfo(updatedBy);
    }

    public void UpdateImage(string? imageUrl, Guid? updatedBy = null)
    {
        ImageUrl = imageUrl;
        UpdateAuditInfo(updatedBy);
    }
}

