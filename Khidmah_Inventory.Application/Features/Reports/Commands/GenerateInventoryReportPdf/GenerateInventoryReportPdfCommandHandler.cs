using MediatR;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Reports.Queries.GetInventoryReport;

namespace Khidmah_Inventory.Application.Features.Reports.Commands.GenerateInventoryReportPdf;

public class GenerateInventoryReportPdfCommandHandler : IRequestHandler<GenerateInventoryReportPdfCommand, Result<byte[]>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IDocumentService _documentService;

    public GenerateInventoryReportPdfCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        IDocumentService documentService)
    {
        _context = context;
        _currentUser = currentUser;
        _documentService = documentService;
    }

    public async Task<Result<byte[]>> Handle(GenerateInventoryReportPdfCommand request, System.Threading.CancellationToken cancellationToken)
    {
        // Get the inventory report data using the existing query handler
        var query = new GetInventoryReportQuery
        {
            WarehouseId = request.WarehouseId,
            CategoryId = request.CategoryId,
            LowStockOnly = request.LowStockOnly
        };

        var getInventoryReportHandler = new GetInventoryReportQueryHandler(_context, _currentUser);
        var reportResult = await getInventoryReportHandler.Handle(query, cancellationToken);

        if (!reportResult.Succeeded)
            return Result<byte[]>.Failure(reportResult.Errors);

        var report = reportResult.Data!;
        var pdfBytes = _documentService.GenerateInventoryReportPdf(report);

        return Result<byte[]>.Success(pdfBytes);
    }
}