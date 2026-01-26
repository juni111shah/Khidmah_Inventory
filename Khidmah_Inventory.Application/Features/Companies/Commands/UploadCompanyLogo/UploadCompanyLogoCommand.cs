using MediatR;
using Microsoft.AspNetCore.Http;
using Khidmah_Inventory.Application.Common.Models;

namespace Khidmah_Inventory.Application.Features.Companies.Commands.UploadCompanyLogo;

public class UploadCompanyLogoCommand : IRequest<Result<string>>
{
    public Guid CompanyId { get; set; }
    public IFormFile File { get; set; } = null!;
}