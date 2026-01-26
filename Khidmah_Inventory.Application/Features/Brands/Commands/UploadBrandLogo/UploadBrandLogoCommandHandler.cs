using MediatR;
using Microsoft.EntityFrameworkCore;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;

namespace Khidmah_Inventory.Application.Features.Brands.Commands.UploadBrandLogo;

public class UploadBrandLogoCommandHandler : IRequestHandler<UploadBrandLogoCommand, Result<string>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IFileStorageService _fileStorage;
    private readonly IFileValidationService _fileValidation;

    public UploadBrandLogoCommandHandler(
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

    public async Task<Result<string>> Handle(UploadBrandLogoCommand request, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId;
        if (!companyId.HasValue)
            return Result<string>.Failure("Company context is required");

        // Validate file
        _fileValidation.ValidateFile(request.File, _fileValidation.GetMaxFileSizeInBytes(), _fileValidation.GetAllowedImageExtensions());

        // Get brand
        var brand = await _context.Brands
            .FirstOrDefaultAsync(b => b.Id == request.BrandId && b.CompanyId == companyId.Value, cancellationToken);

        if (brand == null)
            return Result<string>.Failure("Brand not found");

        // Save file
        var logoUrl = await _fileStorage.SaveFileAsync(request.File, "brands", cancellationToken);

        // Update brand logo
        brand.UpdateLogo(logoUrl, _currentUser.UserId);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<string>.Success(logoUrl);
    }
}