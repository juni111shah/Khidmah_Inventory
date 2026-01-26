using MediatR;
using Microsoft.EntityFrameworkCore;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;

namespace Khidmah_Inventory.Application.Features.Categories.Commands.UploadCategoryImage;

public class UploadCategoryImageCommandHandler : IRequestHandler<UploadCategoryImageCommand, Result<string>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IFileStorageService _fileStorage;
    private readonly IFileValidationService _fileValidation;

    public UploadCategoryImageCommandHandler(
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

    public async Task<Result<string>> Handle(UploadCategoryImageCommand request, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId;
        if (!companyId.HasValue)
            return Result<string>.Failure("Company context is required");

        // Validate file
        _fileValidation.ValidateFile(request.File, _fileValidation.GetMaxFileSizeInBytes(), _fileValidation.GetAllowedImageExtensions());

        // Get category
        var category = await _context.Categories
            .FirstOrDefaultAsync(c => c.Id == request.CategoryId && c.CompanyId == companyId.Value, cancellationToken);

        if (category == null)
            return Result<string>.Failure("Category not found");

        // Save file
        var imageUrl = await _fileStorage.SaveFileAsync(request.File, "categories", cancellationToken);

        // Update category image
        category.UpdateImage(imageUrl, _currentUser.UserId);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<string>.Success(imageUrl);
    }
}