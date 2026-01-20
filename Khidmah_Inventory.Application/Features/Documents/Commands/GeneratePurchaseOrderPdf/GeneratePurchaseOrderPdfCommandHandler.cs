using MediatR;
using Microsoft.EntityFrameworkCore;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;

namespace Khidmah_Inventory.Application.Features.Documents.Commands.GeneratePurchaseOrderPdf;

public class GeneratePurchaseOrderPdfCommandHandler : IRequestHandler<GeneratePurchaseOrderPdfCommand, Result<byte[]>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IDocumentService _documentService;

    public GeneratePurchaseOrderPdfCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        IDocumentService documentService)
    {
        _context = context;
        _currentUser = currentUser;
        _documentService = documentService;
    }

    public async Task<Result<byte[]>> Handle(GeneratePurchaseOrderPdfCommand request, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId;
        if (!companyId.HasValue)
            return Result<byte[]>.Failure("Company context is required");

        var purchaseOrder = await _context.PurchaseOrders
            .Include(po => po.Supplier)
            .Include(po => po.Items)
                .ThenInclude(item => item.Product)
            .FirstOrDefaultAsync(po => po.Id == request.PurchaseOrderId && 
                po.CompanyId == companyId.Value && 
                !po.IsDeleted, cancellationToken);

        if (purchaseOrder == null)
            return Result<byte[]>.Failure("Purchase order not found.");

        var pdfBytes = _documentService.GeneratePurchaseOrderPdf(purchaseOrder);
        return Result<byte[]>.Success(pdfBytes);
    }
}

