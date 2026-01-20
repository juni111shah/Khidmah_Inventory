using MediatR;
using Microsoft.EntityFrameworkCore;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Categories.Models;

namespace Khidmah_Inventory.Application.Features.Categories.Queries.GetCategoryTree;

public class GetCategoryTreeQueryHandler : IRequestHandler<GetCategoryTreeQuery, Result<List<CategoryTreeDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetCategoryTreeQueryHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<List<CategoryTreeDto>>> Handle(GetCategoryTreeQuery request, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId;
        if (!companyId.HasValue)
        {
            return Result<List<CategoryTreeDto>>.Failure("Company context is required");
        }

        var categories = await _context.Categories
            .Include(c => c.ParentCategory)
            .Where(c => c.CompanyId == companyId.Value && !c.IsDeleted)
            .OrderBy(c => c.DisplayOrder)
            .ThenBy(c => c.Name)
            .ToListAsync(cancellationToken);

        // Build tree structure
        var categoryDict = categories.ToDictionary(c => c.Id, c => new CategoryTreeDto
        {
            Id = c.Id,
            Name = c.Name,
            Description = c.Description,
            Code = c.Code,
            ParentCategoryId = c.ParentCategoryId,
            ParentCategoryName = c.ParentCategory?.Name,
            DisplayOrder = c.DisplayOrder,
            ImageUrl = c.ImageUrl,
            CreatedAt = c.CreatedAt,
            UpdatedAt = c.UpdatedAt,
            SubCategories = new List<CategoryTreeDto>()
        });

        // Get product and subcategory counts
        foreach (var category in categories)
        {
            var dto = categoryDict[category.Id];
            dto.ProductCount = await _context.Products
                .CountAsync(p => p.CategoryId == category.Id && p.CompanyId == companyId.Value && !p.IsDeleted, cancellationToken);
            dto.SubCategoryCount = await _context.Categories
                .CountAsync(c => c.ParentCategoryId == category.Id && c.CompanyId == companyId.Value && !c.IsDeleted, cancellationToken);
        }

        // Build tree
        var rootCategories = new List<CategoryTreeDto>();
        foreach (var category in categories)
        {
            var dto = categoryDict[category.Id];
            if (category.ParentCategoryId == null)
            {
                rootCategories.Add(dto);
            }
            else if (categoryDict.ContainsKey(category.ParentCategoryId.Value))
            {
                categoryDict[category.ParentCategoryId.Value].SubCategories.Add(dto);
            }
        }

        return Result<List<CategoryTreeDto>>.Success(rootCategories);
    }
}

