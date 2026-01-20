using Microsoft.AspNetCore.Authorization;

namespace Khidmah_Inventory.API.Attributes;

/// <summary>
/// Authorization attribute for permission-based access control
/// Usage: [AuthorizePermission("Products:Create")]
/// </summary>
public class AuthorizePermissionAttribute : AuthorizeAttribute
{
    private const string POLICY_PREFIX = "Permission:";

    public AuthorizePermissionAttribute(string permission)
    {
        Policy = $"{POLICY_PREFIX}{permission}";
    }
}

