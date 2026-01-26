namespace Khidmah_Inventory.API.Constants;

/// <summary>
/// API validation codes for all modules
/// </summary>
public static class ApiValidationCodes
{
    public static class ProductsModuleCode
    {
        public const string ViewAll = "PRODUCTS_VIEW_ALL";
        public const string ViewById = "PRODUCTS_VIEW_BY_ID";
        public const string ViewActive = "PRODUCTS_VIEW_ACTIVE";
        public const string Search = "PRODUCTS_SEARCH";
        public const string Add = "PRODUCTS_ADD";
        public const string Update = "PRODUCTS_UPDATE";
        public const string Delete = "PRODUCTS_DELETE";
        public const string UpdateStatus = "PRODUCTS_UPDATE_STATUS";
        public const string UploadImage = "PRODUCTS_UPLOAD_IMAGE";
    }

    public static class CategoriesModuleCode
    {
        public const string ViewAll = "CATEGORIES_VIEW_ALL";
        public const string ViewById = "CATEGORIES_VIEW_BY_ID";
        public const string ViewTree = "CATEGORIES_VIEW_TREE";
        public const string Add = "CATEGORIES_ADD";
        public const string Update = "CATEGORIES_UPDATE";
        public const string Delete = "CATEGORIES_DELETE";
        public const string UploadImage = "CATEGORIES_UPLOAD_IMAGE";
    }

    public static class BrandsModuleCode
    {
        public const string ViewAll = "BRANDS_VIEW_ALL";
        public const string ViewById = "BRANDS_VIEW_BY_ID";
        public const string Add = "BRANDS_ADD";
        public const string Update = "BRANDS_UPDATE";
        public const string Delete = "BRANDS_DELETE";
        public const string UploadLogo = "BRANDS_UPLOAD_LOGO";
    }

    public static class SuppliersModuleCode
    {
        public const string ViewAll = "SUPPLIERS_VIEW_ALL";
        public const string ViewById = "SUPPLIERS_VIEW_BY_ID";
        public const string Add = "SUPPLIERS_ADD";
        public const string Update = "SUPPLIERS_UPDATE";
        public const string Delete = "SUPPLIERS_DELETE";
        public const string UpdateStatus = "SUPPLIERS_UPDATE_STATUS";
        public const string UploadImage = "SUPPLIERS_UPLOAD_IMAGE";
    }

    public static class CustomersModuleCode
    {
        public const string ViewAll = "CUSTOMERS_VIEW_ALL";
        public const string ViewById = "CUSTOMERS_VIEW_BY_ID";
        public const string Add = "CUSTOMERS_ADD";
        public const string Update = "CUSTOMERS_UPDATE";
        public const string UploadImage = "CUSTOMERS_UPLOAD_IMAGE";
    }

    public static class UsersModuleCode
    {
        public const string ViewAll = "USERS_VIEW_ALL";
        public const string ViewById = "USERS_VIEW_BY_ID";
        public const string ViewCurrent = "USERS_VIEW_CURRENT";
        public const string Add = "USERS_ADD";
        public const string Update = "USERS_UPDATE";
        public const string ChangePassword = "USERS_CHANGE_PASSWORD";
        public const string UpdateStatus = "USERS_UPDATE_STATUS";
        public const string UploadAvatar = "USERS_UPLOAD_AVATAR";
    }

    public static class CompaniesModuleCode
    {
        public const string ViewAll = "COMPANIES_VIEW_ALL";
        public const string ViewById = "COMPANIES_VIEW_BY_ID";
        public const string Add = "COMPANIES_ADD";
        public const string Update = "COMPANIES_UPDATE";
        public const string UploadLogo = "COMPANIES_UPLOAD_LOGO";
    }

    public static class SalesOrdersModuleCode
    {
        public const string ViewAll = "SALES_ORDERS_VIEW_ALL";
        public const string ViewById = "SALES_ORDERS_VIEW_BY_ID";
        public const string Add = "SALES_ORDERS_ADD";
        public const string Update = "SALES_ORDERS_UPDATE";
        public const string Delete = "SALES_ORDERS_DELETE";
    }

    public static class PurchaseOrdersModuleCode
    {
        public const string ViewAll = "PURCHASE_ORDERS_VIEW_ALL";
        public const string ViewById = "PURCHASE_ORDERS_VIEW_BY_ID";
        public const string Add = "PURCHASE_ORDERS_ADD";
        public const string Update = "PURCHASE_ORDERS_UPDATE";
        public const string Delete = "PURCHASE_ORDERS_DELETE";
    }

    public static class ReportsModuleCode
    {
        public const string Sales = "REPORTS_SALES";
        public const string SalesPdf = "REPORTS_SALES_PDF";
        public const string Inventory = "REPORTS_INVENTORY";
        public const string InventoryPdf = "REPORTS_INVENTORY_PDF";
        public const string Purchase = "REPORTS_PURCHASE";
        public const string PurchasePdf = "REPORTS_PURCHASE_PDF";
        public const string Custom = "REPORTS_CUSTOM";
        public const string CustomExecute = "REPORTS_CUSTOM_EXECUTE";
    }

    public static class WarehousesModuleCode
    {
        public const string ViewAll = "WAREHOUSES_VIEW_ALL";
        public const string ViewById = "WAREHOUSES_VIEW_BY_ID";
        public const string Add = "WAREHOUSES_ADD";
        public const string Update = "WAREHOUSES_UPDATE";
        public const string Delete = "WAREHOUSES_DELETE";
        public const string UpdateStatus = "WAREHOUSES_UPDATE_STATUS";
    }

    public static class InventoryModuleCode
    {
        public const string StockTransaction = "INVENTORY_STOCK_TRANSACTION";
        public const string StockTransactions = "INVENTORY_STOCK_TRANSACTIONS";
        public const string StockLevels = "INVENTORY_STOCK_LEVELS";
        public const string AdjustStock = "INVENTORY_ADJUST_STOCK";
        public const string Batch = "INVENTORY_BATCH";
        public const string Batches = "INVENTORY_BATCHES";
        public const string BatchUpdate = "INVENTORY_BATCH_UPDATE";
        public const string SerialNumber = "INVENTORY_SERIAL_NUMBER";
        public const string SerialNumbers = "INVENTORY_SERIAL_NUMBERS";
    }

    public static class RolesModuleCode
    {
        public const string ViewAll = "ROLES_VIEW_ALL";
        public const string ViewById = "ROLES_VIEW_BY_ID";
        public const string Add = "ROLES_ADD";
        public const string Update = "ROLES_UPDATE";
        public const string Delete = "ROLES_DELETE";
        public const string Assign = "ROLES_ASSIGN";
    }

    public static class PermissionsModuleCode
    {
        public const string ViewAll = "PERMISSIONS_VIEW_ALL";
    }

    public static class SettingsModuleCode
    {
        public const string CompanyRead = "SETTINGS_COMPANY_READ";
        public const string CompanyUpdate = "SETTINGS_COMPANY_UPDATE";
        public const string UserRead = "SETTINGS_USER_READ";
        public const string UserUpdate = "SETTINGS_USER_UPDATE";
        public const string SystemRead = "SETTINGS_SYSTEM_READ";
        public const string SystemUpdate = "SETTINGS_SYSTEM_UPDATE";
        public const string NotificationRead = "SETTINGS_NOTIFICATION_READ";
        public const string NotificationUpdate = "SETTINGS_NOTIFICATION_UPDATE";
        public const string UIRead = "SETTINGS_UI_READ";
        public const string UIUpdate = "SETTINGS_UI_UPDATE";
        public const string ReportRead = "SETTINGS_REPORT_READ";
        public const string ReportUpdate = "SETTINGS_REPORT_UPDATE";
    }

    public static class ThemeModuleCode
    {
        public const string ViewUser = "THEME_VIEW_USER";
        public const string ViewGlobal = "THEME_VIEW_GLOBAL";
        public const string UpdateUser = "THEME_UPDATE_USER";
        public const string UpdateGlobal = "THEME_UPDATE_GLOBAL";
        public const string UploadLogo = "THEME_UPLOAD_LOGO";
    }

    public static class AuthModuleCode
    {
        public const string Login = "AUTH_LOGIN";
        public const string Register = "AUTH_REGISTER";
    }

    public static class DashboardModuleCode
    {
        public const string View = "DASHBOARD_VIEW";
    }

    public static class AnalyticsModuleCode
    {
        public const string Sales = "ANALYTICS_SALES";
        public const string Inventory = "ANALYTICS_INVENTORY";
        public const string Profit = "ANALYTICS_PROFIT";
    }

    public static class SearchModuleCode
    {
        public const string Global = "SEARCH_GLOBAL";
    }

    public static class PricingModuleCode
    {
        public const string Suggestions = "PRICING_SUGGESTIONS";
    }

    public static class ReorderingModuleCode
    {
        public const string Suggestions = "REORDERING_SUGGESTIONS";
        public const string GeneratePO = "REORDERING_GENERATE_PO";
    }

    public static class CollaborationModuleCode
    {
        public const string ActivityFeed = "COLLABORATION_ACTIVITY_FEED";
        public const string Comments = "COLLABORATION_COMMENTS";
    }

    public static class DocumentsModuleCode
    {
        public const string GenerateInvoice = "DOCUMENTS_GENERATE_INVOICE";
        public const string GeneratePurchaseOrder = "DOCUMENTS_GENERATE_PURCHASE_ORDER";
    }

    public static class WorkflowsModuleCode
    {
        public const string Create = "WORKFLOWS_CREATE";
        public const string Start = "WORKFLOWS_START";
        public const string Approve = "WORKFLOWS_APPROVE";
    }

    public static class AIModuleCode
    {
        public const string DemandForecast = "AI_DEMAND_FORECAST";
    }

    public static class PosModuleCode
    {
        public const string Session = "POS_SESSION";
        public const string StartSession = "POS_START_SESSION";
        public const string EndSession = "POS_END_SESSION";
    }
}
