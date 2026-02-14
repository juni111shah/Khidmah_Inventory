/**
 * API validation codes for all modules
 * These codes are sent in the X-Api-Code header to validate API requests
 */
export const ApiValidationCodes = {
  Products: {
    ViewAll: 'PRODUCTS_VIEW_ALL',
    ViewById: 'PRODUCTS_VIEW_BY_ID',
    ViewActive: 'PRODUCTS_VIEW_ACTIVE',
    Search: 'PRODUCTS_SEARCH',
    Add: 'PRODUCTS_ADD',
    Update: 'PRODUCTS_UPDATE',
    Delete: 'PRODUCTS_DELETE',
    UpdateStatus: 'PRODUCTS_UPDATE_STATUS',
    UploadImage: 'PRODUCTS_UPLOAD_IMAGE'
  },
  Categories: {
    ViewAll: 'CATEGORIES_VIEW_ALL',
    ViewById: 'CATEGORIES_VIEW_BY_ID',
    ViewTree: 'CATEGORIES_VIEW_TREE',
    Add: 'CATEGORIES_ADD',
    Update: 'CATEGORIES_UPDATE',
    Delete: 'CATEGORIES_DELETE',
    UploadImage: 'CATEGORIES_UPLOAD_IMAGE'
  },
  Brands: {
    ViewAll: 'BRANDS_VIEW_ALL',
    ViewById: 'BRANDS_VIEW_BY_ID',
    Add: 'BRANDS_ADD',
    Update: 'BRANDS_UPDATE',
    Delete: 'BRANDS_DELETE',
    UploadLogo: 'BRANDS_UPLOAD_LOGO'
  },
  Suppliers: {
    ViewAll: 'SUPPLIERS_VIEW_ALL',
    ViewById: 'SUPPLIERS_VIEW_BY_ID',
    Add: 'SUPPLIERS_ADD',
    Update: 'SUPPLIERS_UPDATE',
    Delete: 'SUPPLIERS_DELETE',
    UpdateStatus: 'SUPPLIERS_UPDATE_STATUS',
    UploadImage: 'SUPPLIERS_UPLOAD_IMAGE'
  },
  Customers: {
    ViewAll: 'CUSTOMERS_VIEW_ALL',
    ViewById: 'CUSTOMERS_VIEW_BY_ID',
    Add: 'CUSTOMERS_ADD',
    Update: 'CUSTOMERS_UPDATE',
    UploadImage: 'CUSTOMERS_UPLOAD_IMAGE'
  },
  Users: {
    ViewAll: 'USERS_VIEW_ALL',
    ViewById: 'USERS_VIEW_BY_ID',
    ViewCurrent: 'USERS_VIEW_CURRENT',
    Add: 'USERS_ADD',
    Update: 'USERS_UPDATE',
    ChangePassword: 'USERS_CHANGE_PASSWORD',
    UpdateStatus: 'USERS_UPDATE_STATUS',
    UploadAvatar: 'USERS_UPLOAD_AVATAR'
  },
  Companies: {
    ViewAll: 'COMPANIES_VIEW_ALL',
    ViewById: 'COMPANIES_VIEW_BY_ID',
    Add: 'COMPANIES_ADD',
    Update: 'COMPANIES_UPDATE',
    UpdateStatus: 'COMPANIES_UPDATE_STATUS',
    UploadLogo: 'COMPANIES_UPLOAD_LOGO'
  },
  SalesOrders: {
    ViewAll: 'SALES_ORDERS_VIEW_ALL',
    ViewById: 'SALES_ORDERS_VIEW_BY_ID',
    Add: 'SALES_ORDERS_ADD',
    Update: 'SALES_ORDERS_UPDATE',
    Delete: 'SALES_ORDERS_DELETE'
  },
  PurchaseOrders: {
    ViewAll: 'PURCHASE_ORDERS_VIEW_ALL',
    ViewById: 'PURCHASE_ORDERS_VIEW_BY_ID',
    Add: 'PURCHASE_ORDERS_ADD',
    Update: 'PURCHASE_ORDERS_UPDATE',
    Delete: 'PURCHASE_ORDERS_DELETE'
  },
  Reports: {
    Sales: 'REPORTS_SALES',
    SalesPdf: 'REPORTS_SALES_PDF',
    Inventory: 'REPORTS_INVENTORY',
    InventoryPdf: 'REPORTS_INVENTORY_PDF',
    Purchase: 'REPORTS_PURCHASE',
    PurchasePdf: 'REPORTS_PURCHASE_PDF',
    Custom: 'REPORTS_CUSTOM',
    CustomExecute: 'REPORTS_CUSTOM_EXECUTE'
  },
  Kpi: {
    Executive: 'KPI_EXECUTIVE',
    Sales: 'KPI_SALES',
    Inventory: 'KPI_INVENTORY',
    Customers: 'KPI_CUSTOMERS'
  },
  Currency: {
    List: 'CURRENCY_LIST',
    GetById: 'CURRENCY_GET_BY_ID',
    Create: 'CURRENCY_CREATE',
    Update: 'CURRENCY_UPDATE',
    Delete: 'CURRENCY_DELETE'
  },
  ExchangeRates: {
    List: 'EXCHANGE_RATES_LIST',
    Create: 'EXCHANGE_RATES_CREATE'
  },
  Finance: {
    AccountsList: 'FINANCE_ACCOUNTS_LIST',
    AccountsTree: 'FINANCE_ACCOUNTS_TREE',
    AccountById: 'FINANCE_ACCOUNTS_READ',
    AccountsCreate: 'FINANCE_ACCOUNTS_CREATE',
    AccountsUpdate: 'FINANCE_ACCOUNTS_UPDATE',
    AccountsDelete: 'FINANCE_ACCOUNTS_DELETE',
    ImportChart: 'FINANCE_IMPORT_CHART',
    Journals: 'FINANCE_JOURNALS',
    StatementsPl: 'FINANCE_STATEMENTS_PL',
    StatementsBalanceSheet: 'FINANCE_STATEMENTS_BALANCE',
    StatementsCashFlow: 'FINANCE_STATEMENTS_CASHFLOW'
  },
  Warehouses: {
    ViewAll: 'WAREHOUSES_VIEW_ALL',
    ViewById: 'WAREHOUSES_VIEW_BY_ID',
    Add: 'WAREHOUSES_ADD',
    Update: 'WAREHOUSES_UPDATE',
    Delete: 'WAREHOUSES_DELETE',
    UpdateStatus: 'WAREHOUSES_UPDATE_STATUS'
  },
  HandsFree: {
    Tasks: 'HANDSFREE_TASKS',
    Complete: 'HANDSFREE_COMPLETE',
    ValidateBarcode: 'HANDSFREE_VALIDATE_BARCODE'
  },
  Inventory: {
    StockTransaction: 'INVENTORY_STOCK_TRANSACTION',
    StockTransactions: 'INVENTORY_STOCK_TRANSACTIONS',
    StockLevels: 'INVENTORY_STOCK_LEVELS',
    AdjustStock: 'INVENTORY_ADJUST_STOCK',
    Batch: 'INVENTORY_BATCH',
    Batches: 'INVENTORY_BATCHES',
    BatchUpdate: 'INVENTORY_BATCH_UPDATE',
    SerialNumber: 'INVENTORY_SERIAL_NUMBER',
    SerialNumbers: 'INVENTORY_SERIAL_NUMBERS'
  },
  Roles: {
    ViewAll: 'ROLES_VIEW_ALL',
    ViewById: 'ROLES_VIEW_BY_ID',
    Add: 'ROLES_ADD',
    Update: 'ROLES_UPDATE',
    Delete: 'ROLES_DELETE',
    Assign: 'ROLES_ASSIGN'
  },
  Permissions: {
    ViewAll: 'PERMISSIONS_VIEW_ALL'
  },
  Settings: {
    CompanyRead: 'SETTINGS_COMPANY_READ',
    CompanyUpdate: 'SETTINGS_COMPANY_UPDATE',
    UserRead: 'SETTINGS_USER_READ',
    UserUpdate: 'SETTINGS_USER_UPDATE',
    SystemRead: 'SETTINGS_SYSTEM_READ',
    SystemUpdate: 'SETTINGS_SYSTEM_UPDATE',
    NotificationRead: 'SETTINGS_NOTIFICATION_READ',
    NotificationUpdate: 'SETTINGS_NOTIFICATION_UPDATE',
    UIRead: 'SETTINGS_UI_READ',
    UIUpdate: 'SETTINGS_UI_UPDATE',
    ReportRead: 'SETTINGS_REPORT_READ',
    ReportUpdate: 'SETTINGS_REPORT_UPDATE'
  },
  Theme: {
    ViewUser: 'THEME_VIEW_USER',
    ViewGlobal: 'THEME_VIEW_GLOBAL',
    UpdateUser: 'THEME_UPDATE_USER',
    UpdateGlobal: 'THEME_UPDATE_GLOBAL',
    UploadLogo: 'THEME_UPLOAD_LOGO'
  },
  Auth: {
    Login: 'AUTH_LOGIN',
    Register: 'AUTH_REGISTER'
  },
  Dashboard: {
    View: 'DASHBOARD_VIEW'
  },
  Analytics: {
    Sales: 'ANALYTICS_SALES',
    Inventory: 'ANALYTICS_INVENTORY',
    Profit: 'ANALYTICS_PROFIT'
  },
  Search: {
    Global: 'SEARCH_GLOBAL'
  },
  Pricing: {
    Suggestions: 'PRICING_SUGGESTIONS'
  },
  Reordering: {
    Suggestions: 'REORDERING_SUGGESTIONS',
    GeneratePO: 'REORDERING_GENERATE_PO'
  },
  Collaboration: {
    ActivityFeed: 'COLLABORATION_ACTIVITY_FEED',
    Comments: 'COLLABORATION_COMMENTS'
  },
  Documents: {
    GenerateInvoice: 'DOCUMENTS_GENERATE_INVOICE',
    GeneratePurchaseOrder: 'DOCUMENTS_GENERATE_PURCHASE_ORDER'
  },
  Workflows: {
    Create: 'WORKFLOWS_CREATE',
    Start: 'WORKFLOWS_START',
    Approve: 'WORKFLOWS_APPROVE'
  },
  AI: {
    DemandForecast: 'AI_DEMAND_FORECAST'
  },
  Pos: {
    Session: 'POS_SESSION',
    StartSession: 'POS_START_SESSION',
    EndSession: 'POS_END_SESSION'
  },
  Intelligence: {
    Product: 'INTELLIGENCE_PRODUCT',
    Dashboard: 'INTELLIGENCE_DASHBOARD'
  },
  Platform: {
    ApiKeysList: 'PLATFORM_API_KEYS_LIST',
    ApiKeysCreate: 'PLATFORM_API_KEYS_CREATE',
    ApiKeysRevoke: 'PLATFORM_API_KEYS_REVOKE',
    ApiKeysUsage: 'PLATFORM_API_KEYS_USAGE',
    WebhooksList: 'PLATFORM_WEBHOOKS_LIST',
    WebhooksCreate: 'PLATFORM_WEBHOOKS_CREATE',
    WebhooksUpdate: 'PLATFORM_WEBHOOKS_UPDATE',
    WebhooksDelete: 'PLATFORM_WEBHOOKS_DELETE',
    WebhooksLogs: 'PLATFORM_WEBHOOKS_LOGS',
    IntegrationsList: 'PLATFORM_INTEGRATIONS_LIST',
    IntegrationsToggle: 'PLATFORM_INTEGRATIONS_TOGGLE',
    ScheduledReportsList: 'PLATFORM_SCHEDULED_REPORTS_LIST',
    ScheduledReportsCreate: 'PLATFORM_SCHEDULED_REPORTS_CREATE',
    ScheduledReportsUpdate: 'PLATFORM_SCHEDULED_REPORTS_UPDATE',
    ScheduledReportsDelete: 'PLATFORM_SCHEDULED_REPORTS_DELETE'
  }
};
