using MediatR;
using Microsoft.AspNetCore.Http;
using Khidmah_Inventory.Application.Common.Models;

namespace Khidmah_Inventory.Application.Features.Suppliers.Commands.UploadSupplierImage;

public class UploadSupplierImageCommand : IRequest<Result<string>>
{
    public Guid SupplierId { get; set; }
    public IFormFile File { get; set; } = null!;
}