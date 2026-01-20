using MediatR;
using Microsoft.EntityFrameworkCore;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;

namespace Khidmah_Inventory.Application.Features.Auth.Commands.RefreshToken;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, Result<RefreshTokenResponseDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IIdentityService _identityService;

    public RefreshTokenCommandHandler(
        IApplicationDbContext context,
        IIdentityService identityService)
    {
        _context = context;
        _identityService = identityService;
    }

    public async Task<Result<RefreshTokenResponseDto>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        // Validate refresh token
        var user = await _context.Users
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                    .ThenInclude(r => r.RolePermissions)
                        .ThenInclude(rp => rp.Permission)
            .Include(u => u.UserCompanies)
            .FirstOrDefaultAsync(u => u.RefreshToken == request.RefreshToken && !u.IsDeleted, cancellationToken);

        if (user == null || !user.IsActive)
        {
            return Result<RefreshTokenResponseDto>.Failure("Invalid refresh token");
        }

        // Validate refresh token expiry
        var isValid = await _identityService.ValidateRefreshTokenAsync(
            request.RefreshToken,
            user.RefreshToken!,
            user.RefreshTokenExpiryTime);

        if (!isValid)
        {
            return Result<RefreshTokenResponseDto>.Failure("Refresh token expired");
        }

        // Get user's default company
        var defaultCompany = user.UserCompanies.FirstOrDefault(uc => uc.IsDefault && uc.IsActive);
        if (defaultCompany == null)
        {
            return Result<RefreshTokenResponseDto>.Failure("No active company found for user");
        }

        // Get roles and permissions
        var roles = user.UserRoles
            .Where(ur => ur.Role.CompanyId == defaultCompany.CompanyId)
            .Select(ur => ur.Role.Name)
            .ToList();

        var permissions = user.UserRoles
            .Where(ur => ur.Role.CompanyId == defaultCompany.CompanyId)
            .SelectMany(ur => ur.Role.RolePermissions)
            .Select(rp => rp.Permission.Name)
            .Distinct()
            .ToList();

        // Generate new tokens
        var newToken = await _identityService.GenerateJwtTokenAsync(
            user.Id,
            user.Email,
            defaultCompany.CompanyId,
            roles,
            permissions);

        var newRefreshToken = await _identityService.GenerateRefreshTokenAsync();
        user.SetRefreshToken(newRefreshToken, DateTime.UtcNow.AddDays(7));

        await _context.SaveChangesAsync(cancellationToken);

        var response = new RefreshTokenResponseDto
        {
            Token = newToken,
            RefreshToken = newRefreshToken,
            ExpiresAt = DateTime.UtcNow.AddHours(24)
        };

        return Result<RefreshTokenResponseDto>.Success(response);
    }
}

