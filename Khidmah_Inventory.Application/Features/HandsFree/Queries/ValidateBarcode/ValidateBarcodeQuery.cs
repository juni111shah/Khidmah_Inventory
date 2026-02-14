using MediatR;
using Khidmah_Inventory.Application.Common.Models;

namespace Khidmah_Inventory.Application.Features.HandsFree.Queries.ValidateBarcode;

public class ValidateBarcodeQuery : IRequest<Result<ValidateBarcodeResult>>
{
    public string Barcode { get; set; } = string.Empty;
}

public class ValidateBarcodeResult
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string? Barcode { get; set; }
    public string Sku { get; set; } = string.Empty;
}
