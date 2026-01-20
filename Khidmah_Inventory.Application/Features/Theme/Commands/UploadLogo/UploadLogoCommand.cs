using MediatR;
using Microsoft.AspNetCore.Http;
using Khidmah_Inventory.Application.Common.Models;

namespace Khidmah_Inventory.Application.Features.Theme.Commands.UploadLogo;

public class UploadLogoCommand : IRequest<Result<UploadLogoResponseDto>>
{
    public IFormFile File { get; set; } = null!;
}

public class UploadLogoResponseDto
{
    public string LogoUrl { get; set; } = string.Empty;
}

