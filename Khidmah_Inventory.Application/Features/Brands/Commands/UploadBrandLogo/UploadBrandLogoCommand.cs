using MediatR;
using Microsoft.AspNetCore.Http;
using Khidmah_Inventory.Application.Common.Models;

namespace Khidmah_Inventory.Application.Features.Brands.Commands.UploadBrandLogo;

public class UploadBrandLogoCommand : IRequest<Result<string>>
{
    public Guid BrandId { get; set; }
    public IFormFile File { get; set; } = null!;
}