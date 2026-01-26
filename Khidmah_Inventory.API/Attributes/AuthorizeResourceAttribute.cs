using Microsoft.AspNetCore.Authorization;

namespace Khidmah_Inventory.API.Attributes;

/// <summary>
/// Authorization attribute for resource-based access control
/// Usage: [AuthorizeResource(ProductsPermissions.Controller, ProductsPermissions.Actions.ViewAll)]
/// </summary>
public class AuthorizeResourceAttribute : AuthorizeAttribute
{
    private const string POLICY_PREFIX = "Permission:";

    public AuthorizeResourceAttribute(string controller, string action)
    {
        // Map controller and action to permission string (e.g., "Products:Create")
        var permission = $"{controller}:{action}";
        Policy = $"{POLICY_PREFIX}{permission}";
    }
}
