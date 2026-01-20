using MediatR;
using Microsoft.EntityFrameworkCore;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Products.Models;

namespace Khidmah_Inventory.Application.Features.Products.Queries.GetProduct;

public class GetProductQueryHandler : IRequestHandler<GetProductQuery, Result<ProductDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetProductQueryHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<ProductDto>> Handle(GetProductQuery request, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId;
        if (!companyId.HasValue)
        {
            return Result<ProductDto>.Failure("Company context is required");
        }

        var product = await _context.Products
            .Include(p => p.Category)
            .Include(p => p.Brand)
            .Include(p => p.UnitOfMeasure)
            .FirstOrDefaultAsync(p => p.Id == request.Id && p.CompanyId == companyId.Value && !p.IsDeleted, cancellationToken);

        if (product == null)
        {
            return Result<ProductDto>.Failure("Product not found.");
        }

        var dto = new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            SKU = product.SKU,
            Barcode = product.Barcode,
            CategoryId = product.CategoryId,
            CategoryName = product.Category?.Name,
            BrandId = product.BrandId,
            BrandName = product.Brand?.Name,
            UnitOfMeasureId = product.UnitOfMeasureId,
            UnitOfMeasureName = product.UnitOfMeasure.Name,
            UnitOfMeasureCode = product.UnitOfMeasure.Code,
            PurchasePrice = product.PurchasePrice,
            SalePrice = product.SalePrice,
            CostPrice = product.CostPrice,
            MinStockLevel = product.MinStockLevel,
            MaxStockLevel = product.MaxStockLevel,
            ReorderPoint = product.ReorderPoint,
            TrackQuantity = product.TrackQuantity,
            TrackBatch = product.TrackBatch,
            TrackExpiry = product.TrackExpiry,
            IsActive = product.IsActive,
            ImageUrl = product.ImageUrl,
            Weight = product.Weight,
            WeightUnit = product.WeightUnit,
            Length = product.Length,
            Width = product.Width,
            Height = product.Height,
            DimensionsUnit = product.DimensionsUnit,
            CreatedAt = product.CreatedAt,
            UpdatedAt = product.UpdatedAt
        };

        return Result<ProductDto>.Success(dto);
    }
}

