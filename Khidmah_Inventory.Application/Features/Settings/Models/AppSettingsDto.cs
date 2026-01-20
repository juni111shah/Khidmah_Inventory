namespace Khidmah_Inventory.Application.Features.Settings.Models;

public class CompanySettingsDto
{
    public string Name { get; set; } = string.Empty;
    public string? LegalName { get; set; }
    public string? TaxId { get; set; }
    public string? RegistrationNumber { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Country { get; set; }
    public string? PostalCode { get; set; }
    public string Currency { get; set; } = "USD";
    public string TimeZone { get; set; } = "UTC";
    public string FiscalYearStart { get; set; } = "01-01";
    public string FiscalYearEnd { get; set; } = "12-31";
}

public class UserSettingsDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string Language { get; set; } = "en";
    public string DateFormat { get; set; } = "MM/DD/YYYY";
    public string TimeFormat { get; set; } = "12h";
    public string TimeZone { get; set; } = "UTC";
    public bool NotificationsEnabled { get; set; } = true;
    public bool EmailNotifications { get; set; } = true;
    public bool PushNotifications { get; set; } = false;
}

public class SystemSettingsDto
{
    // Numbering Sequences
    public string PurchaseOrderPrefix { get; set; } = "PO";
    public string SalesOrderPrefix { get; set; } = "SO";
    public string InvoicePrefix { get; set; } = "INV";
    public string DeliveryNotePrefix { get; set; } = "DN";
    public string GrnPrefix { get; set; } = "GRN";

    // Stock Settings
    public string? DefaultWarehouse { get; set; }
    public bool AllowNegativeStock { get; set; } = false;
    public bool AutoGenerateSKU { get; set; } = true;
    public string SkuPrefix { get; set; } = "SKU";
    public bool TrackBatchNumbers { get; set; } = false;
    public bool TrackExpiryDates { get; set; } = false;

    // Pricing
    public decimal DefaultTaxRate { get; set; } = 0;
    public bool PriceIncludesTax { get; set; } = false;

    // Inventory Valuation
    public string ValuationMethod { get; set; } = "FIFO";

    // Alerts
    public int LowStockThreshold { get; set; } = 10;
    public int ExpiryAlertDays { get; set; } = 30;

    // Security
    public int SessionTimeout { get; set; } = 60;
    public bool RequirePasswordChange { get; set; } = false;
    public int PasswordMinLength { get; set; } = 8;
    public bool RequireStrongPassword { get; set; } = true;

    // Email/SMTP
    public string? SmtpHost { get; set; }
    public int? SmtpPort { get; set; }
    public string? SmtpUsername { get; set; }
    public string? SmtpPassword { get; set; }
    public bool SmtpEnableSSL { get; set; } = true;
    public string? FromEmail { get; set; }
    public string? FromName { get; set; }
}

public class NotificationSettingsDto
{
    public bool LowStockAlert { get; set; } = true;
    public bool ExpiryAlert { get; set; } = true;
    public bool PurchaseOrderStatus { get; set; } = true;
    public bool SalesOrderStatus { get; set; } = true;
    public bool InvoiceGenerated { get; set; } = true;
    public bool PaymentReceived { get; set; } = true;
    public bool PaymentDue { get; set; } = true;
    public bool SystemUpdates { get; set; } = false;
    public string EmailDigest { get; set; } = "daily";
}

public class UISettingsDto
{
    public bool SidebarCollapsed { get; set; } = false;
    public string SidebarPosition { get; set; } = "left";
    public string Theme { get; set; } = "light";
    public bool CompactMode { get; set; } = false;
    public bool ShowTooltips { get; set; } = true;
    public bool AnimationsEnabled { get; set; } = true;
    public int TablePageSize { get; set; } = 25;
    public bool ShowBreadcrumbs { get; set; } = true;
    public bool ShowNotifications { get; set; } = true;
}

public class ReportSettingsDto
{
    public string DefaultFormat { get; set; } = "pdf";
    public bool IncludeLogo { get; set; } = true;
    public bool IncludeFooter { get; set; } = true;
    public string? FooterText { get; set; }
    public string DateRange { get; set; } = "month";
}

