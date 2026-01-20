using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Reports.Models;
using Khidmah_Inventory.Domain.Entities;

namespace Khidmah_Inventory.Application.Features.Reports.Commands.SaveCustomReport;

public class SaveCustomReportCommandHandler : IRequestHandler<SaveCustomReportCommand, Result<CustomReportDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public SaveCustomReportCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<CustomReportDto>> Handle(SaveCustomReportCommand request, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId;
        if (!companyId.HasValue)
            return Result<CustomReportDto>.Failure("Company context is required");

        // Validate report definition JSON
        try
        {
            JsonSerializer.Deserialize<ReportDefinitionDto>(request.ReportDefinition);
        }
        catch
        {
            return Result<CustomReportDto>.Failure("Invalid report definition format.");
        }

        CustomReport report;

        if (request.Id.HasValue)
        {
            // Update existing report
            report = await _context.CustomReports
                .FirstOrDefaultAsync(cr => cr.Id == request.Id.Value && 
                    cr.CompanyId == companyId.Value && 
                    !cr.IsDeleted, cancellationToken);

            if (report == null)
                return Result<CustomReportDto>.Failure("Report not found.");

            // Check if user has permission to update (creator or admin)
            if (report.CreatedByUserId != _currentUser.UserId)
                return Result<CustomReportDto>.Failure("You don't have permission to update this report.");

            report.Update(request.Name, request.Description, request.ReportDefinition, request.IsPublic, _currentUser.UserId);
        }
        else
        {
            // Create new report
            report = new CustomReport(
                companyId.Value,
                request.Name,
                request.ReportType,
                request.ReportDefinition,
                _currentUser.UserId);

            report.Update(request.Name, request.Description, request.ReportDefinition, request.IsPublic, _currentUser.UserId);
            _context.CustomReports.Add(report);
        }

        await _context.SaveChangesAsync(cancellationToken);

        // Load with navigation properties
        var reportWithNav = await _context.CustomReports
            .Include(cr => cr.CreatedByUser)
            .FirstOrDefaultAsync(cr => cr.Id == report.Id && cr.CompanyId == companyId.Value, cancellationToken);

        var dto = new CustomReportDto
        {
            Id = reportWithNav!.Id,
            Name = reportWithNav.Name,
            Description = reportWithNav.Description,
            ReportType = reportWithNav.ReportType,
            ReportDefinition = reportWithNav.ReportDefinition,
            IsPublic = reportWithNav.IsPublic,
            CreatedByUserId = reportWithNav.CreatedByUserId,
            CreatedByUserName = reportWithNav.CreatedByUser?.UserName,
            CreatedAt = reportWithNav.CreatedAt
        };

        return Result<CustomReportDto>.Success(dto);
    }
}

