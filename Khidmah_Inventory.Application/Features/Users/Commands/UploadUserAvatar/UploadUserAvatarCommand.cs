using MediatR;
using Microsoft.AspNetCore.Http;
using Khidmah_Inventory.Application.Common.Models;

namespace Khidmah_Inventory.Application.Features.Users.Commands.UploadUserAvatar;

public class UploadUserAvatarCommand : IRequest<Result<string>>
{
    public Guid UserId { get; set; }
    public IFormFile File { get; set; } = null!;
}