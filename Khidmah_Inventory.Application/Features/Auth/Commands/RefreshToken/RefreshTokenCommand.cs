using MediatR;
using Khidmah_Inventory.Application.Common.Models;

namespace Khidmah_Inventory.Application.Features.Auth.Commands.RefreshToken;

public class RefreshTokenCommand : IRequest<Result<RefreshTokenResponseDto>>
{
    public string Token { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
}

public class RefreshTokenResponseDto
{
    public string Token { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
}

