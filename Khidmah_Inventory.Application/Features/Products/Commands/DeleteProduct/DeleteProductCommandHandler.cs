using MediatR;
using Microsoft.EntityFrameworkCore;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;

namespace Khidmah_Inventory.Application.Features.Products.Commands.DeleteProduct;

public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public DeleteProductCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId;
        if (!companyId.HasValue)
        {
            return Result.Failure("Company context is required");
        }

        var product = await _context.Products
            .FirstOrDefaultAsync(p => p.Id == request.Id && p.CompanyId == companyId.Value && !p.IsDeleted, cancellationToken);

        if (product == null)
        {
            return Result.Failure("Product not found.");
        }

        // Check if product has stock transactions (optional - you might want to allow deletion)
        var hasStockTransactions = await _context.StockTransactions
            .AnyAsync(st => st.ProductId == request.Id && st.CompanyId == companyId.Value, cancellationToken);

        if (hasStockTransactions)
        {
            return Result.Failure("Cannot delete product with stock transactions. Please deactivate the product instead.");
        }

        product.MarkAsDeleted(_currentUser.UserId);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

