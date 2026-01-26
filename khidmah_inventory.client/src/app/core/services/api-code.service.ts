import { Injectable } from '@angular/core';
import { ApiValidationCodes } from '../constants/api-validation-codes';

/**
 * Service to get API validation codes for endpoints
 */
@Injectable({
  providedIn: 'root'
})
export class ApiCodeService {
  /**
   * Maps API endpoints to their corresponding validation codes
   * Format: 'method:path' => 'API_CODE'
   */
  private endpointCodeMap: Map<string, string> = new Map([
    // Auth
    ['POST:/api/auth/login', ApiValidationCodes.Auth.Login],
    ['POST:/api/auth/register', ApiValidationCodes.Auth.Register],
    ['POST:/api/auth/refresh-token', ApiValidationCodes.Auth.Login], // Reuse login code
    ['POST:/api/auth/logout', ApiValidationCodes.Auth.Login],

    // Products
    ['POST:/api/products/list', ApiValidationCodes.Products.ViewAll],
    ['GET:/api/products/{id}', ApiValidationCodes.Products.ViewById],
    ['POST:/api/products', ApiValidationCodes.Products.Add],
    ['PUT:/api/products/{id}', ApiValidationCodes.Products.Update],
    ['DELETE:/api/products/{id}', ApiValidationCodes.Products.Delete],
    ['PATCH:/api/products/{id}/activate', ApiValidationCodes.Products.UpdateStatus],
    ['PATCH:/api/products/{id}/deactivate', ApiValidationCodes.Products.UpdateStatus],
    ['POST:/api/products/{id}/image', ApiValidationCodes.Products.UploadImage],

    // Categories
    ['POST:/api/categories/list', ApiValidationCodes.Categories.ViewAll],
    ['GET:/api/categories/{id}', ApiValidationCodes.Categories.ViewById],
    ['GET:/api/categories/tree', ApiValidationCodes.Categories.ViewTree],
    ['POST:/api/categories', ApiValidationCodes.Categories.Add],
    ['PUT:/api/categories/{id}', ApiValidationCodes.Categories.Update],
    ['DELETE:/api/categories/{id}', ApiValidationCodes.Categories.Delete],
    ['POST:/api/categories/{id}/image', ApiValidationCodes.Categories.UploadImage],

    // Brands
    ['POST:/api/brands/list', ApiValidationCodes.Brands.ViewAll],
    ['GET:/api/brands/{id}', ApiValidationCodes.Brands.ViewById],
    ['POST:/api/brands', ApiValidationCodes.Brands.Add],
    ['PUT:/api/brands/{id}', ApiValidationCodes.Brands.Update],
    ['DELETE:/api/brands/{id}', ApiValidationCodes.Brands.Delete],
    ['POST:/api/brands/{id}/logo', ApiValidationCodes.Brands.UploadLogo],

    // Suppliers
    ['POST:/api/suppliers/list', ApiValidationCodes.Suppliers.ViewAll],
    ['GET:/api/suppliers/{id}', ApiValidationCodes.Suppliers.ViewById],
    ['POST:/api/suppliers', ApiValidationCodes.Suppliers.Add],
    ['PUT:/api/suppliers/{id}', ApiValidationCodes.Suppliers.Update],
    ['DELETE:/api/suppliers/{id}', ApiValidationCodes.Suppliers.Delete],
    ['PATCH:/api/suppliers/{id}/activate', ApiValidationCodes.Suppliers.UpdateStatus],
    ['PATCH:/api/suppliers/{id}/deactivate', ApiValidationCodes.Suppliers.UpdateStatus],
    ['POST:/api/suppliers/{id}/image', ApiValidationCodes.Suppliers.UploadImage],

    // Customers
    ['POST:/api/customers/list', ApiValidationCodes.Customers.ViewAll],
    ['POST:/api/customers', ApiValidationCodes.Customers.Add],
    ['POST:/api/customers/{id}/image', ApiValidationCodes.Customers.UploadImage],

    // Users
    ['GET:/api/users/current', ApiValidationCodes.Users.ViewCurrent],
    ['GET:/api/users/new', ApiValidationCodes.Users.Add],
    ['POST:/api/users/list', ApiValidationCodes.Users.ViewAll],
    ['GET:/api/users/{id}', ApiValidationCodes.Users.ViewById],
    ['POST:/api/users', ApiValidationCodes.Users.Add],
    ['PUT:/api/users/{id}/profile', ApiValidationCodes.Users.Update],
    ['POST:/api/users/{id}/change-password', ApiValidationCodes.Users.ChangePassword],
    ['PATCH:/api/users/{id}/activate', ApiValidationCodes.Users.UpdateStatus],
    ['PATCH:/api/users/{id}/deactivate', ApiValidationCodes.Users.UpdateStatus],
    ['POST:/api/users/{id}/avatar', ApiValidationCodes.Users.UploadAvatar],

    // Companies
    ['POST:/api/companies/{id}/logo', ApiValidationCodes.Companies.UploadLogo],

    // Sales Orders
    ['POST:/api/salesorders/list', ApiValidationCodes.SalesOrders.ViewAll],
    ['GET:/api/salesorders/{id}', ApiValidationCodes.SalesOrders.ViewById],
    ['POST:/api/salesorders', ApiValidationCodes.SalesOrders.Add],

    // Purchase Orders
    ['POST:/api/purchaseorders/list', ApiValidationCodes.PurchaseOrders.ViewAll],
    ['GET:/api/purchaseorders/{id}', ApiValidationCodes.PurchaseOrders.ViewById],
    ['POST:/api/purchaseorders', ApiValidationCodes.PurchaseOrders.Add],

    // Reports
    ['GET:/api/reports/sales', ApiValidationCodes.Reports.Sales],
    ['POST:/api/reports/sales/pdf', ApiValidationCodes.Reports.SalesPdf],
    ['GET:/api/reports/inventory', ApiValidationCodes.Reports.Inventory],
    ['POST:/api/reports/inventory/pdf', ApiValidationCodes.Reports.InventoryPdf],
    ['GET:/api/reports/purchase', ApiValidationCodes.Reports.Purchase],
    ['POST:/api/reports/purchase/pdf', ApiValidationCodes.Reports.PurchasePdf],
    ['GET:/api/reports/custom', ApiValidationCodes.Reports.Custom],
    ['POST:/api/reports/custom', ApiValidationCodes.Reports.Custom],
    ['POST:/api/reports/custom/{id}/execute', ApiValidationCodes.Reports.CustomExecute],

    // Warehouses
    ['POST:/api/warehouses/list', ApiValidationCodes.Warehouses.ViewAll],
    ['GET:/api/warehouses/{id}', ApiValidationCodes.Warehouses.ViewById],
    ['POST:/api/warehouses', ApiValidationCodes.Warehouses.Add],
    ['PUT:/api/warehouses/{id}', ApiValidationCodes.Warehouses.Update],
    ['DELETE:/api/warehouses/{id}', ApiValidationCodes.Warehouses.Delete],
    ['PATCH:/api/warehouses/{id}/activate', ApiValidationCodes.Warehouses.UpdateStatus],
    ['PATCH:/api/warehouses/{id}/deactivate', ApiValidationCodes.Warehouses.UpdateStatus],

    // Inventory
    ['POST:/api/inventory/stock-transaction', ApiValidationCodes.Inventory.StockTransaction],
    ['POST:/api/inventory/transactions', ApiValidationCodes.Inventory.StockTransaction], // Alias for compatibility
    ['POST:/api/inventory/transactions/list', ApiValidationCodes.Inventory.StockTransactions],
    ['POST:/api/inventory/stock-levels/list', ApiValidationCodes.Inventory.StockLevels],
    ['POST:/api/inventory/adjust-stock', ApiValidationCodes.Inventory.AdjustStock],
    ['POST:/api/inventory/batches', ApiValidationCodes.Inventory.Batch],
    ['POST:/api/inventory/batches/list', ApiValidationCodes.Inventory.Batches],
    ['POST:/api/inventory/batches/{id}/recall', ApiValidationCodes.Inventory.BatchUpdate],
    ['POST:/api/inventory/serial-numbers', ApiValidationCodes.Inventory.SerialNumber],
    ['POST:/api/inventory/serial-numbers/list', ApiValidationCodes.Inventory.SerialNumbers],

    // Roles
    ['GET:/api/roles', ApiValidationCodes.Roles.ViewAll],
    ['GET:/api/roles/{id}', ApiValidationCodes.Roles.ViewById],
    ['POST:/api/roles', ApiValidationCodes.Roles.Add],
    ['PUT:/api/roles/{id}', ApiValidationCodes.Roles.Update],
    ['DELETE:/api/roles/{id}', ApiValidationCodes.Roles.Delete],
    ['POST:/api/roles/{roleId}/assign-user/{userId}', ApiValidationCodes.Roles.Assign],
    ['DELETE:/api/roles/{roleId}/remove-user/{userId}', ApiValidationCodes.Roles.Assign],

    // Permissions
    ['GET:/api/permissions', ApiValidationCodes.Permissions.ViewAll],

    // Settings
    ['GET:/api/settings/company', ApiValidationCodes.Settings.CompanyRead],
    ['POST:/api/settings/company', ApiValidationCodes.Settings.CompanyUpdate],
    ['GET:/api/settings/user', ApiValidationCodes.Settings.UserRead],
    ['POST:/api/settings/user', ApiValidationCodes.Settings.UserUpdate],
    ['GET:/api/settings/system', ApiValidationCodes.Settings.SystemRead],
    ['POST:/api/settings/system', ApiValidationCodes.Settings.SystemUpdate],
    ['GET:/api/settings/notifications', ApiValidationCodes.Settings.NotificationRead],
    ['POST:/api/settings/notifications', ApiValidationCodes.Settings.NotificationUpdate],
    ['GET:/api/settings/ui', ApiValidationCodes.Settings.UIRead],
    ['POST:/api/settings/ui', ApiValidationCodes.Settings.UIUpdate],
    ['GET:/api/settings/reports', ApiValidationCodes.Settings.ReportRead],
    ['POST:/api/settings/reports', ApiValidationCodes.Settings.ReportUpdate],

    // Theme
    ['GET:/api/theme/user', ApiValidationCodes.Theme.ViewUser],
    ['GET:/api/theme/global', ApiValidationCodes.Theme.ViewGlobal],
    ['POST:/api/theme/user', ApiValidationCodes.Theme.UpdateUser],
    ['POST:/api/theme/global', ApiValidationCodes.Theme.UpdateGlobal],
    ['POST:/api/theme/logo', ApiValidationCodes.Theme.UploadLogo],

    // Dashboard
    ['GET:/api/dashboard', ApiValidationCodes.Dashboard.View],

    // Analytics
    ['POST:/api/analytics/sales', ApiValidationCodes.Analytics.Sales],
    ['GET:/api/analytics/inventory', ApiValidationCodes.Analytics.Inventory],
    ['POST:/api/analytics/profit', ApiValidationCodes.Analytics.Profit],

    // Search
    ['GET:/api/search/global', ApiValidationCodes.Search.Global],

    // Pricing
    ['GET:/api/pricing/suggestions', ApiValidationCodes.Pricing.Suggestions],

    // Reordering
    ['GET:/api/reordering/suggestions', ApiValidationCodes.Reordering.Suggestions],
    ['POST:/api/reordering/generate-po', ApiValidationCodes.Reordering.GeneratePO],

    // Collaboration
    ['GET:/api/collaboration/activity-feed', ApiValidationCodes.Collaboration.ActivityFeed],
    ['GET:/api/collaboration/comments', ApiValidationCodes.Collaboration.Comments],
    ['POST:/api/collaboration/comments', ApiValidationCodes.Collaboration.Comments],

    // Documents
    ['GET:/api/documents/invoice/{salesOrderId}', ApiValidationCodes.Documents.GenerateInvoice],
    ['GET:/api/documents/purchase-order/{purchaseOrderId}', ApiValidationCodes.Documents.GeneratePurchaseOrder],

    // Workflows
    ['POST:/api/workflows', ApiValidationCodes.Workflows.Create],
    ['POST:/api/workflows/start', ApiValidationCodes.Workflows.Start],
    ['POST:/api/workflows/{id}/approve', ApiValidationCodes.Workflows.Approve],

    // AI
    ['GET:/api/ai/demand-forecast/{productId}', ApiValidationCodes.AI.DemandForecast],

    // POS
    ['POST:/api/pos/sessions/open', ApiValidationCodes.Pos.StartSession],
    ['POST:/api/pos/sessions/close', ApiValidationCodes.Pos.EndSession],
    ['GET:/api/pos/sessions/active', ApiValidationCodes.Pos.Session],
    ['POST:/api/pos/sales', ApiValidationCodes.Pos.Session]
  ]);

  /**
   * Get API code for a given HTTP method and URL
   * @param method - HTTP method (GET, POST, PUT, DELETE, PATCH)
   * @param url - Full URL or path
   * @returns API validation code or null if not found
   */
  getApiCode(method: string, url: string): string | null {
    // Normalize the URL to match our map format
    const normalizedUrl = this.normalizeUrl(url);
    const methodUpper = method.toUpperCase();
    const key = `${methodUpper}:${normalizedUrl}`;

    // Try exact match first
    let apiCode = this.endpointCodeMap.get(key);
    if (apiCode) {
      return apiCode;
    }

    // Try pattern matching for dynamic routes (e.g., {id}, {productId})
    for (const [pattern, code] of this.endpointCodeMap.entries()) {
      if (this.matchesPattern(key, pattern)) {
        return code;
      }
    }

    return null;
  }

  /**
   * Normalize URL by removing query parameters and base URL
   */
  private normalizeUrl(url: string): string {
    // Remove protocol and domain if present
    let normalized = url.replace(/^https?:\/\/[^\/]+/, '');
    
    // Remove query string
    normalized = normalized.split('?')[0];
    
    // Remove hash
    normalized = normalized.split('#')[0];
    
    // Ensure it starts with /api
    if (!normalized.startsWith('/api')) {
      // Try to extract /api/... part
      const apiIndex = normalized.indexOf('/api');
      if (apiIndex !== -1) {
        normalized = normalized.substring(apiIndex);
      } else {
        // If no /api found, assume it's relative and add /api
        normalized = `/api${normalized.startsWith('/') ? '' : '/'}${normalized}`;
      }
    }
    
    // Remove trailing slash (except for root)
    if (normalized.length > 1 && normalized.endsWith('/')) {
      normalized = normalized.slice(0, -1);
    }
    
    return normalized;
  }

  /**
   * Check if a request URL matches a pattern with placeholders
   */
  private matchesPattern(requestKey: string, patternKey: string): boolean {
    // Split method and URL
    const [requestMethod, requestUrl] = requestKey.split(':');
    const [patternMethod, patternUrl] = patternKey.split(':');
    
    // Methods must match
    if (requestMethod !== patternMethod) {
      return false;
    }
    
    // Convert pattern URL to regex by replacing placeholders
    const regexPattern = patternUrl
      .replace(/\{[^}]+\}/g, '[^/]+') // Replace {id}, {productId}, etc. with regex
      .replace(/\//g, '\\/'); // Escape slashes
    
    const regex = new RegExp(`^${regexPattern}$`);
    return regex.test(requestUrl);
  }
}
