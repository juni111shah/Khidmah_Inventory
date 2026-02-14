# Khidmah Inventory Management System – Complete Overview

## 1. What the App Is

**Khidmah Inventory** is a **multi-tenant inventory management system** for businesses to manage products, warehouses, stock, purchase orders, sales orders, point of sale (POS), users/roles, and reporting.

**Tech stack:**
- **Backend:** .NET 8, Clean Architecture, CQRS (MediatR), EF Core, SQL Server
- **Frontend:** Angular 17, Bootstrap 5, Chart.js / ApexCharts, SignalR
- **Auth:** JWT, role-based access control (RBAC), multi-tenant by CompanyId

---

## 2. How It Works

### Architecture (Clean Architecture + CQRS)

| Layer | Project | Purpose |
|-------|---------|---------|
| **Domain** | `Khidmah_Inventory.Domain` | Entities, value objects; no external dependencies |
| **Application** | `Khidmah_Inventory.Application` | Commands/Queries, handlers, DTOs, validators (FluentValidation), AutoMapper |
| **Infrastructure** | `Khidmah_Inventory.Infrastructure` | DbContext, EF configs, interceptors (audit, multi-tenant), services (identity, file, search, etc.) |
| **API** | `Khidmah_Inventory.API` | Controllers, middleware (exception, multi-tenant), Swagger |
| **Client** | `khidmah_inventory.client` | Angular SPA, routes, guards (auth + permission), API services |

### Request Flow

1. HTTP request → Controller
2. Controller sends Command/Query via MediatR
3. Handler uses `IApplicationDbContext` (and other services)
4. Interceptors set `CompanyId`, `CreatedAt`, `CreatedBy`, etc.
5. Response: standardized `ApiResponse<T>` (success, message, data, errors)

### Multi-Tenancy

- Every entity is scoped by `CompanyId`
- Company comes from JWT claim or `X-Company-Id` header
- Users can belong to multiple companies via `UserCompany`

### Auth & Authorization

- Login returns JWT (and refresh)
- Roles have permissions; endpoints are protected by permission (e.g. `Products:List`, `Users:Create`)
- Frontend uses `AuthGuard` and `PermissionGuard`; some routes require specific permissions

---

## 3. Features (Planned vs Implemented)

### Core / Foundation (Implemented)

| Feature | Backend | Frontend | Notes |
|--------|---------|----------|--------|
| **Authentication** | ✅ Login, Register, JWT, refresh | ✅ Login, Register | Admin-only registration |
| **Users** | ✅ CRUD, assign companies/roles | ✅ List, form, profile | Permission-based |
| **Roles & Permissions** | ✅ CRUD, assign users, list permissions | ✅ List, form | RBAC |
| **Companies** | ✅ API (e.g. list, CRUD, logo upload) | ⚠️ No dedicated UI | Used in auth/tenant context |
| **Theme** | ✅ User/global theme, logo upload | ✅ Theme service / usage | |
| **Settings** | ✅ Company, user, system, notifications, UI, reports | ✅ Settings page | Multiple setting groups |

### Product & Catalog (Implemented)

| Feature | Backend | Frontend | Notes |
|--------|---------|----------|--------|
| **Products** | ✅ Full CRUD, activate/deactivate, image upload | ✅ List, form, detail | SKU, barcode, category, brand, UoM, pricing, thresholds |
| **Categories** | ✅ CRUD, tree | ✅ List, form | Hierarchical |
| **Brands** | ✅ CRUD | ✅ Used in product form | |
| **Units of Measure** | ✅ In domain/DB | ✅ Used in product form | |
| **Barcode Scanner** | ✅ Product lookup | ✅ Barcode scanner page | |
| **Product Images** | ✅ Upload, storage | ✅ Image upload component | |

### Warehouse & Inventory (Implemented)

| Feature | Backend | Frontend | Notes |
|--------|---------|----------|--------|
| **Warehouses** | ✅ Full CRUD, activate/deactivate | ✅ List, form | |
| **Stock Levels** | ✅ List query | ✅ Stock levels list | Per product/warehouse |
| **Stock Transfer** | ✅ Commands | ✅ Transfer UI | Between warehouses |
| **Batches / Lots** | ✅ CRUD, recall | ✅ Batches list (lazy-loaded) | Expiry, recall |
| **Serial Numbers** | ✅ CRUD, list | ✅ Serial numbers list (lazy-loaded) | |
| **Stock Transactions** | ✅ List query | ✅ Used in inventory context | |

### Purchase (Implemented)

| Feature | Backend | Frontend | Notes |
|--------|---------|----------|--------|
| **Suppliers** | ✅ Full CRUD, image, activate/deactivate | ✅ List, form | |
| **Purchase Orders** | ✅ CRUD, items | ✅ List, form | |

### Sales (Implemented)

| Feature | Backend | Frontend | Notes |
|--------|---------|----------|--------|
| **Customers** | ✅ CRUD | ✅ List, form | |
| **Sales Orders** | ✅ CRUD, items | ✅ List, form | |

### POS (Implemented)

| Feature | Backend | Frontend | Notes |
|--------|---------|----------|--------|
| **POS** | ✅ Sessions, create sale, stock deduction | ✅ POS module (lazy), POS main | Product browser, cart, payment, warehouse |

### Reports & Analytics (Implemented)

| Feature | Backend | Frontend | Notes |
|--------|---------|----------|--------|
| **Dashboard** | ✅ Get dashboard data | ✅ Dashboard with charts, stats, time frames | KPIs, charts (e.g. ApexCharts) |
| **Reports** | ✅ Custom reports, execution, analytics | ✅ Reports page | |
| **Sales Analytics** | ✅ Analytics APIs | ✅ Sales analytics page | |

### Advanced / “Premium” Features (Implemented on Backend; Frontend Varies)

| Feature | Backend | Frontend | Notes |
|--------|---------|----------|--------|
| **Document Generation** | ✅ Invoice PDF, PO PDF (QuestPDF) | ✅ Used from order flows | |
| **Reorder Suggestions** | ✅ Suggestions, generate PO from suggestions | ⚠️ API only | Smart reordering |
| **Activity Feed & Comments** | ✅ Activity log, comments, @mentions | ⚠️ API only | Collaboration |
| **Workflows** | ✅ Create, start, approve steps | ⚠️ API only | Approval workflows |
| **AI Demand Forecast** | ✅ Demand forecast query | ✅ Product detail “AI Insights” tab | 7/30/90 day, trend |
| **Price Optimization** | ✅ Price suggestions API | ⚠️ API only | |
| **Custom Report Builder** | ✅ Save/execute custom reports | ⚠️ Reports page exists; builder UI unclear | |
| **Global Search** | ✅ Search API | ⚠️ API only | Multi-entity search |
| **PWA** | N/A | ✅ Offline/Sync/PWA services | Install, offline, sync |

### Not Implemented (from README Roadmap)

- Quotations, delivery orders/challan (as separate modules)
- Full accounting (chart of accounts, invoice posting)
- GRN (Goods Received Note) as dedicated flow
- Purchase return / sales return modules
- Cash register open/close (POS may have partial support)
- Dedicated notification/alert UI (e.g. low stock, expiry)
- Numbering sequences (PO, SO, invoice) in settings

---

## 4. What’s Completed End-to-End (Backend + UI)

- **Auth:** Login, register (admin), JWT, guards
- **Users:** List, create, edit, profile
- **Roles & Permissions:** List, create, edit, assign permissions
- **Products:** List, create, edit, detail, images, activate/deactivate
- **Categories:** List, create, edit, tree
- **Brands:** Used in products (backend CRUD)
- **Warehouses:** List, create, edit, activate/deactivate
- **Inventory:** Stock levels list, stock transfer, batches list, serial numbers list
- **Suppliers:** List, create, edit, images
- **Purchase Orders:** List, create, edit
- **Customers:** List, create, edit
- **Sales Orders:** List, create, edit
- **POS:** Session, cart, payment, stock update
- **Dashboard:** Stats, charts, time frames
- **Reports:** Reports page and backend report execution
- **Sales Analytics:** Dedicated page
- **Settings:** Multiple setting categories
- **Theme:** User/global theme
- **AI Demand Forecast:** Product detail “AI Insights” tab
- **Documents:** PDF generation (invoice, PO) used from orders
- **PWA:** Offline/sync/install support

---

## 5. Backend-Ready, Limited or No UI

- **Companies:** API only (no company list/form in app)
- **Reorder Suggestions:** API only
- **Activity Feed & Comments:** API only
- **Workflows:** API only
- **Price Optimization:** API only
- **Custom Report Builder:** Backend support; UI may be partial
- **Global Search:** API only

---

## 6. Database and API Summary

- **Database:** SQL Server; migrations in `Khidmah_Inventory.Infrastructure/Migrations`; seed data in `Database/Seed` (companies, permissions, roles, users, categories, brands, UoM, etc.).
- **Entities:** 32 domain entities (e.g. User, Role, Company, Product, Category, Brand, Warehouse, StockLevel, StockTransaction, Batch, SerialNumber, PurchaseOrder, SalesOrder, Supplier, Customer, ActivityLog, Comment, Workflow, CustomReport, PosSession, Settings, etc.).
- **API:** 30+ controllers; standard `ApiResponse<T>`; list endpoints support `FilterRequest` (pagination, sort, filters, search). Swagger at `/swagger`.

---

## 7. Frontend Stack and Structure

- **Angular 17**, standalone components where used, lazy loading for POS, batches, serial numbers.
- **Routing:** Permission-based routes for users, roles, categories, products, warehouses, inventory, suppliers, purchase orders, customers, sales orders, POS, dashboard, reports, sales analytics, settings.
- **Services:** Auth, theme, settings, dashboard, product, category, warehouse, inventory, supplier, purchase order, customer, sales order, POS, report, analytics, AI, SignalR, offline/sync/PWA, etc.
- **Shared:** Toast, loading spinner, icons, charts, stat cards, data table patterns, image upload, permission directive.

---

## 8. Summary

The app is a **multi-tenant inventory, purchase, and sales system** with **roles/permissions**, **POS**, **dashboard/reports**, and **advanced backend features** (reorder, workflows, AI forecast, documents). **Core flows (products, stock, PO, SO, POS, users, roles, settings) are implemented end-to-end**; **companies** and several **advanced features** have **backend APIs** but **no or limited dedicated UI**.
