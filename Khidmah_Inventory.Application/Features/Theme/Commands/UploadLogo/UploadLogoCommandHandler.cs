using MediatR;
using Microsoft.Extensions.Logging;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;

namespace Khidmah_Inventory.Application.Features.Theme.Commands.UploadLogo;

public class UploadLogoCommandHandler : IRequestHandler<UploadLogoCommand, Result<UploadLogoResponseDto>>
{
    private readonly IThemeRepository _themeRepository;
    private readonly ILogger<UploadLogoCommandHandler> _logger;

    public UploadLogoCommandHandler(
        IThemeRepository themeRepository,
        ILogger<UploadLogoCommandHandler> logger)
    {
        _themeRepository = themeRepository;
        _logger = logger;
    }

    public async Task<Result<UploadLogoResponseDto>> Handle(UploadLogoCommand request, CancellationToken cancellationToken)
    {
        if (request.File == null || request.File.Length == 0)
        {
            return Result<UploadLogoResponseDto>.Failure("File is required");
        }

        var logoUrl = await _themeRepository.SaveLogoAsync(request.File, cancellationToken);
        _logger.LogInformation("Logo uploaded successfully: {LogoUrl}", logoUrl);
        
        return Result<UploadLogoResponseDto>.Success(new UploadLogoResponseDto
        {
            LogoUrl = logoUrl
        });
    }
}

