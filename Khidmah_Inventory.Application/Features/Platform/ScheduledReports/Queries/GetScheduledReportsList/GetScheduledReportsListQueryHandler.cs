using MediatR;
using Microsoft.EntityFrameworkCore;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Platform.ScheduledReports.Models;

namespace Khidmah_Inventory.Application.Features.Platform.ScheduledReports.Queries.GetScheduledReportsList;

public class GetScheduledReportsListQueryHandler : IRequestHandler<GetScheduledReportsListQuery, Result<List<ScheduledReportDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetScheduledReportsListQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<List<ScheduledReportDto>>> Handle(GetScheduledReportsListQuery request, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId;
        if (!companyId.HasValue)
            return Result<List<ScheduledReportDto>>.Failure("Company context is required");

        var query = _context.ScheduledReports
            .Where(r => r.CompanyId == companyId.Value && !r.IsDeleted)
            .AsQueryable();
        if (request.IsActive.HasValue)
            query = query.Where(r => r.IsActive == request.IsActive.Value);

        var list = await query
            .OrderBy(r => r.Name)
            .Select(r => new ScheduledReportDto
            {
                Id = r.Id,
                Name = r.Name,
                ReportType = r.ReportType,
                Frequency = r.Frequency,
                CronExpression = r.CronExpression,
                RecipientsJson = r.RecipientsJson,
                LastRunAt = r.LastRunAt,
                NextRunAt = r.NextRunAt,
                IsActive = r.IsActive,
                CreatedAt = r.CreatedAt
            })
            .ToListAsync(cancellationToken);
        return Result<List<ScheduledReportDto>>.Success(list);
    }
}
