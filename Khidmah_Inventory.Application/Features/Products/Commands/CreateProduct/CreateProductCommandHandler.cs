using MediatR;
using Microsoft.EntityFrameworkCore;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Products.Models;
using Khidmah_Inventory.Domain.Entities;

namespace Khidmah_Inventory.Application.Features.Products.Commands.CreateProduct;

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Result<ProductDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public CreateProductCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<ProductDto>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId;
        if (!companyId.HasValue)
        {
            return Result<ProductDto>.Failure("Company context is required");
        }

        // Check if SKU is unique
        var existingProduct = await _context.Products
            .FirstOrDefaultAsync(p => p.CompanyId == companyId.Value && p.SKU == request.SKU && !p.IsDeleted, cancellationToken);

        if (existingProduct != null)
        {
            return Result<ProductDto>.Failure("A product with this SKU already exists.");
        }

        // Check if barcode is unique (if provided)
        if (!string.IsNullOrWhiteSpace(request.Barcode))
        {
            var existingBarcode = await _context.Products
                .FirstOrDefaultAsync(p => p.CompanyId == companyId.Value && p.Barcode == request.Barcode && !p.IsDeleted, cancellationToken);

            if (existingBarcode != null)
            {
                return Result<ProductDto>.Failure("A product with this barcode already exists.");
            }
        }

        // Validate UnitOfMeasure exists
        var unitOfMeasure = await _context.UnitOfMeasures
            .FirstOrDefaultAsync(u => u.Id == request.UnitOfMeasureId && u.CompanyId == companyId.Value && !u.IsDeleted, cancellationToken);

        if (unitOfMeasure == null)
        {
            return Result<ProductDto>.Failure("Unit of measure not found.");
        }

        // Validate Category if provided
        if (request.CategoryId.HasValue)
        {
            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == request.CategoryId.Value && c.CompanyId == companyId.Value && !c.IsDeleted, cancellationToken);

            if (category == null)
            {
                return Result<ProductDto>.Failure("Category not found.");
            }
        }

        // Validate Brand if provided
        if (request.BrandId.HasValue)
        {
            var brand = await _context.Brands
                .FirstOrDefaultAsync(b => b.Id == request.BrandId.Value && b.CompanyId == companyId.Value && !b.IsDeleted, cancellationToken);

            if (brand == null)
            {
                return Result<ProductDto>.Failure("Brand not found.");
            }
        }

        var product = new Product(
            companyId.Value,
            request.Name,
            request.SKU,
            request.UnitOfMeasureId,
            request.PurchasePrice,
            request.SalePrice,
            request.CategoryId,
            request.BrandId,
            _currentUser.UserId);

        product.Update(
            request.Name,
            request.Description,
            request.Barcode,
            request.CategoryId,
            request.BrandId,
            request.PurchasePrice,
            request.SalePrice,
            request.CostPrice,
            request.MinStockLevel,
            request.MaxStockLevel,
            request.ReorderPoint,
            request.TrackQuantity,
            request.TrackBatch,
            request.TrackExpiry,
            _currentUser.UserId);

        // Set additional properties
        if (!string.IsNullOrWhiteSpace(request.ImageUrl))
        {
            // ImageUrl would be set via a separate command for file upload
        }

        _context.Products.Add(product);
        await _context.SaveChangesAsync(cancellationToken);

        var dto = await MapToDtoAsync(product.Id, companyId.Value, cancellationToken);
        return Result<ProductDto>.Success(dto);
    }

    private async Task<ProductDto> MapToDtoAsync(Guid productId, Guid companyId, CancellationToken cancellationToken)
    {
        var product = await _context.Products
            .Include(p => p.Category)
            .Include(p => p.Brand)
            .Include(p => p.UnitOfMeasure)
            .FirstOrDefaultAsync(p => p.Id == productId && p.CompanyId == companyId, cancellationToken);

        if (product == null)
        {
            throw new InvalidOperationException("Product not found after creation");
        }

        return new ProductDto
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
    }
}

