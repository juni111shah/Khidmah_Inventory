using MediatR;
using Microsoft.EntityFrameworkCore;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;

namespace Khidmah_Inventory.Application.Features.HandsFree.Queries.ValidateBarcode;

public class ValidateBarcodeQueryHandler : IRequestHandler<ValidateBarcodeQuery, Result<ValidateBarcodeResult>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public ValidateBarcodeQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<ValidateBarcodeResult>> Handle(ValidateBarcodeQuery request, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId;
        if (!companyId.HasValue)
            return Result<ValidateBarcodeResult>.Failure("Company context is required.");

        var code = (request.Barcode ?? "").Trim();
        if (string.IsNullOrEmpty(code))
            return Result<ValidateBarcodeResult>.Failure("Barcode is required.");

        var product = await _context.Products
            .AsNoTracking()
            .FirstOrDefaultAsync(p =>
                p.CompanyId == companyId &&
                p.IsDeleted == false &&
                (p.Barcode == code || p.SKU == code),
                cancellationToken);

        if (product == null)
            return Result<ValidateBarcodeResult>.Failure("Product not found for this barcode.");

        return Result<ValidateBarcodeResult>.Success(new ValidateBarcodeResult
        {
            ProductId = product.Id,
            ProductName = product.Name,
            Barcode = product.Barcode,
            Sku = product.SKU
        });
    }
}
