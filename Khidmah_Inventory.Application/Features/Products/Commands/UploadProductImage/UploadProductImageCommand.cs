using MediatR;
using Microsoft.AspNetCore.Http;
using Khidmah_Inventory.Application.Common.Models;

namespace Khidmah_Inventory.Application.Features.Products.Commands.UploadProductImage;

public class UploadProductImageCommand : IRequest<Result<string>>
{
    public Guid ProductId { get; set; }
    public IFormFile File { get; set; } = null!;
    public string? AltText { get; set; }
    public bool IsPrimary { get; set; } = false;
}