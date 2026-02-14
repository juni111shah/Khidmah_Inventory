using MediatR;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Platform.ApiKeys.Models;
using Khidmah_Inventory.Domain.Entities;

namespace Khidmah_Inventory.Application.Features.Platform.ApiKeys.Commands.CreateApiKey;

public class CreateApiKeyCommandHandler : IRequestHandler<CreateApiKeyCommand, Result<CreateApiKeyResult>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IApiKeyService _apiKeyService;

    public CreateApiKeyCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser, IApiKeyService apiKeyService)
    {
        _context = context;
        _currentUser = currentUser;
        _apiKeyService = apiKeyService;
    }

    public async Task<Result<CreateApiKeyResult>> Handle(CreateApiKeyCommand request, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId;
        if (!companyId.HasValue)
            return Result<CreateApiKeyResult>.Failure("Company context is required");

        var (keyValue, keyPrefix, keyHash) = _apiKeyService.GenerateKey();
        var key = new ApiKey(
            companyId.Value,
            request.Name.Trim(),
            keyPrefix,
            keyHash,
            request.Permissions?.Trim() ?? "",
            request.ExpiresAt,
            _currentUser.UserId);
        _context.ApiKeys.Add(key);
        await _context.SaveChangesAsync(cancellationToken);

        var dto = new ApiKeyDto
        {
            Id = key.Id,
            Name = key.Name,
            KeyPrefix = key.KeyPrefix,
            Permissions = key.Permissions,
            ExpiresAt = key.ExpiresAt,
            IsActive = key.IsActive,
            LastUsedAt = key.LastUsedAt,
            RequestCount = key.RequestCount,
            ErrorCount = key.ErrorCount,
            CreatedAt = key.CreatedAt
        };
        return Result<CreateApiKeyResult>.Success(new CreateApiKeyResult { ApiKey = dto, PlainKey = keyValue });
    }
}
