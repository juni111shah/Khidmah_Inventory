using MediatR;
using Microsoft.EntityFrameworkCore;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;

namespace Khidmah_Inventory.Application.Features.Companies.Commands.UploadCompanyLogo;

public class UploadCompanyLogoCommandHandler : IRequestHandler<UploadCompanyLogoCommand, Result<string>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IFileStorageService _fileStorage;
    private readonly IFileValidationService _fileValidation;

    public UploadCompanyLogoCommandHandler(
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

    public async Task<Result<string>> Handle(UploadCompanyLogoCommand request, CancellationToken cancellationToken)
    {
        // Only company admins or system admins can upload company logos
        if (!_currentUser.HasPermission("Companies:Update") && _currentUser.CompanyId != request.CompanyId)
            return Result<string>.Failure("You don't have permission to upload this company's logo");

        // Validate file
        _fileValidation.ValidateFile(request.File, _fileValidation.GetMaxFileSizeInBytes(), _fileValidation.GetAllowedImageExtensions());

        // Get company
        var company = await _context.Companies.FindAsync(request.CompanyId);
        if (company == null)
            return Result<string>.Failure("Company not found");

        // Save file
        var logoUrl = await _fileStorage.SaveFileAsync(request.File, "companies", cancellationToken);

        // Update company logo
        company.UpdateLogo(logoUrl, _currentUser.UserId);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<string>.Success(logoUrl);
    }
}