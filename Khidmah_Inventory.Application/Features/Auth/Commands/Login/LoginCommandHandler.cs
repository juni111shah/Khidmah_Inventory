using MediatR;
using Microsoft.EntityFrameworkCore;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Domain.Entities;

namespace Khidmah_Inventory.Application.Features.Auth.Commands.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<LoginResponseDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IIdentityService _identityService;

    public LoginCommandHandler(
        IApplicationDbContext context,
        IIdentityService identityService)
    {
        _context = context;
        _identityService = identityService;
    }

    public async Task<Result<LoginResponseDto>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                    .ThenInclude(r => r.RolePermissions)
                        .ThenInclude(rp => rp.Permission)
            .Include(u => u.UserCompanies)
            .FirstOrDefaultAsync(u => u.Email == request.Email && !u.IsDeleted, cancellationToken);

        if (user == null || !user.IsActive)
        {
            return Result<LoginResponseDto>.Failure("Invalid email or password");
        }

        var isPasswordValid = await _identityService.VerifyPasswordAsync(request.Password, user.PasswordHash);
        if (!isPasswordValid)
        {
            return Result<LoginResponseDto>.Failure("Invalid email or password");
        }

        // Get user's default company
        var defaultCompany = user.UserCompanies.FirstOrDefault(uc => uc.IsDefault && uc.IsActive);
        if (defaultCompany == null)
        {
            return Result<LoginResponseDto>.Failure("No active company found for user");
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

        // Generate tokens
        var token = await _identityService.GenerateJwtTokenAsync(
            user.Id,
            user.Email,
            defaultCompany.CompanyId,
            roles,
            permissions);

        var refreshToken = await _identityService.GenerateRefreshTokenAsync();
        user.SetRefreshToken(refreshToken, DateTime.UtcNow.AddDays(7));
        user.RecordLogin();

        await _context.SaveChangesAsync(cancellationToken);

        var response = new LoginResponseDto
        {
            Token = token,
            RefreshToken = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddHours(24),
            User = new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                CompanyId = defaultCompany.CompanyId,
                Roles = roles,
                Permissions = permissions
            }
        };

        return Result<LoginResponseDto>.Success(response);
    }
}

