using MediatR;
using Microsoft.EntityFrameworkCore;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Reports.Models;

namespace Khidmah_Inventory.Application.Features.Reports.Queries.GetCustomReportsList;

public class GetCustomReportsListQueryHandler : IRequestHandler<GetCustomReportsListQuery, Result<List<CustomReportDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetCustomReportsListQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<List<CustomReportDto>>> Handle(GetCustomReportsListQuery request, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId;
        if (!companyId.HasValue)
            return Result<List<CustomReportDto>>.Failure("Company context is required");

        var query = _context.CustomReports
            .Include(cr => cr.CreatedByUser)
            .Where(cr => cr.CompanyId == companyId.Value && !cr.IsDeleted);

        if (!string.IsNullOrWhiteSpace(request.ReportType))
            query = query.Where(cr => cr.ReportType == request.ReportType);

        if (request.IncludePublic == true)
            query = query.Where(cr => cr.IsPublic || cr.CreatedByUserId == _currentUser.UserId);
        else
            query = query.Where(cr => cr.CreatedByUserId == _currentUser.UserId);

        var reports = await query
            .OrderByDescending(cr => cr.CreatedAt)
            .Select(cr => new CustomReportDto
            {
                Id = cr.Id,
                Name = cr.Name,
                Description = cr.Description,
                ReportType = cr.ReportType,
                ReportDefinition = cr.ReportDefinition,
                IsPublic = cr.IsPublic,
                CreatedByUserId = cr.CreatedByUserId,
                CreatedByUserName = cr.CreatedByUser != null ? cr.CreatedByUser.UserName : null,
                CreatedAt = cr.CreatedAt
            })
            .ToListAsync(cancellationToken);

        return Result<List<CustomReportDto>>.Success(reports);
    }
}

