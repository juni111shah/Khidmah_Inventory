using MediatR;
using Microsoft.EntityFrameworkCore;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Reports.Queries.GetSalesReport;

namespace Khidmah_Inventory.Application.Features.Reports.Commands.GenerateSalesReportPdf;

public class GenerateSalesReportPdfCommandHandler : IRequestHandler<GenerateSalesReportPdfCommand, Result<byte[]>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IDocumentService _documentService;

    public GenerateSalesReportPdfCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        IDocumentService documentService)
    {
        _context = context;
        _currentUser = currentUser;
        _documentService = documentService;
    }

    public async Task<Result<byte[]>> Handle(GenerateSalesReportPdfCommand request, CancellationToken cancellationToken)
    {
        // Get the sales report data using the existing query handler
        var query = new GetSalesReportQuery
        {
            FromDate = request.FromDate,
            ToDate = request.ToDate,
            CustomerId = request.CustomerId
        };

        var getSalesReportHandler = new GetSalesReportQueryHandler(_context, _currentUser);
        var reportResult = await getSalesReportHandler.Handle(query, cancellationToken);

        if (!reportResult.Succeeded)
            return Result<byte[]>.Failure(reportResult.Errors);

        var report = reportResult.Data!;
        var pdfBytes = _documentService.GenerateSalesReportPdf(report);

        return Result<byte[]>.Success(pdfBytes);
    }
}