using MediatR;
using Microsoft.EntityFrameworkCore;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Domain.Entities;
using Khidmah_Inventory.Application.Features.Roles.Commands.AssignRoleToUser;

namespace Khidmah_Inventory.Application.Features.Users.Commands.CreateUser;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Result<string>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IIdentityService _identityService;
    private readonly IMediator _mediator;

    public CreateUserCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        IIdentityService identityService,
        IMediator mediator)
    {
        _context = context;
        _currentUser = currentUser;
        _identityService = identityService;
        _mediator = mediator;
    }

    public async Task<Result<string>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var currentUserCompanyId = _currentUser.CompanyId;
        if (!currentUserCompanyId.HasValue)
            return Result<string>.Failure("Company context is required");

        // Use provided company ID or current user's company
        var targetCompanyId = request.CompanyId ?? currentUserCompanyId.Value;

        // Verify target company exists and is active
        var companyExists = await _context.Companies
            .AnyAsync(c => c.Id == targetCompanyId && !c.IsDeleted && c.IsActive, cancellationToken);

        if (!companyExists)
            return Result<string>.Failure("Target company not found or inactive");

        // Check if email already exists
        var emailExists = await _context.Users
            .AnyAsync(u => u.Email == request.Email && !u.IsDeleted, cancellationToken);

        if (emailExists)
            return Result<string>.Failure("Email already exists");

        // Check if username already exists
        var usernameExists = await _context.Users
            .AnyAsync(u => u.UserName == request.UserName && !u.IsDeleted, cancellationToken);

        if (usernameExists)
            return Result<string>.Failure("Username already exists");

        // Hash password
        var passwordHash = await _identityService.GeneratePasswordHashAsync(request.Password);

        // Create user
        var user = new User(
            targetCompanyId,
            request.Email,
            request.UserName,
            passwordHash,
            request.FirstName,
            request.LastName,
            null);

        if (!string.IsNullOrEmpty(request.PhoneNumber))
        {
            user.UpdateProfile(request.FirstName, request.LastName, request.PhoneNumber);
        }

        _context.Users.Add(user);

        // Create user-company relationship
        var userCompany = new UserCompany(targetCompanyId, user.Id, isDefault: true);
        _context.UserCompanies.Add(userCompany);

        await _context.SaveChangesAsync(cancellationToken);

        // Assign roles if provided
        if (request.Roles != null && request.Roles.Any())
        {
            foreach (var roleName in request.Roles)
            {
                // Find role by name
                var role = await _context.Roles
                    .FirstOrDefaultAsync(r => r.Name == roleName && r.CompanyId == targetCompanyId, cancellationToken);

                if (role != null)
                {
                    var assignRoleCommand = new AssignRoleToUserCommand
                    {
                        UserId = user.Id,
                        RoleId = role.Id
                    };

                    var roleResult = await _mediator.Send(assignRoleCommand, cancellationToken);
                    if (!roleResult.Succeeded)
                    {
                        // Log warning but don't fail the entire operation
                        // User is created but role assignment failed
                    }
                }
            }
        }

        return Result<string>.Success(user.Id.ToString());
    }
}