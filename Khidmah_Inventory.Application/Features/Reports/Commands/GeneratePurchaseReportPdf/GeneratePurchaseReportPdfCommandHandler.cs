using MediatR;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Reports.Queries.GetPurchaseReport;

namespace Khidmah_Inventory.Application.Features.Reports.Commands.GeneratePurchaseReportPdf;

public class GeneratePurchaseReportPdfCommandHandler : IRequestHandler<GeneratePurchaseReportPdfCommand, Result<byte[]>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IDocumentService _documentService;

    public GeneratePurchaseReportPdfCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        IDocumentService documentService)
    {
        _context = context;
        _currentUser = currentUser;
        _documentService = documentService;
    }

    public async Task<Result<byte[]>> Handle(GeneratePurchaseReportPdfCommand request, System.Threading.CancellationToken cancellationToken)
    {
        // Get the purchase report data using the existing query handler
        var query = new GetPurchaseReportQuery
        {
            FromDate = request.FromDate,
            ToDate = request.ToDate,
            SupplierId = request.SupplierId
        };

        var getPurchaseReportHandler = new GetPurchaseReportQueryHandler(_context, _currentUser);
        var reportResult = await getPurchaseReportHandler.Handle(query, cancellationToken);

        if (!reportResult.Succeeded)
            return Result<byte[]>.Failure(reportResult.Errors);

        var report = reportResult.Data!;
        var pdfBytes = _documentService.GeneratePurchaseReportPdf(report);

        return Result<byte[]>.Success(pdfBytes);
    }
}