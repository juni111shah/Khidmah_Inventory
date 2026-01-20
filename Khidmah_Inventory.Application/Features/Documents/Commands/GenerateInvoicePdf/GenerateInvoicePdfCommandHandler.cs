using MediatR;
using Microsoft.EntityFrameworkCore;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;

namespace Khidmah_Inventory.Application.Features.Documents.Commands.GenerateInvoicePdf;

public class GenerateInvoicePdfCommandHandler : IRequestHandler<GenerateInvoicePdfCommand, Result<byte[]>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IDocumentService _documentService;

    public GenerateInvoicePdfCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        IDocumentService documentService)
    {
        _context = context;
        _currentUser = currentUser;
        _documentService = documentService;
    }

    public async Task<Result<byte[]>> Handle(GenerateInvoicePdfCommand request, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId;
        if (!companyId.HasValue)
            return Result<byte[]>.Failure("Company context is required");

        var salesOrder = await _context.SalesOrders
            .Include(so => so.Customer)
            .Include(so => so.Items)
                .ThenInclude(item => item.Product)
                    .ThenInclude(p => p.UnitOfMeasure)
            .FirstOrDefaultAsync(so => so.Id == request.SalesOrderId && 
                so.CompanyId == companyId.Value && 
                !so.IsDeleted, cancellationToken);

        if (salesOrder == null)
            return Result<byte[]>.Failure("Sales order not found.");

        var pdfBytes = _documentService.GenerateInvoicePdf(salesOrder);
        return Result<byte[]>.Success(pdfBytes);
    }
}

