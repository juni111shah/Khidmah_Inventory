using MediatR;
using Microsoft.EntityFrameworkCore;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;

namespace Khidmah_Inventory.Application.Features.Users.Commands.UploadUserAvatar;

public class UploadUserAvatarCommandHandler : IRequestHandler<UploadUserAvatarCommand, Result<string>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IFileStorageService _fileStorage;
    private readonly IFileValidationService _fileValidation;

    public UploadUserAvatarCommandHandler(
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

    public async Task<Result<string>> Handle(UploadUserAvatarCommand request, CancellationToken cancellationToken)
    {
        // Validate file
        _fileValidation.ValidateFile(request.File, _fileValidation.GetMaxFileSizeInBytes(), _fileValidation.GetAllowedImageExtensions());

        // Get user
        var user = await _context.Users.FindAsync(request.UserId);
        if (user == null)
            return Result<string>.Failure("User not found");

        // Check permissions - users can upload their own avatar, admins can upload any
        if (_currentUser.UserId != request.UserId && !_currentUser.HasPermission("Users:Update"))
            return Result<string>.Failure("You don't have permission to upload this user's avatar");

        // Save file
        var avatarUrl = await _fileStorage.SaveFileAsync(request.File, "avatars", cancellationToken);

        // Update user avatar
        user.UpdateAvatar(avatarUrl, _currentUser.UserId);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<string>.Success(avatarUrl);
    }
}