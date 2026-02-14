# API Implementation Audit

This document tracks backend API controllers, routes, and frontend integration (api-code.service and API services).

## Summary

| Area | Status | Notes |
|------|--------|-------|
| **Customers** | ⚠️ Gap | Backend missing GET by id & PUT update; client calls them. **Fixed:** GetCustomer + UpdateCustomer added. |
| **Sales Orders** | ⚠️ Gap | Backend missing PUT update; client calls it. **Fixed:** UpdateSalesOrder added. |
| **Purchase Orders** | ⚠️ Gap | Backend missing PUT update; client calls it. **Fixed:** UpdatePurchaseOrder added. |
| **Companies** | ✅ | Full CRUD + activate/deactivate implemented. |
| **Platform** | ⚠️ Client | Platform endpoints not in api-code.service; **Fixed:** all Platform routes added. |
| **Intelligence** | ⚠️ Client | Intelligence endpoints not in api-code.service; **Fixed:** routes added. |
| **All other modules** | ✅ | Controllers, routes, and client api-code align. |

---

## Backend Controllers vs Routes

| Controller | Base Route | Endpoints | ApiRoutes Used |
|------------|------------|-----------|----------------|
| AuthController | api/auth | POST login, register, refresh-token, logout | ✅ |
| ProductsController | api/products | POST list, GET {id}, POST, PUT {id}, DELETE {id}, PATCH activate/deactivate, POST {id}/image | ✅ |
| CategoriesController | api/categories | POST list, GET {id}, GET tree, POST, PUT {id}, DELETE {id}, POST {id}/image | ✅ |
| BrandsController | api/brands | POST list, GET {id}, POST, PUT {id}, DELETE {id}, POST {id}/logo | ✅ |
| SuppliersController | api/suppliers | POST list, GET {id}, POST, PUT {id}, DELETE {id}, PATCH activate/deactivate, POST {id}/image | ✅ |
| CustomersController | api/customers | POST list, POST, POST {id}/image, **GET {id}, PUT {id}** | ✅ Added |
| UsersController | api/users | GET current, GET new, POST list, GET {id}, POST, PUT {id}/profile, POST change-password, PATCH activate/deactivate, POST avatar | ✅ |
| CompaniesController | api/companies | POST list, GET {id}, POST, PUT {id}, PATCH activate/deactivate, POST {id}/logo | ✅ |
| SalesOrdersController | api/salesorders | POST list, GET {id}, POST, **PUT {id}** | ✅ Added |
| PurchaseOrdersController | api/purchaseorders | POST list, GET {id}, POST, **PUT {id}** | ✅ Added |
| ReportsController | api/reports | GET/POST sales, inventory, purchase, custom, POST custom/{id}/execute | ✅ |
| WarehousesController | api/warehouses | POST list, GET {id}, POST, PUT {id}, DELETE {id}, PATCH activate/deactivate | ✅ |
| InventoryController | api/inventory | POST stock-transaction, transactions/list, stock-levels/list, adjust-stock, batches, batches/list, batches/{id}/recall, serial-numbers, serial-numbers/list | ✅ |
| RolesController | api/roles | GET, GET {id}, POST, PUT {id}, DELETE {id}, POST assign-user, DELETE remove-user | ✅ |
| PermissionsController | api/permissions | GET | ✅ |
| SettingsController | api/settings | GET/POST company, user, system, notification, ui, reports | ✅ |
| ThemeController | api/theme | GET/POST user, global, POST logo | ✅ |
| DashboardController | api/dashboard | GET | ✅ |
| AnalyticsController | api/analytics | POST sales, GET inventory, POST profit | ✅ |
| SearchController | api/search | GET global | ✅ |
| PricingController | api/pricing | GET suggestions | ✅ |
| ReorderingController | api/reordering | GET suggestions, POST generate-po | ✅ |
| CollaborationController | api/collaboration | GET activity-feed, GET/POST comments | ✅ |
| DocumentsController | api/documents | GET invoice/{id}, GET purchase-order/{id} | ✅ |
| WorkflowsController | api/workflows | POST, POST start, POST {id}/approve | ✅ |
| AIController | api/ai | GET demand-forecast/{productId} | ✅ |
| PosController | api/pos | POST sessions/open, POST sessions/close, GET sessions/active, POST sales | ✅ |
| IntelligenceController | api/intelligence | GET product/{productId}, GET dashboard | ✅ |
| PlatformController | api/platform | POST api-keys/list, api-keys, PATCH revoke, POST usage, POST webhooks/list, webhooks, PUT/DELETE webhooks/{id}, POST logs, POST integrations/list, PATCH toggle, POST scheduled-reports/list, scheduled-reports, PUT/DELETE scheduled-reports/{id} | ✅ |

---

## Frontend api-code.service Coverage

- All **Products, Categories, Brands, Suppliers, Customers, Users, Companies, SalesOrders, PurchaseOrders, Reports, Warehouses, Inventory, Roles, Permissions, Settings, Theme, Dashboard, Analytics, Search, Pricing, Reordering, Collaboration, Documents, Workflows, AI, Pos** routes are mapped.
- **Platform** and **Intelligence** routes are added so X-Api-Code header is sent for these endpoints when the interceptor uses api-code.service.

---

## Client API Services vs Backend

| Service | getList | getById | create | update | delete | Other |
|---------|---------|---------|--------|--------|--------|-------|
| customer-api | POST /list | GET /{id} | POST | PUT /{id} | - | - |
| sales-order-api | POST /list | GET /{id} | POST | PUT /{id} | - | - |
| purchase-order-api | POST /list | GET /{id} | POST | PUT /{id} | - | - |
| company-api | POST /list | GET /{id} | POST | PUT /{id} | - | PATCH activate/deactivate, POST logo |
| inventory-api | POST .../list (transactions, stock-levels, batches, serial-numbers) | - | POST (stock-transaction, batches, serial-numbers, adjust-stock) | - | - | POST batches/{id}/recall |
| reorder-api | GET /suggestions | - | - | - | - | POST /generate-po |
| platform-api | POST .../list (api-keys, webhooks, integrations, scheduled-reports) | - | POST (api-keys, webhooks, scheduled-reports) | PUT (webhooks, scheduled-reports) | DELETE (webhooks, scheduled-reports) | PATCH revoke, toggle; POST usage, webhook logs |
| intelligence-api | - | GET /product/{id} | - | - | - | GET /dashboard |

Backend now implements the missing GET/PUT for Customers and PUT for SalesOrders and PurchaseOrders so these client calls succeed.

---

*Last updated: after adding Customers Get/Update, SalesOrders Update, PurchaseOrders Update, and Platform/Intelligence api-code entries.*
