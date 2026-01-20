using MediatR;
using Microsoft.EntityFrameworkCore;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Roles.Models;

namespace Khidmah_Inventory.Application.Features.Permissions.Queries.GetPermissionsList;

public class GetPermissionsListQueryHandler : IRequestHandler<GetPermissionsListQuery, Result<List<PermissionDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetPermissionsListQueryHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<List<PermissionDto>>> Handle(GetPermissionsListQuery request, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId;
        if (!companyId.HasValue)
        {
            return Result<List<PermissionDto>>.Failure("Company context is required");
        }

        var query = _context.Permissions
            .Where(p => p.CompanyId == companyId.Value && !p.IsDeleted)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.Module))
        {
            query = query.Where(p => p.Module == request.Module);
        }

        var permissions = await query
            .OrderBy(p => p.Module)
            .ThenBy(p => p.Action)
            .ToListAsync(cancellationToken);

        var dtos = permissions.Select(p => new PermissionDto
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            Module = p.Module,
            Action = p.Action
        }).ToList();

        return Result<List<PermissionDto>>.Success(dtos);
    }
}

