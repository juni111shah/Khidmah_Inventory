# Khidmah Inventory – Complete App Flow, Features & Implementation Details

This document describes the **end-to-end flow** of the application, **all features** with their **implementation details** (backend and frontend), and how they connect.

---

## 1. Application Overview

**Khidmah Inventory** is a **multi-tenant inventory management system** for managing products, warehouses, stock, purchase orders, sales orders, POS, users/roles, reporting, and intelligence features.

| Aspect | Technology |
|--------|------------|
| **Backend** | .NET 8, Clean Architecture, CQRS (MediatR), EF Core, SQL Server |
| **Frontend** | Angular 17, Bootstrap 5, Chart.js/ApexCharts, SignalR |
| **Auth** | JWT, role-based access control (RBAC), multi-tenant by `CompanyId` |

---

## 2. High-Level Request Flow

### 2.1 Backend (API → Application → Infrastructure)

```
HTTP Request (with JWT / X-Company-Id)
    ↓
API Middleware (Exception, Multi-tenant, CORS)
    ↓
Controller (e.g. PurchaseOrdersController)
    ↓
[Authorize] + [AuthorizeResource(permission)]
    ↓
MediatR.Send(Command | Query)
    ↓
Handler (e.g. CreatePurchaseOrderCommandHandler)
    ↓
IApplicationDbContext / ICurrentUserService / other services
    ↓
EF Core + Interceptors (Audit, MultiTenant) → SQL Server
    ↓
Result<T> → ApiResponse<T> → JSON Response
```

### 2.2 Frontend (User Action → API → UI)

```
User navigates / clicks (e.g. Create Purchase Order)
    ↓
Route Guard (AuthGuard → PermissionGuard)
    ↓
Component loads (e.g. PurchaseOrderFormComponent)
    ↓
Feature API Service (e.g. purchase-order-api.service.ts)
    ↓
HTTP Client (with JWT in header, optional X-Company-Id)
    ↓
API endpoint (e.g. POST api/purchaseorders)
    ↓
Response parsed as ApiResponse<T> → success/errors
    ↓
UI update (toast, redirect, refresh list)
```

### 2.3 Multi-Tenancy

- **Every entity** is scoped by `CompanyId`.
- **Company context** comes from:
  - JWT claim (after login), or
  - `X-Company-Id` header.
- **Users** can belong to multiple companies via `UserCompany`; current company is set at login or via header.
- **Infrastructure:** `MultiTenantInterceptor` sets `CompanyId` on new entities; list/query handlers filter by `CompanyId`.

### 2.4 Authentication & Authorization

- **Login:** `POST api/auth/login` → JWT (access + refresh). Frontend stores tokens and user/permissions.
- **Guards:**
  - **AuthGuard:** Ensures user is logged in; otherwise redirects to `/login?returnUrl=...`.
  - **PermissionGuard:** Reads `route.data['permission']` and `permissionMode` ('any' | 'all'); redirects to `/unauthorized` if user lacks permission.
- **Backend:** Controllers use `[Authorize]` and `[AuthorizeResource(Controller, Action)]`; permission names align with frontend (e.g. `PurchaseOrders:List`, `Products:Create`).

---

## 3. API Layer Summary

### 3.1 Standard Response Format

All endpoints return **ApiResponse&lt;T&gt;** (or `ApiResponse` for no data):

- `success`, `message`, `statusCode`, `data`, `errors`, `timestamp`.
- Success with data: `200` + `data` populated.
- Validation/business failure: `400` + `errors` array.
- Not found: `404` + message.

### 3.2 Base Controller Pattern

- **BaseController** receives `IMediator`.
- **ExecuteRequest(command/query):** Sends to MediatR, then maps `Result<T>` to `ApiResponse<T>` (Ok / BadRequest / NotFound).
- List endpoints typically accept **FilterRequest** (pagination, sort, filters, search) in POST body.

### 3.3 Controllers & Routes (by feature)

| Controller | Base Route | Main Operations |
|------------|------------|------------------|
| AuthController | api/auth | login, register |
| UsersController | api/users | list (POST), get, create, update, profile, change-password, activate/deactivate, avatar |
| RolesController | api/roles | list, get, create, update, delete, assign-user, remove-user |
| PermissionsController | api/permissions | list |
| CompaniesController | api/companies | list, get, create, update, activate/deactivate, logo |
| CategoriesController | api/categories | list, tree, get, create, update, delete, image |
| BrandsController | api/brands | list, get, create, update, delete, logo |
| ProductsController | api/products | list, get, create, update, delete, activate/deactivate, image |
| WarehousesController | api/warehouses | list, get, create, update, delete, activate/deactivate |
| InventoryController | api/inventory | stock-levels, stock-transaction, batches, serial-numbers, adjust-stock |
| SuppliersController | api/suppliers | list, get, create, update, delete, activate/deactivate, image |
| CustomersController | api/customers | list, get, create, update, image |
| PurchaseOrdersController | api/purchaseorders | list (POST), get, create, update |
| SalesOrdersController | api/salesorders | list (POST), get, create, update, delete |
| PosController | api/pos | session/start, session/end, sale |
| DashboardController | api/dashboard | get data |
| ReportsController | api/reports | sales, inventory, purchase (JSON + PDF), custom execute |
| AnalyticsController | api/analytics | sales, inventory, profit |
| IntelligenceController | api/intelligence | product, dashboard, inventory, warehouse-metrics, supplier, customers, pos/hints |
| PlatformController | api/platform | api-keys, webhooks, integrations, scheduled-reports |
| SettingsController | api/settings | company, user, system, notification, ui, reports |
| ThemeController | api/theme | user, global, logo |
| DocumentsController | api/documents | invoice/{salesOrderId}, purchase-order/{purchaseOrderId} |
| WorkflowsController | api/workflows | create, start, approve |
| CollaborationController | api/collaboration | activity-feed, comments |
| ReorderingController | api/reordering | suggestions, generate-po |
| PricingController | api/pricing | suggestions |
| SearchController | api/search | global |
| AIController | api/ai | demand-forecast/{productId} |

---

## 4. Application Layer (CQRS) – Implementation Pattern

### 4.1 Per-Feature Structure

Each feature lives under `Khidmah_Inventory.Application/Features/<FeatureName>/`:

- **Commands/** – e.g. CreatePurchaseOrder, UpdatePurchaseOrder  
  - `*Command.cs`, `*CommandHandler.cs`, optional `*CommandValidator.cs`
- **Queries/** – e.g. GetPurchaseOrder, GetPurchaseOrdersList  
  - `*Query.cs`, `*QueryHandler.cs`
- **Models/** – DTOs (e.g. PurchaseOrderDto, PurchaseOrderItemDto)

### 4.2 Example: Create Purchase Order (Implementation Flow)

1. **Controller:** `POST api/purchaseorders` with body `CreatePurchaseOrderCommand`.
2. **Command:** Contains `SupplierId`, `OrderDate`, `ExpectedDeliveryDate`, `Notes`, `TermsAndConditions`, `Items[]` (ProductId, Quantity, UnitPrice, etc.).
3. **Handler (CreatePurchaseOrderCommandHandler):**
   - Resolves `CompanyId` from `ICurrentUserService`.
   - Validates supplier exists and belongs to company.
   - Generates order number (e.g. `PO-{yyyyMM}-0001`).
   - Creates `PurchaseOrder` and `PurchaseOrderItem` entities (domain).
   - Calls `CalculateTotals()` on aggregate.
   - Saves via `IApplicationDbContext`.
   - Optionally broadcasts via `IOperationsBroadcast` (SignalR) for real-time UI.
   - Returns `Result<PurchaseOrderDto>`; DTO is built by reloading with includes (Supplier, Items, Product).
4. **Response:** BaseController maps `Result<PurchaseOrderDto>` to `ApiResponse<PurchaseOrderDto>` (200 OK or 400 with errors).

### 4.3 Cross-Cutting in Handlers

- **IApplicationDbContext** – EF Core DbContext abstraction (sets, SaveChanges).
- **ICurrentUserService** – UserId, CompanyId from JWT/header.
- **IOperationsBroadcast** (optional) – SignalR events for real-time dashboards/lists.
- **FluentValidation** – Validators registered with MediatR pipeline.

---

## 5. Domain Entities (Database / Persistence)

Entities live in `Khidmah_Inventory.Domain/Entities/`. All tenant-scoped entities have `CompanyId` and usually inherit from a base with `Id`, audit fields, and soft delete.

| Entity | Purpose |
|--------|--------|
| User, UserCompany, UserRole | Users, company membership, role assignment |
| Role, Permission, RolePermission | RBAC |
| Company, CompanyIntegration | Tenants and integrations |
| Category, Brand, UnitOfMeasure | Catalog master data |
| Product, ProductImage, ProductVariant | Products |
| Warehouse, WarehouseZone, Bin | Warehouses and locations |
| StockLevel, StockTransaction, Batch, SerialNumber | Inventory |
| Supplier, Customer | Partners |
| PurchaseOrder, PurchaseOrderItem | Purchase orders |
| SalesOrder, SalesOrderItem | Sales orders |
| PosSession | POS sessions |
| ActivityLog, Comment | Collaboration / audit |
| Workflow | Approval workflows |
| CustomReport, ScheduledReport | Reports |
| Settings | Company/user/system/UI/notification/report settings |
| ApiKey, ApiKeyUsageLog, Webhook, WebhookDeliveryLog | Platform (API keys, webhooks) |

EF configurations and migrations are in **Khidmah_Inventory.Infrastructure** (Data/Configurations, Migrations).

---

## 6. Frontend Structure & Features

### 6.1 Routing & Guards

- **Public:** `/login`, `/register`.
- **Protected:** All other routes use `AuthGuard`; most also use `PermissionGuard` with `data.permission` and optional `data.permissionMode: 'any' | 'all'`.
- **Lazy-loaded:** POS (`/pos`), Reorder (`/reorder`), Batches (`/inventory/batches`), Serial Numbers (`/inventory/serial-numbers`).
- **Default:** `''` and `'**'` redirect to `/dashboard`.

### 6.2 Navigation (Sidebar) – Feature Grouping

Menu items are defined in **NavigationService** and filtered by **PermissionService** (user permissions). Structure:

| Section | Routes | Permissions (examples) |
|---------|--------|-------------------------|
| Dashboard | /dashboard | Dashboard:Read |
| Daily Briefing | /briefing | Dashboard:Read |
| Command Center | /command-center | Dashboard:Read |
| Reports | /reports | Reports:Sales:Read, Reports:Inventory:Read, Reports:Purchase:Read (any) |
| Intelligence | /intelligence/profit, /branch, /staff, /risks, /decisions | Various (any) |
| Automation | /automation, /automation/builder, /automation/history | Dashboard:Read |
| Users | /users, /users/new, /users/:id, /users/:id/edit | Users:List, Create, Read, Update |
| Roles | /roles, /roles/new, /roles/:id, /roles/:id/edit | Roles:* |
| Companies | /companies, /companies/new, /companies/:id, /companies/:id/users | Companies:Update |
| Workflows | /workflows, /workflows/designer, /workflows/inbox | Workflows:Create, Workflows:Approve (any) |
| Integration Center | /platform | Platform:ApiKeys:List, Webhooks, Integrations, ScheduledReports, ApiKeys:Usage (any) |
| Categories | /categories, /categories/new, /categories/:id, /categories/:id/edit | Categories:* |
| Products | /products, /products/new, /products/barcode-scanner, /products/:id, /products/:id/edit | Products:* |
| Warehouses | /warehouses, /warehouses/new, /warehouses/:id, /warehouses/:id/edit | Warehouses:* |
| Inventory | Stock Levels, Transfer, Batches, Serial Numbers, Reorder | Inventory:StockLevel:List, StockTransaction:Create, Batch:List, SerialNumber:List, Reordering |
| Purchase | Suppliers, Purchase Orders | Suppliers:*, PurchaseOrders:* |
| Sales | Customers, Sales Orders, POS | Customers:*, SalesOrders:*, SalesOrders:Create for POS |
| Settings | /settings | Settings:Company:Read, User, System, Notification, UI, Report (any) |

### 6.3 Feature API Services (Client)

Each feature typically has a corresponding **`*-api.service.ts`** (or similar) in `core/services/` that:

- Calls HTTP endpoints (with auth header and optional `X-Company-Id`).
- Maps responses to `ApiResponse<T>` and returns data or throws/handles errors.
- Used by list/form/detail components.

Examples: `auth.service`, `user-api.service`, `product-api.service`, `purchase-order-api.service`, `pos-api.service`, `dashboard-api.service`, `report-api.service`, `intelligence-api.service`, `platform-api.service`, `workflow-api.service`, etc.

### 6.4 Shared Components & UX

- **Data table** – Pagination, sort, filters, search (aligned with FilterRequest).
- **Forms** – Product, Category, Warehouse, Supplier, Customer, Purchase Order, Sales Order, User, Role, Company, etc.
- **Charts** – Dashboard and analytics (e.g. ApexCharts).
- **Theme** – User/global theme, logo (ThemeService, appearance settings).
- **Toast, loading, icons, stat cards, image upload, permission directive** – Reused across features.

---

## 7. Feature-by-Feature Implementation Summary

### 7.1 Authentication

- **Backend:** AuthController → Login/Register commands → IdentityService (JWT, password hash). Tokens include user id, company, and permissions/roles.
- **Frontend:** Login/Register components → AuthService (store token, user, permissions) → AuthGuard on protected routes.

### 7.2 Users & Roles

- **Backend:** UsersController, RolesController, PermissionsController. Commands: Create/Update user, assign companies/roles, activate/deactivate. Queries: List (with FilterRequest), Get by id. Permissions list for role assignment.
- **Frontend:** Users list/form/profile, Roles list/form; permission-based visibility and route guards.

### 7.3 Companies

- **Backend:** CompaniesController – full CRUD, logo upload, activate/deactivate. Company-scoped queries.
- **Frontend:** Company list, form, company-users (assign users to company).

### 7.4 Categories & Products

- **Backend:** Categories (CRUD, tree), Brands (CRUD), Products (CRUD, activate/deactivate, image upload). Products have SKU, barcode, category, brand, UoM, pricing, thresholds.
- **Frontend:** Categories list/form (tree), Products list/form/detail, barcode scanner, image upload.

### 7.5 Warehouses & Inventory

- **Backend:** Warehouses CRUD; Inventory: stock-levels list, stock-transaction (create transfer/adjust), batches, serial numbers (list/CRUD). CreateStockTransaction updates StockLevel and creates StockTransaction record.
- **Frontend:** Warehouses list/form; Inventory: stock levels list, transfer UI, batches list (lazy), serial numbers list (lazy).

### 7.6 Purchase (Suppliers & Purchase Orders)

- **Backend:** Suppliers CRUD + image; PurchaseOrders list (FilterRequest), get, create, update. Create handler generates PO number, validates supplier/products, calculates totals, can broadcast via SignalR.
- **Frontend:** Suppliers list/form; Purchase orders list/form (create/edit with line items).

### 7.7 Sales (Customers & Sales Orders)

- **Backend:** Customers CRUD; SalesOrders list, get, create, update, delete. Create/update handlers manage order and line items and may trigger stock deduction/document generation.
- **Frontend:** Customers list/form; Sales orders list/form.

### 7.8 Point of Sale (POS)

- **Backend:** PosController – start/end session, create sale (CreatePosSaleCommand: cart, payment, warehouse; deducts stock, creates SalesOrder/PosSession linkage).
- **Frontend:** POS module (lazy): product browser, cart, payment, warehouse selection.

### 7.9 Dashboard & Reports

- **Backend:** Dashboard query returns KPIs and chart data; Reports: sales/inventory/purchase (data + PDF), custom report save/execute.
- **Frontend:** Dashboard with stats and charts; Reports page (run and view reports, export).

### 7.10 Analytics & Intelligence

- **Backend:** Analytics (sales, inventory, profit); Intelligence (product, dashboard, inventory, warehouse metrics, supplier, customers, POS hints). Some use ML/AI services.
- **Frontend:** Sales analytics page; Intelligence: profit, branch performance, staff performance, predictive risk, decision support.

### 7.11 Workflows & Automation

- **Backend:** Workflows: create, start, approve (ApproveWorkflowStepCommand). Automation: rule engine (list, builder, history) – backend support.
- **Frontend:** Workflow list, designer, approval inbox; Automation list, builder, history.

### 7.12 Platform (Integration Center)

- **Backend:** PlatformController – API keys (CRUD, revoke, usage), webhooks (CRUD, logs), integrations (list, toggle), scheduled reports (CRUD).
- **Frontend:** Integration Center page – API keys, webhooks, integrations, scheduled reports.

### 7.13 Settings & Theme

- **Backend:** Settings (company, user, system, notification, UI, reports); Theme (user, global, logo).
- **Frontend:** Settings page (tabs/sections per group); theme/appearance applied via ThemeService and user/global endpoints.

### 7.14 Documents, Reordering, Collaboration, Search, AI

- **Backend:** Documents (invoice PDF, PO PDF); Reordering (suggestions, generate PO); Collaboration (activity feed, comments); Search (global); AI (demand forecast per product).
- **Frontend:** Documents used from order flows; Reorder module (lazy); other features partially or fully API-only (e.g. global search, AI insights on product detail).

### 7.15 Real-Time (SignalR)

- **Backend:** OperationsHub; handlers (e.g. CreatePurchaseOrder) call IOperationsBroadcast to push events (e.g. PurchaseCreated, OrderCreated) per company.
- **Frontend:** SignalR service connects to hub and can refresh lists/dashboard when events received.

### 7.16 PWA & Offline

- **Frontend:** PWA service (install, offline), offline/sync services for limited offline support.

---

## 8. File Locations Quick Reference

| Concern | Location |
|--------|----------|
| API routes | Khidmah_Inventory.API/Constants/ApiRoutes.cs |
| API permissions | Khidmah_Inventory.API/Constants/AuthorizePermissions.cs |
| Controllers | Khidmah_Inventory.API/Controllers/*.cs |
| Base controller / ApiResponse | Khidmah_Inventory.API/Controllers/BaseController.cs, Models/ApiResponse.cs |
| Commands/Queries/Handlers | Khidmah_Inventory.Application/Features/<Feature>/ |
| Domain entities | Khidmah_Inventory.Domain/Entities/ |
| DbContext & configs | Khidmah_Inventory.Infrastructure/Data/ |
| Interceptors | Khidmah_Inventory.Infrastructure/Data/Interceptors/ |
| App routing | khidmah_inventory.client/src/app/app-routing.module.ts |
| Sidebar / menu definition | khidmah_inventory.client/src/app/core/services/navigation.service.ts |
| Guards | khidmah_inventory.client/src/app/core/guards/auth.guard.ts, permission.guard.ts |
| Feature API services | khidmah_inventory.client/src/app/core/services/*-api.service.ts |
| Feature components | khidmah_inventory.client/src/app/features/<feature>/ |

---

## 9. End-to-End Flow Examples

### Create Purchase Order (Full Stack)

1. User has permission `PurchaseOrders:Create` → sees "New Purchase Order" or navigates to `/purchase-orders/new`.
2. PermissionGuard allows route; PurchaseOrderFormComponent loads; form includes supplier, dates, line items (product, qty, price, etc.).
3. User submits → `purchase-order-api.service.create(command)` → `POST api/purchaseorders` with JWT.
4. PurchaseOrdersController.Create receives command; AuthorizeResource checks permission; BaseController.ExecuteRequest sends CreatePurchaseOrderCommand via MediatR.
5. CreatePurchaseOrderCommandHandler: validates supplier and products, generates PO number, creates PurchaseOrder + items, SaveChanges, optional SignalR broadcast, returns Result&lt;PurchaseOrderDto&gt;.
6. BaseController returns 200 + ApiResponse with PurchaseOrderDto.
7. Client shows success toast and redirects to list or detail; optional SignalR update refreshes other clients’ lists.

### View Dashboard

1. User navigates to `/dashboard`; guards require Dashboard:Read.
2. DashboardComponent loads; calls dashboard-api.service.getData() → GET api/dashboard (or POST with filter).
3. DashboardController returns dashboard DTO (KPIs, chart data).
4. Component renders stat cards and charts (e.g. ApexCharts).

### POS Sale

1. User opens POS (`/pos`); POS module loads; pos-api.service starts or gets active session.
2. User adds products to cart, selects warehouse, chooses payment.
3. Submit → pos-api.service.createSale(saleDto) → POST api/pos/sale (or equivalent).
4. CreatePosSaleCommandHandler: creates/updates PosSession, creates SalesOrder and items, creates stock transactions (deduct), updates StockLevels.
5. Client receives success and can print receipt or start new sale.

---

This document gives the **complete flow** of the app, **all major features**, and **implementation details** across API, Application, Domain, Infrastructure, and Angular client. For deeper detail on a specific feature, use the file locations above and the corresponding Command/Query handlers and Angular components/services.
