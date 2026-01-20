using MediatR;
using Microsoft.EntityFrameworkCore;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Categories.Models;

namespace Khidmah_Inventory.Application.Features.Categories.Queries.GetCategory;

public class GetCategoryQueryHandler : IRequestHandler<GetCategoryQuery, Result<CategoryDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetCategoryQueryHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<CategoryDto>> Handle(GetCategoryQuery request, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId;
        if (!companyId.HasValue)
        {
            return Result<CategoryDto>.Failure("Company context is required");
        }

        var category = await _context.Categories
            .Include(c => c.ParentCategory)
            .FirstOrDefaultAsync(c => c.Id == request.Id && c.CompanyId == companyId.Value && !c.IsDeleted, cancellationToken);

        if (category == null)
        {
            return Result<CategoryDto>.Failure("Category not found.");
        }

        var productCount = await _context.Products
            .CountAsync(p => p.CategoryId == request.Id && p.CompanyId == companyId.Value && !p.IsDeleted, cancellationToken);

        var subCategoryCount = await _context.Categories
            .CountAsync(c => c.ParentCategoryId == request.Id && c.CompanyId == companyId.Value && !c.IsDeleted, cancellationToken);

        var dto = new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            Code = category.Code,
            ParentCategoryId = category.ParentCategoryId,
            ParentCategoryName = category.ParentCategory?.Name,
            DisplayOrder = category.DisplayOrder,
            ImageUrl = category.ImageUrl,
            ProductCount = productCount,
            SubCategoryCount = subCategoryCount,
            CreatedAt = category.CreatedAt,
            UpdatedAt = category.UpdatedAt
        };

        return Result<CategoryDto>.Success(dto);
    }
}

