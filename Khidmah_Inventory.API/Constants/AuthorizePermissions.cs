namespace Khidmah_Inventory.API.Constants;

/// <summary>
/// Authorization permission constants for all controllers
/// </summary>
public static class AuthorizePermissions
{
    public static class ProductsPermissions
    {
        public const string Controller = "Products";
        public static class Actions
        {
            public const string ViewAll = "List";
            public const string ViewById = "Read";
            public const string Add = "Create";
            public const string Update = "Update";
            public const string Delete = "Delete";
        }
    }

    public static class CategoriesPermissions
    {
        public const string Controller = "Categories";
        public static class Actions
        {
            public const string ViewAll = "List";
            public const string ViewById = "Read";
            public const string Add = "Create";
            public const string Update = "Update";
            public const string Delete = "Delete";
        }
    }

    public static class BrandsPermissions
    {
        public const string Controller = "Brands";
        public static class Actions
        {
            public const string ViewAll = "List";
            public const string ViewById = "Read";
            public const string Add = "Create";
            public const string Update = "Update";
            public const string Delete = "Delete";
        }
    }

    public static class SuppliersPermissions
    {
        public const string Controller = "Suppliers";
        public static class Actions
        {
            public const string ViewAll = "List";
            public const string ViewById = "Read";
            public const string Add = "Create";
            public const string Update = "Update";
            public const string Delete = "Delete";
        }
    }

    public static class CustomersPermissions
    {
        public const string Controller = "Customers";
        public static class Actions
        {
            public const string ViewAll = "List";
            public const string ViewById = "Read";
            public const string Add = "Create";
            public const string Update = "Update";
        }
    }

    public static class UsersPermissions
    {
        public const string Controller = "Users";
        public static class Actions
        {
            public const string ViewAll = "List";
            public const string ViewById = "Read";
            public const string Add = "Create";
            public const string Update = "Update";
        }
    }

    public static class CompaniesPermissions
    {
        public const string Controller = "Companies";
        public static class Actions
        {
            public const string ViewAll = "List";
            public const string ViewById = "Read";
            public const string Add = "Create";
            public const string Update = "Update";
        }
    }

    public static class SalesOrdersPermissions
    {
        public const string Controller = "SalesOrders";
        public static class Actions
        {
            public const string ViewAll = "List";
            public const string ViewById = "Read";
            public const string Add = "Create";
            public const string Update = "Update";
        }
    }

    public static class PurchaseOrdersPermissions
    {
        public const string Controller = "PurchaseOrders";
        public static class Actions
        {
            public const string ViewAll = "List";
            public const string ViewById = "Read";
            public const string Add = "Create";
            public const string Update = "Update";
        }
    }

    public static class ReportsPermissions
    {
        public const string Controller = "Reports";
        public static class Actions
        {
            public const string SalesRead = "Sales:Read";
            public const string InventoryRead = "Inventory:Read";
            public const string PurchaseRead = "Purchase:Read";
            public const string CustomRead = "Custom:Read";
            public const string CustomCreate = "Custom:Create";
            public const string CustomExecute = "Custom:Execute";
        }
    }

    public static class KpiPermissions
    {
        public const string Controller = "Kpi";
        public static class Actions
        {
            public const string Read = "Read";
        }
    }

    public static class FinancePermissions
    {
        public const string Controller = "Finance";
        public static class Actions
        {
            public const string AccountsList = "Accounts:List";
            public const string AccountsRead = "Accounts:Read";
            public const string AccountsCreate = "Accounts:Create";
            public const string AccountsUpdate = "Accounts:Update";
            public const string AccountsDelete = "Accounts:Delete";
            public const string JournalsRead = "Journals:Read";
            public const string StatementsRead = "Statements:Read";
        }
    }

    public static class WarehousesPermissions
    {
        public const string Controller = "Warehouses";
        public static class Actions
        {
            public const string ViewAll = "List";
            public const string ViewById = "Read";
            public const string Add = "Create";
            public const string Update = "Update";
            public const string Delete = "Delete";
        }
    }

    /// <summary>
    /// Digital warehouse map (autonomous warehouse spatial model).
    /// </summary>
    public static class WarehouseMapPermissions
    {
        public const string Controller = "Warehouses";
        public static class Actions
        {
            public const string ViewAll = "List";
            public const string ViewById = "Read";
            public const string Add = "Create";
            public const string Update = "Update";
            public const string Delete = "Delete";
        }
    }

    /// <summary>
    /// Copilot actions respect user permissions (e.g. PurchaseOrders:Create for create PO).
    /// </summary>
    public static class CopilotPermissions
    {
        public const string Controller = "Dashboard";
        public static class Actions
        {
            public const string Execute = "Read";
        }
    }

    public static class InventoryPermissions
    {
        public const string Controller = "Inventory";
        public static class Actions
        {
            public const string StockTransactionCreate = "StockTransaction:Create";
            public const string StockTransactionList = "StockTransaction:List";
            public const string StockLevelList = "StockLevel:List";
            public const string BatchCreate = "Batch:Create";
            public const string BatchList = "Batch:List";
            public const string BatchUpdate = "Batch:Update";
            public const string SerialNumberCreate = "SerialNumber:Create";
            public const string SerialNumberList = "SerialNumber:List";
        }
    }

    public static class RolesPermissions
    {
        public const string Controller = "Roles";
        public static class Actions
        {
            public const string ViewAll = "List";
            public const string ViewById = "Read";
            public const string Add = "Create";
            public const string Update = "Update";
            public const string Delete = "Delete";
            public const string Assign = "Assign";
        }
    }

    public static class PermissionsPermissions
    {
        public const string Controller = "Permissions";
        public static class Actions
        {
            public const string ViewAll = "Read";
        }
    }

    public static class SettingsPermissions
    {
        public const string Controller = "Settings";
        public static class Actions
        {
            public const string CompanyRead = "Company:Read";
            public const string CompanyUpdate = "Company:Update";
            public const string UserRead = "User:Read";
            public const string UserUpdate = "User:Update";
            public const string SystemRead = "System:Read";
            public const string SystemUpdate = "System:Update";
            public const string NotificationRead = "Notification:Read";
            public const string NotificationUpdate = "Notification:Update";
            public const string UIRead = "UI:Read";
            public const string UIUpdate = "UI:Update";
            public const string ReportRead = "Report:Read";
            public const string ReportUpdate = "Report:Update";
        }
    }

    public static class ThemePermissions
    {
        public const string Controller = "Theme";
        public static class Actions
        {
            public const string Read = "Read";
            public const string Update = "Update";
        }
    }

    public static class AuthPermissions
    {
        public const string Controller = "Auth";
        public static class Actions
        {
            public const string Create = "Create";
        }
    }

    public static class DashboardPermissions
    {
        public const string Controller = "Dashboard";
        public static class Actions
        {
            public const string Read = "Read";
        }
    }

    public static class AnalyticsPermissions
    {
        public const string Controller = "Analytics";
        public static class Actions
        {
            public const string Sales = "Sales:Read";
            public const string Inventory = "Inventory:Read";
            public const string Profit = "Profit:Read";
        }
    }

    public static class SearchPermissions
    {
        public const string Controller = "Search";
        public static class Actions
        {
            public const string Global = "Global:Read";
        }
    }

    public static class PricingPermissions
    {
        public const string Controller = "Pricing";
        public static class Actions
        {
            public const string Suggestions = "Suggestions:Read";
        }
    }

    public static class ReorderingPermissions
    {
        public const string Controller = "Reordering";
        public static class Actions
        {
            public const string Suggestions = "Suggestions:Read";
            public const string GeneratePO = "GeneratePO:Create";
        }
    }

    public static class CollaborationPermissions
    {
        public const string Controller = "Collaboration";
        public static class Actions
        {
            public const string ActivityFeed = "ActivityFeed:Read";
            public const string Comments = "Comments:Read";
            public const string CommentsCreate = "Comments:Create";
        }
    }

    public static class DocumentsPermissions
    {
        public const string Controller = "Documents";
        public static class Actions
        {
            public const string GenerateInvoice = "Invoice:Generate";
            public const string GeneratePurchaseOrder = "PurchaseOrder:Generate";
        }
    }

    public static class WorkflowsPermissions
    {
        public const string Controller = "Workflows";
        public static class Actions
        {
            public const string Create = "Create";
            public const string Start = "Start";
            public const string Approve = "Approve";
        }
    }

    public static class AIPermissions
    {
        public const string Controller = "AI";
        public static class Actions
        {
            public const string DemandForecast = "DemandForecast:Read";
        }
    }

    public static class CurrencyPermissions
    {
        public const string Controller = "Currency";
        public static class Actions
        {
            public const string List = "List";
            public const string Read = "Read";
            public const string Create = "Create";
            public const string Update = "Update";
            public const string Delete = "Delete";
        }
    }

    public static class ExchangeRatesPermissions
    {
        public const string Controller = "ExchangeRates";
        public static class Actions
        {
            public const string List = "List";
            public const string Create = "Create";
        }
    }

    public static class PosPermissions
    {
        public const string Controller = "Pos";
        public static class Actions
        {
            public const string Session = "Session";
            public const string StartSession = "StartSession";
            public const string EndSession = "EndSession";
        }
    }

    public static class PlatformPermissions
    {
        public const string Controller = "Platform";
        public static class Actions
        {
            public const string ApiKeysList = "ApiKeys:List";
            public const string ApiKeysCreate = "ApiKeys:Create";
            public const string ApiKeysUpdate = "ApiKeys:Update";
            public const string ApiKeysRevoke = "ApiKeys:Revoke";
            public const string ApiKeysUsage = "ApiKeys:Usage";
            public const string WebhooksList = "Webhooks:List";
            public const string WebhooksCreate = "Webhooks:Create";
            public const string WebhooksUpdate = "Webhooks:Update";
            public const string WebhooksDelete = "Webhooks:Delete";
            public const string WebhooksLogs = "Webhooks:Logs";
            public const string IntegrationsList = "Integrations:List";
            public const string IntegrationsToggle = "Integrations:Toggle";
            public const string ScheduledReportsList = "ScheduledReports:List";
            public const string ScheduledReportsCreate = "ScheduledReports:Create";
            public const string ScheduledReportsUpdate = "ScheduledReports:Update";
            public const string ScheduledReportsDelete = "ScheduledReports:Delete";
        }
    }
}
