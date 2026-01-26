using MediatR;
using Microsoft.AspNetCore.Http;
using Khidmah_Inventory.Application.Common.Models;

namespace Khidmah_Inventory.Application.Features.Categories.Commands.UploadCategoryImage;

public class UploadCategoryImageCommand : IRequest<Result<string>>
{
    public Guid CategoryId { get; set; }
    public IFormFile File { get; set; } = null!;
}