using MediatR;
using Microsoft.EntityFrameworkCore;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;

namespace Khidmah_Inventory.Application.Features.Categories.Commands.DeleteCategory;

public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public DeleteCategoryCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId;
        if (!companyId.HasValue)
        {
            return Result.Failure("Company context is required");
        }

        var category = await _context.Categories
            .FirstOrDefaultAsync(c => c.Id == request.Id && c.CompanyId == companyId.Value && !c.IsDeleted, cancellationToken);

        if (category == null)
        {
            return Result.Failure("Category not found.");
        }

        // Check if category has subcategories
        var hasSubCategories = await _context.Categories
            .AnyAsync(c => c.ParentCategoryId == request.Id && c.CompanyId == companyId.Value && !c.IsDeleted, cancellationToken);

        if (hasSubCategories)
        {
            return Result.Failure("Cannot delete category with subcategories. Please delete or move subcategories first.");
        }

        // Check if category has products
        var hasProducts = await _context.Products
            .AnyAsync(p => p.CategoryId == request.Id && p.CompanyId == companyId.Value && !p.IsDeleted, cancellationToken);

        if (hasProducts)
        {
            return Result.Failure("Cannot delete category with products. Please remove or reassign products first.");
        }

        category.MarkAsDeleted(_currentUser.UserId);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

