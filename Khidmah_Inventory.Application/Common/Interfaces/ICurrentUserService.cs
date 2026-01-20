namespace Khidmah_Inventory.Application.Common.Interfaces;

public interface ICurrentUserService
{
    Guid? UserId { get; }
    Guid? CompanyId { get; }
    string? Email { get; }
    bool IsAuthenticated { get; }
    bool HasPermission(string permission);
    bool HasRole(string role);
}

