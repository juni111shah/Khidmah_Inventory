using MediatR;
using Microsoft.AspNetCore.Http;
using Khidmah_Inventory.Application.Common.Models;

namespace Khidmah_Inventory.Application.Features.Customers.Commands.UploadCustomerImage;

public class UploadCustomerImageCommand : IRequest<Result<string>>
{
    public Guid CustomerId { get; set; }
    public IFormFile File { get; set; } = null!;
}