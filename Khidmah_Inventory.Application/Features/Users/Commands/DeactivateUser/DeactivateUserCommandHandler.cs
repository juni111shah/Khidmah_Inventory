using MediatR;
using Microsoft.EntityFrameworkCore;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Users.Models;

namespace Khidmah_Inventory.Application.Features.Users.Commands.DeactivateUser;

public class DeactivateUserCommandHandler : IRequestHandler<DeactivateUserCommand, Result<UserDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public DeactivateUserCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<UserDto>> Handle(DeactivateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .Include(u => u.UserCompanies)
                .ThenInclude(uc => uc.Company)
            .FirstOrDefaultAsync(u => u.Id == request.Id && !u.IsDeleted, cancellationToken);

        if (user == null)
        {
            return Result<UserDto>.Failure("User not found");
        }

        // Prevent deactivating own account
        if (_currentUser.UserId == request.Id)
        {
            return Result<UserDto>.Failure("You cannot deactivate your own account");
        }

        var companyId = _currentUser.CompanyId;
        if (companyId.HasValue && !user.UserCompanies.Any(uc => uc.CompanyId == companyId.Value))
        {
            return Result<UserDto>.Failure("User not found in your company");
        }

        user.Deactivate(_currentUser.UserId);
        await _context.SaveChangesAsync(cancellationToken);

        // Reload to get updated data
        user = await _context.Users
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .Include(u => u.UserCompanies)
                .ThenInclude(uc => uc.Company)
            .FirstOrDefaultAsync(u => u.Id == request.Id, cancellationToken);

        var dto = MapToDto(user!, companyId);
        return Result<UserDto>.Success(dto);
    }

    private UserDto MapToDto(Domain.Entities.User user, Guid? companyId)
    {
        var roles = user.UserRoles
            .Where(ur => !companyId.HasValue || ur.Role.CompanyId == companyId.Value)
            .Select(ur => ur.Role.Name)
            .Distinct()
            .ToList();

        var companies = user.UserCompanies
            .Where(uc => !companyId.HasValue || uc.CompanyId == companyId.Value)
            .Select(uc => new CompanyDto
            {
                Id = uc.CompanyId,
                Name = uc.Company.Name,
                IsDefault = uc.IsDefault,
                IsActive = uc.IsActive
            })
            .ToList();

        var defaultCompany = user.UserCompanies.FirstOrDefault(uc => uc.IsDefault && uc.IsActive);

        return new UserDto
        {
            Id = user.Id,
            Email = user.Email,
            UserName = user.UserName,
            FirstName = user.FirstName,
            LastName = user.LastName,
            PhoneNumber = user.PhoneNumber,
            IsActive = user.IsActive,
            EmailConfirmed = user.EmailConfirmed,
            LastLoginAt = user.LastLoginAt,
            Roles = roles,
            Companies = companies,
            DefaultCompanyId = defaultCompany?.CompanyId,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt ?? user.CreatedAt
        };
    }
}

