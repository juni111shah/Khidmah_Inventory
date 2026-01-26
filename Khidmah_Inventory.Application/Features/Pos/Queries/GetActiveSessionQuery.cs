using MediatR;
using Microsoft.EntityFrameworkCore;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;

namespace Khidmah_Inventory.Application.Features.Pos.Queries;

public class GetActiveSessionQuery : IRequest<Result<Guid?>>
{
}

public class GetActiveSessionQueryHandler : IRequestHandler<GetActiveSessionQuery, Result<Guid?>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetActiveSessionQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<Guid?>> Handle(GetActiveSessionQuery request, CancellationToken cancellationToken)
    {
        if (!_currentUser.UserId.HasValue)
            return Result<Guid?>.Failure("User not authenticated");

        var session = await _context.PosSessions
            .Where(s => s.UserId == _currentUser.UserId.Value && s.Status == "Open" && s.CompanyId == _currentUser.CompanyId)
            .Select(s => (Guid?)s.Id)
            .FirstOrDefaultAsync(cancellationToken);

        return Result<Guid?>.Success(session);
    }
}
