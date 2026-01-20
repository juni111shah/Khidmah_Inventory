using MediatR;
using Microsoft.EntityFrameworkCore;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Domain.Entities;

namespace Khidmah_Inventory.Application.Features.Auth.Commands.Register;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result<RegisterResponseDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IIdentityService _identityService;

    public RegisterCommandHandler(
        IApplicationDbContext context,
        IIdentityService identityService)
    {
        _context = context;
        _identityService = identityService;
    }

    public async Task<Result<RegisterResponseDto>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        // Check if email already exists
        var emailExists = await _context.Users
            .AnyAsync(u => u.Email == request.Email && !u.IsDeleted, cancellationToken);

        if (emailExists)
        {
            return Result<RegisterResponseDto>.Failure("Email already exists");
        }

        // Check if username already exists
        var usernameExists = await _context.Users
            .AnyAsync(u => u.UserName == request.UserName && !u.IsDeleted, cancellationToken);

        if (usernameExists)
        {
            return Result<RegisterResponseDto>.Failure("Username already exists");
        }

        // Verify company exists
        var companyExists = await _context.Companies
            .AnyAsync(c => c.Id == request.CompanyId && !c.IsDeleted && c.IsActive, cancellationToken);

        if (!companyExists)
        {
            return Result<RegisterResponseDto>.Failure("Company not found or inactive");
        }

        // Hash password
        var passwordHash = await _identityService.GeneratePasswordHashAsync(request.Password);

        // Create user
        var user = new User(
            request.CompanyId,
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
        var userCompany = new UserCompany(request.CompanyId, user.Id, isDefault: true);
        _context.UserCompanies.Add(userCompany);

        await _context.SaveChangesAsync(cancellationToken);

        return Result<RegisterResponseDto>.Success(new RegisterResponseDto
        {
            UserId = user.Id,
            Email = user.Email,
            UserName = user.UserName
        });
    }
}

