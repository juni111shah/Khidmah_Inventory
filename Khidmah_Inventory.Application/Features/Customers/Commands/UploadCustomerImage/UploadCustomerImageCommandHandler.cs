using MediatR;
using Microsoft.EntityFrameworkCore;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;

namespace Khidmah_Inventory.Application.Features.Customers.Commands.UploadCustomerImage;

public class UploadCustomerImageCommandHandler : IRequestHandler<UploadCustomerImageCommand, Result<string>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IFileStorageService _fileStorage;
    private readonly IFileValidationService _fileValidation;

    public UploadCustomerImageCommandHandler(
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

    public async Task<Result<string>> Handle(UploadCustomerImageCommand request, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId;
        if (!companyId.HasValue)
            return Result<string>.Failure("Company context is required");

        // Validate file
        _fileValidation.ValidateFile(request.File, _fileValidation.GetMaxFileSizeInBytes(), _fileValidation.GetAllowedImageExtensions());

        // Get customer
        var customer = await _context.Customers
            .FirstOrDefaultAsync(c => c.Id == request.CustomerId && c.CompanyId == companyId.Value && !c.IsDeleted, cancellationToken);

        if (customer == null)
            return Result<string>.Failure("Customer not found");

        // Save file
        var imageUrl = await _fileStorage.SaveFileAsync(request.File, "customers", cancellationToken);

        // Update customer image
        customer.UpdateImage(imageUrl, _currentUser.UserId);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<string>.Success(imageUrl);
    }
}