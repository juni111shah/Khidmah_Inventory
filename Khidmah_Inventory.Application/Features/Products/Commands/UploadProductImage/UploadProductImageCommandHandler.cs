using MediatR;
using Microsoft.EntityFrameworkCore;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Domain.Entities;

namespace Khidmah_Inventory.Application.Features.Products.Commands.UploadProductImage;

public class UploadProductImageCommandHandler : IRequestHandler<UploadProductImageCommand, Result<string>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IFileStorageService _fileStorage;
    private readonly IFileValidationService _fileValidation;

    public UploadProductImageCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        IFileStorageService fileStorage,
        IFileValidationService fileValidation)
    {
        _context = context;
        _currentUser = currentUser;
        _fileStorage = fileStorage;
        _fileValidation = fileValidation;
    }

    public async Task<Result<string>> Handle(UploadProductImageCommand request, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId;
        if (!companyId.HasValue)
            return Result<string>.Failure("Company context is required");

        // Validate file
        _fileValidation.ValidateFile(request.File, _fileValidation.GetMaxFileSizeInBytes(), _fileValidation.GetAllowedImageExtensions());

        // Get product
        var product = await _context.Products
            .FirstOrDefaultAsync(p => p.Id == request.ProductId && p.CompanyId == companyId.Value && !p.IsDeleted, cancellationToken);

        if (product == null)
            return Result<string>.Failure("Product not found");

        // Save file
        var imageUrl = await _fileStorage.SaveFileAsync(request.File, "products", cancellationToken);

        // Get next display order
        var maxOrder = await _context.ProductImages
            .Where(pi => pi.ProductId == request.ProductId)
            .MaxAsync(pi => (int?)pi.DisplayOrder, cancellationToken) ?? 0;

        // If this is primary, set all others to non-primary
        if (request.IsPrimary)
        {
            var existingPrimaryImages = await _context.ProductImages
                .Where(pi => pi.ProductId == request.ProductId && pi.IsPrimary)
                .ToListAsync(cancellationToken);

            foreach (var img in existingPrimaryImages)
            {
                img.Update(imageUrl, img.AltText, img.DisplayOrder, false, _currentUser.UserId);
            }
        }

        // Create product image
        var productImage = new ProductImage(
            companyId.Value,
            request.ProductId,
            imageUrl,
            _currentUser.UserId
        );

        productImage.Update(
            imageUrl,
            request.AltText,
            request.IsPrimary ? 0 : maxOrder + 1, // Primary images get order 0
            request.IsPrimary,
            _currentUser.UserId
        );

        _context.ProductImages.Add(productImage);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<string>.Success(imageUrl);
    }
}