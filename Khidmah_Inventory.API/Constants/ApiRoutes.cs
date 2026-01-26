namespace Khidmah_Inventory.API.Constants;

/// <summary>
/// API route constants for all controllers
/// </summary>
public static class ApiRoutes
{
    public static class Products
    {
        public const string Base = "api/products";
        public const string Index = "list";
        public const string GetById = "{id}";
        public const string Add = "";
        public const string Update = "{id}";
        public const string Delete = "{id}";
        public const string Activate = "{id}/activate";
        public const string Deactivate = "{id}/deactivate";
        public const string UploadImage = "{id}/image";
    }

    public static class Categories
    {
        public const string Base = "api/categories";
        public const string Index = "list";
        public const string GetById = "{id}";
        public const string Add = "";
        public const string Update = "{id}";
        public const string Delete = "{id}";
        public const string Tree = "tree";
        public const string UploadImage = "{id}/image";
    }

    public static class Brands
    {
        public const string Base = "api/brands";
        public const string Index = "list";
        public const string GetById = "{id}";
        public const string Add = "";
        public const string Update = "{id}";
        public const string Delete = "{id}";
        public const string UploadLogo = "{id}/logo";
    }

    public static class Suppliers
    {
        public const string Base = "api/suppliers";
        public const string Index = "list";
        public const string GetById = "{id}";
        public const string Add = "";
        public const string Update = "{id}";
        public const string Delete = "{id}";
        public const string Activate = "{id}/activate";
        public const string Deactivate = "{id}/deactivate";
        public const string UploadImage = "{id}/image";
    }

    public static class Customers
    {
        public const string Base = "api/customers";
        public const string Index = "list";
        public const string Add = "";
        public const string UploadImage = "{id}/image";
    }

    public static class Users
    {
        public const string Base = "api/users";
        public const string Index = "list";
        public const string GetById = "{id}";
        public const string Add = "";
        public const string UpdateProfile = "{id}/profile";
        public const string ChangePassword = "{id}/change-password";
        public const string Activate = "{id}/activate";
        public const string Deactivate = "{id}/deactivate";
        public const string UploadAvatar = "{id}/avatar";
        public const string Current = "current";
        public const string New = "new";
    }

    public static class Companies
    {
        public const string Base = "api/companies";
        public const string Index = "list";
        public const string GetById = "{id}";
        public const string Add = "";
        public const string Update = "{id}";
        public const string UploadLogo = "{id}/logo";
    }

    public static class SalesOrders
    {
        public const string Base = "api/salesorders";
        public const string Index = "list";
        public const string GetById = "{id}";
        public const string Add = "";
        public const string Update = "{id}";
        public const string Delete = "{id}";
    }

    public static class PurchaseOrders
    {
        public const string Base = "api/purchaseorders";
        public const string Index = "list";
        public const string GetById = "{id}";
        public const string Add = "";
        public const string Update = "{id}";
        public const string Delete = "{id}";
    }

    public static class Reports
    {
        public const string Base = "api/reports";
        public const string Sales = "sales";
        public const string SalesPdf = "sales/pdf";
        public const string Inventory = "inventory";
        public const string InventoryPdf = "inventory/pdf";
        public const string Purchase = "purchase";
        public const string PurchasePdf = "purchase/pdf";
        public const string Custom = "custom";
        public const string ExecuteCustom = "custom/{id}/execute";
    }

    public static class Warehouses
    {
        public const string Base = "api/warehouses";
        public const string Index = "list";
        public const string GetById = "{id}";
        public const string Add = "";
        public const string Update = "{id}";
        public const string Delete = "{id}";
        public const string Activate = "{id}/activate";
        public const string Deactivate = "{id}/deactivate";
    }

    public static class Inventory
    {
        public const string Base = "api/inventory";
        public const string StockTransaction = "stock-transaction";
        public const string StockTransactions = "stock-transactions";
        public const string StockLevels = "stock-levels";
        public const string AdjustStock = "adjust-stock";
        public const string Batch = "batch";
        public const string Batches = "batches";
        public const string BatchUpdate = "batch/{id}";
        public const string SerialNumber = "serial-number";
        public const string SerialNumbers = "serial-numbers";
    }

    public static class Roles
    {
        public const string Base = "api/roles";
        public const string Index = "";
        public const string GetById = "{id}";
        public const string Add = "";
        public const string Update = "{id}";
        public const string Delete = "{id}";
        public const string AssignUser = "{roleId}/assign-user/{userId}";
        public const string RemoveUser = "{roleId}/remove-user/{userId}";
    }

    public static class Permissions
    {
        public const string Base = "api/permissions";
        public const string Index = "";
    }

    public static class Settings
    {
        public const string Base = "api/settings";
        public const string Company = "company";
        public const string User = "user";
        public const string System = "system";
        public const string Notification = "notification";
        public const string UI = "ui";
        public const string Report = "reports";
    }

    public static class Theme
    {
        public const string Base = "api/theme";
        public const string User = "user";
        public const string Global = "global";
        public const string Logo = "logo";
    }

    public static class Auth
    {
        public const string Base = "api/auth";
        public const string Login = "login";
        public const string Register = "register";
    }

    public static class Dashboard
    {
        public const string Base = "api/dashboard";
        public const string Index = "";
    }

    public static class Analytics
    {
        public const string Base = "api/analytics";
        public const string Sales = "sales";
        public const string Inventory = "inventory";
        public const string Profit = "profit";
    }

    public static class Search
    {
        public const string Base = "api/search";
        public const string Global = "global";
    }

    public static class Pricing
    {
        public const string Base = "api/pricing";
        public const string Suggestions = "suggestions";
    }

    public static class Reordering
    {
        public const string Base = "api/reordering";
        public const string Suggestions = "suggestions";
        public const string GeneratePO = "generate-po";
    }

    public static class Collaboration
    {
        public const string Base = "api/collaboration";
        public const string ActivityFeed = "activity-feed";
        public const string Comments = "comments";
    }

    public static class Documents
    {
        public const string Base = "api/documents";
        public const string GenerateInvoice = "invoice/{salesOrderId}";
        public const string GeneratePurchaseOrder = "purchase-order/{purchaseOrderId}";
    }

    public static class Workflows
    {
        public const string Base = "api/workflows";
        public const string Create = "";
        public const string Start = "start";
        public const string Approve = "approve";
    }

    public static class AI
    {
        public const string Base = "api/ai";
        public const string DemandForecast = "demand-forecast/{productId}";
    }

    public static class Pos
    {
        public const string Base = "api/pos";
        public const string Session = "session";
        public const string StartSession = "session/start";
        public const string EndSession = "session/end";
    }
}
