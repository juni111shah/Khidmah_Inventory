using MediatR;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;
using Microsoft.EntityFrameworkCore;

namespace Khidmah_Inventory.Application.Features.Pos.Commands;

public class ClosePosSessionCommand : IRequest<Result<bool>>
{
    public Guid SessionId { get; set; }
    public decimal ClosingBalance { get; set; }
}

public class ClosePosSessionCommandHandler : IRequestHandler<ClosePosSessionCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public ClosePosSessionCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Result<bool>> Handle(ClosePosSessionCommand request, CancellationToken cancellationToken)
    {
        var session = await _context.PosSessions
            .Include(s => s.SalesOrders)
            .FirstOrDefaultAsync(s => s.Id == request.SessionId, cancellationToken);

        if (session == null) return Result<bool>.Failure("Session not found.");
        if (session.Status == "Closed") return Result<bool>.Failure("Session is already closed.");

        var totalSales = session.SalesOrders.Sum(so => so.TotalAmount);
        var expectedBalance = session.OpeningBalance + totalSales;

        session.Close(request.ClosingBalance, expectedBalance, _currentUserService.UserId);

        await _context.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}
