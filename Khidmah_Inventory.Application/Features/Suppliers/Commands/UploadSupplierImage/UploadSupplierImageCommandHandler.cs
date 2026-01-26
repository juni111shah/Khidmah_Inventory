using MediatR;
using Microsoft.EntityFrameworkCore;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;

namespace Khidmah_Inventory.Application.Features.Suppliers.Commands.UploadSupplierImage;

public class UploadSupplierImageCommandHandler : IRequestHandler<UploadSupplierImageCommand, Result<string>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IFileStorageService _fileStorage;
    private readonly IFileValidationService _fileValidation;

    public UploadSupplierImageCommandHandler(
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

    public async Task<Result<string>> Handle(UploadSupplierImageCommand request, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId;
        if (!companyId.HasValue)
            return Result<string>.Failure("Company context is required");

        // Validate file
        _fileValidation.ValidateFile(request.File, _fileValidation.GetMaxFileSizeInBytes(), _fileValidation.GetAllowedImageExtensions());

        // Get supplier
        var supplier = await _context.Suppliers
            .FirstOrDefaultAsync(s => s.Id == request.SupplierId && s.CompanyId == companyId.Value && !s.IsDeleted, cancellationToken);

        if (supplier == null)
            return Result<string>.Failure("Supplier not found");

        // Save file
        var imageUrl = await _fileStorage.SaveFileAsync(request.File, "suppliers", cancellationToken);

        // Update supplier image
        supplier.UpdateImage(imageUrl, _currentUser.UserId);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<string>.Success(imageUrl);
    }
}