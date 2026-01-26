using MediatR;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Domain.Entities;

namespace Khidmah_Inventory.Application.Features.Pos.Commands;

public class OpenPosSessionCommand : IRequest<Result<Guid>>
{
    public decimal OpeningBalance { get; set; }
}

public class OpenPosSessionCommandHandler : IRequestHandler<OpenPosSessionCommand, Result<Guid>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMultiTenantService _multiTenantService;

    public OpenPosSessionCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService,
        IMultiTenantService multiTenantService)
    {
        _context = context;
        _currentUserService = currentUserService;
        _multiTenantService = multiTenantService;
    }

    public async Task<Result<Guid>> Handle(OpenPosSessionCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (userId == null) return Result<Guid>.Failure("User not authenticated.");

        var companyId = _multiTenantService.GetCurrentCompanyId();
        if (companyId == null) return Result<Guid>.Failure("Company context not found.");

        // Check if there's already an open session for this user
        var existingSession = _context.PosSessions
            .Any(s => s.UserId == userId && s.Status == "Open" && s.CompanyId == companyId.Value);

        if (existingSession)
        {
            return Result<Guid>.Failure("User already has an open POS session.");
        }

        var session = new PosSession(companyId.Value, userId.Value, request.OpeningBalance, userId);

        _context.PosSessions.Add(session);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(session.Id);
    }
}
