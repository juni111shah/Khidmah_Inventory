using MediatR;
using Microsoft.EntityFrameworkCore;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Categories.Models;

namespace Khidmah_Inventory.Application.Features.Categories.Commands.UpdateCategory;

public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, Result<CategoryDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public UpdateCategoryCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<CategoryDto>> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId;
        if (!companyId.HasValue)
        {
            return Result<CategoryDto>.Failure("Company context is required");
        }

        var category = await _context.Categories
            .FirstOrDefaultAsync(c => c.Id == request.Id && c.CompanyId == companyId.Value && !c.IsDeleted, cancellationToken);

        if (category == null)
        {
            return Result<CategoryDto>.Failure("Category not found.");
        }

        // Prevent circular reference - category cannot be its own parent or descendant
        if (request.ParentCategoryId.HasValue && request.ParentCategoryId.Value == request.Id)
        {
            return Result<CategoryDto>.Failure("A category cannot be its own parent.");
        }

        if (request.ParentCategoryId.HasValue)
        {
            var isDescendant = await IsDescendantAsync(request.Id, request.ParentCategoryId.Value, companyId.Value, cancellationToken);
            if (isDescendant)
            {
                return Result<CategoryDto>.Failure("A category cannot be a parent of its own descendant.");
            }
        }

        // Check if code is unique (if provided and changed)
        if (!string.IsNullOrWhiteSpace(request.Code) && category.Code != request.Code)
        {
            var existingCategory = await _context.Categories
                .FirstOrDefaultAsync(c => c.CompanyId == companyId.Value && 
                    c.Code == request.Code && 
                    c.Id != request.Id && 
                    !c.IsDeleted, cancellationToken);

            if (existingCategory != null)
            {
                return Result<CategoryDto>.Failure("A category with this code already exists.");
            }
        }

        // Validate parent category if provided
        if (request.ParentCategoryId.HasValue)
        {
            var parentCategory = await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == request.ParentCategoryId.Value && 
                    c.CompanyId == companyId.Value && !c.IsDeleted, cancellationToken);

            if (parentCategory == null)
            {
                return Result<CategoryDto>.Failure("Parent category not found.");
            }
        }

        category.Update(
            request.Name,
            request.Description,
            request.Code,
            request.ParentCategoryId,
            request.DisplayOrder,
            _currentUser.UserId);

        await _context.SaveChangesAsync(cancellationToken);

        var dto = await MapToDtoAsync(category.Id, companyId.Value, cancellationToken);
        return Result<CategoryDto>.Success(dto);
    }

    private async Task<bool> IsDescendantAsync(Guid categoryId, Guid potentialParentId, Guid companyId, CancellationToken cancellationToken)
    {
        var current = await _context.Categories
            .FirstOrDefaultAsync(c => c.Id == potentialParentId && c.CompanyId == companyId, cancellationToken);

        while (current != null && current.ParentCategoryId.HasValue)
        {
            if (current.ParentCategoryId.Value == categoryId)
            {
                return true;
            }
            current = await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == current.ParentCategoryId.Value && c.CompanyId == companyId, cancellationToken);
        }

        return false;
    }

    private async Task<CategoryDto> MapToDtoAsync(Guid categoryId, Guid companyId, CancellationToken cancellationToken)
    {
        var category = await _context.Categories
            .Include(c => c.ParentCategory)
            .FirstOrDefaultAsync(c => c.Id == categoryId && c.CompanyId == companyId, cancellationToken);

        if (category == null)
        {
            throw new InvalidOperationException("Category not found after update");
        }

        var productCount = await _context.Products
            .CountAsync(p => p.CategoryId == categoryId && p.CompanyId == companyId && !p.IsDeleted, cancellationToken);

        var subCategoryCount = await _context.Categories
            .CountAsync(c => c.ParentCategoryId == categoryId && c.CompanyId == companyId && !c.IsDeleted, cancellationToken);

        return new CategoryDto
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
    }
}

