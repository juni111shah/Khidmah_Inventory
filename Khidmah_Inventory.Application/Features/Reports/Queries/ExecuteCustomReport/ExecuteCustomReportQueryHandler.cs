using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Reports.Models;

namespace Khidmah_Inventory.Application.Features.Reports.Queries.ExecuteCustomReport;

public class ExecuteCustomReportQueryHandler : IRequestHandler<ExecuteCustomReportQuery, Result<object>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public ExecuteCustomReportQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<object>> Handle(ExecuteCustomReportQuery request, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId;
        if (!companyId.HasValue)
            return Result<object>.Failure("Company context is required");

        var report = await _context.CustomReports
            .FirstOrDefaultAsync(cr => cr.Id == request.ReportId && 
                cr.CompanyId == companyId.Value && 
                !cr.IsDeleted &&
                (cr.IsPublic || cr.CreatedByUserId == _currentUser.UserId), cancellationToken);

        if (report == null)
            return Result<object>.Failure("Report not found or access denied.");

        // Parse report definition
        var definition = JsonSerializer.Deserialize<ReportDefinitionDto>(report.ReportDefinition);
        if (definition == null)
            return Result<object>.Failure("Invalid report definition.");

        // Execute report based on type
        object reportData = report.ReportType switch
        {
            "Sales" => await ExecuteSalesReport(definition, request.Parameters, cancellationToken),
            "Inventory" => await ExecuteInventoryReport(definition, request.Parameters, cancellationToken),
            "Purchase" => await ExecutePurchaseReport(definition, request.Parameters, cancellationToken),
            _ => new { Error = "Unsupported report type" }
        };

        return Result<object>.Success(reportData);
    }

    private async Task<object> ExecuteSalesReport(ReportDefinitionDto definition, Dictionary<string, object>? parameters, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId!.Value;

        var query = _context.SalesOrders
            .Include(so => so.Customer)
            .Include(so => so.Items)
                .ThenInclude(item => item.Product)
            .Where(so => so.CompanyId == companyId && !so.IsDeleted)
            .AsQueryable();

        // Apply filters
        if (definition.Filters != null && definition.Filters.Any())
        {
            foreach (var filter in definition.Filters)
            {
                query = ApplyFilter(query, filter);
            }
        }

        // Apply grouping
        // Apply sorting
        // Select fields

        var data = await query.Take(1000).ToListAsync(cancellationToken);

        return new { Data = data, TotalCount = data.Count };
    }

    private async Task<object> ExecuteInventoryReport(ReportDefinitionDto definition, Dictionary<string, object>? parameters, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId!.Value;

        var query = _context.StockLevels
            .Include(sl => sl.Product)
            .Include(sl => sl.Warehouse)
            .Where(sl => sl.CompanyId == companyId)
            .AsQueryable();

        var data = await query.Take(1000).ToListAsync(cancellationToken);

        return new { Data = data, TotalCount = data.Count };
    }

    private async Task<object> ExecutePurchaseReport(ReportDefinitionDto definition, Dictionary<string, object>? parameters, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId!.Value;

        var query = _context.PurchaseOrders
            .Include(po => po.Supplier)
            .Include(po => po.Items)
                .ThenInclude(item => item.Product)
            .Where(po => po.CompanyId == companyId && !po.IsDeleted)
            .AsQueryable();

        var data = await query.Take(1000).ToListAsync(cancellationToken);

        return new { Data = data, TotalCount = data.Count };
    }

    private IQueryable<T> ApplyFilter<T>(IQueryable<T> query, ReportFilter filter)
    {
        // Simplified filter application
        // In a full implementation, this would use dynamic LINQ or expression trees
        return query;
    }
}

