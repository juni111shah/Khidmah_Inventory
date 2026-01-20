namespace Khidmah_Inventory.Application.Features.Categories.Models;

public class CategoryDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Code { get; set; }
    public Guid? ParentCategoryId { get; set; }
    public string? ParentCategoryName { get; set; }
    public int DisplayOrder { get; set; }
    public string? ImageUrl { get; set; }
    public int ProductCount { get; set; }
    public int SubCategoryCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CategoryTreeDto : CategoryDto
{
    public List<CategoryTreeDto> SubCategories { get; set; } = new();
}

