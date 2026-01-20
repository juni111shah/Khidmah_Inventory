using MediatR;
using Microsoft.EntityFrameworkCore;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Categories.Models;
using Khidmah_Inventory.Domain.Entities;

namespace Khidmah_Inventory.Application.Features.Categories.Commands.CreateCategory;

public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, Result<CategoryDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public CreateCategoryCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<CategoryDto>> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId;
        if (!companyId.HasValue)
        {
            return Result<CategoryDto>.Failure("Company context is required");
        }

        // Check if code is unique (if provided)
        if (!string.IsNullOrWhiteSpace(request.Code))
        {
            var existingCategory = await _context.Categories
                .FirstOrDefaultAsync(c => c.CompanyId == companyId.Value && c.Code == request.Code && !c.IsDeleted, cancellationToken);

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

        var category = new Category(
            companyId.Value,
            request.Name,
            request.Code,
            request.ParentCategoryId,
            _currentUser.UserId);

        if (!string.IsNullOrWhiteSpace(request.Description))
        {
            category.Update(
                request.Name,
                request.Description,
                request.Code,
                request.ParentCategoryId,
                request.DisplayOrder,
                _currentUser.UserId);
        }
        else
        {
            category.Update(
                request.Name,
                null,
                request.Code,
                request.ParentCategoryId,
                request.DisplayOrder,
                _currentUser.UserId);
        }

        _context.Categories.Add(category);
        await _context.SaveChangesAsync(cancellationToken);

        var dto = await MapToDtoAsync(category.Id, companyId.Value, cancellationToken);
        return Result<CategoryDto>.Success(dto);
    }

    private async Task<CategoryDto> MapToDtoAsync(Guid categoryId, Guid companyId, CancellationToken cancellationToken)
    {
        var category = await _context.Categories
            .Include(c => c.ParentCategory)
            .FirstOrDefaultAsync(c => c.Id == categoryId && c.CompanyId == companyId, cancellationToken);

        if (category == null)
        {
            throw new InvalidOperationException("Category not found after creation");
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

