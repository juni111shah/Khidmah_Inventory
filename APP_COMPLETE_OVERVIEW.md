# Khidmah Inventory - Complete App Overview

## 1) What this app is

Khidmah Inventory is a full business platform for inventory-driven companies.  
It is not only a stock system, it also includes sales, purchase, warehouse operations, finance, reporting, automation, and AI-assisted workflows.

---

## 2) Tech stack and architecture

## Backend (.NET 8)
- `Khidmah_Inventory.API` - HTTP API, middleware, auth, SignalR hubs
- `Khidmah_Inventory.Application` - business use cases (CQRS with MediatR)
- `Khidmah_Inventory.Domain` - entities and domain rules
- `Khidmah_Inventory.Infrastructure` - EF Core, SQL Server access, services, integrations

## Frontend (Angular)
- `khidmah_inventory.client` - web UI
- `core` layer for auth, guards, interceptors, API and realtime services
- `features` layer for business pages/modules
- `shared` layer for reusable components (tables, loaders, charts, dialogs, etc.)

## Architecture style
- Clean Architecture (separated layers)
- CQRS pattern for commands/queries
- Multi-tenant design (data isolation by company)

---

## 3) How the app works end-to-end

1. User logs in from Angular frontend.
2. Frontend stores JWT and attaches it to API requests.
3. API receives request in controllers.
4. Middleware pipeline applies:
   - Authentication
   - API key middleware
   - Authorization
   - Multi-tenant context
   - Exception handling
5. Controller sends command/query through MediatR.
6. Application handler executes business logic.
7. Infrastructure layer reads/writes SQL Server through EF Core.
8. If needed, realtime updates are pushed over SignalR.
9. Frontend refreshes lists/charts/metrics instantly.

---

## 4) Main business features

## A) Authentication and access control
- Login and registration
- JWT authentication
- Role and permission-based authorization
- Route and UI access protection in frontend
- API key support for integrations

## B) Master data management
- Products
- Categories (hierarchy/tree)
- Brands
- Warehouses
- Customers
- Suppliers
- Companies (multi-tenant structure)

## C) Inventory management
- Stock level monitoring
- Stock transactions (in, out, adjustments)
- Stock transfer between warehouses
- Batch/lot tracking
- Serial number tracking
- Reorder suggestions and purchase order generation path

## D) Sales and purchase operations
- Sales orders (create, edit, track)
- Purchase orders (create, edit, track)
- Supplier and customer flows
- Order lifecycle actions and approvals

## E) Warehouse operations
- Standard warehouse management
- Hands-free picking mode (voice + camera workflow)
- Hands-free supervisor page
- Autonomous warehouse module:
  - Task planning
  - Route optimization
  - Live operations board
- Warehouse map model (zones, aisles, racks, bins)

## F) Dashboard, KPI, intelligence, analytics
- Dashboard with key business metrics
- Sales analytics
- KPI suite:
  - Executive center
  - Sales performance
  - Inventory health
  - Customer intelligence
- Intelligence pages:
  - Profit intelligence
  - Branch performance
  - Staff performance
  - Predictive risk
  - Decision support

## G) Finance and accounting
- Chart of accounts
- Journal entries
- Profit and loss statement
- Balance sheet
- Cash flow statement
- Currency management
- Exchange rates

## H) Reports and documents
- Sales, inventory, and purchase reports
- Custom report endpoints
- PDF generation for documents (such as invoice and purchase order)

## I) Platform and integration features
- Integration Center
- API key management
- Webhook setup and delivery
- Scheduled reports
- Rule-based automation builder
- Automation execution history

## J) AI copilot and voice features
- Dedicated Copilot page (`/copilot`)
- Chat assistant for conversational operations
- Voice input (speech recognition)
- Voice output (text-to-speech)
- Backend copilot command execution via intent parsing

---

## 5) Realtime capabilities

- SignalR hubs for analytics and operations
- Realtime updates for key events (inventory/order/dashboard related)
- Frontend subscribes through a centralized realtime service

---

## 6) Security and tenancy model

- JWT bearer authentication
- Permission-based authorization policies
- API key authentication path for platform use cases
- Multi-tenant middleware and company-scoped data handling
- Password hashing and standard protected auth flow

---

## 7) Frontend user experience features

- Skeleton loading components across pages
- Global search overlay
- Reusable table with state persistence
- Notification center
- Theme/appearance handling
- Onboarding assistant components

---

## 8) Typical user flow examples

## Example 1 - Inventory officer
1. Login
2. Open stock levels
3. Transfer or adjust stock
4. Handle batches/serials
5. Use hands-free picking in warehouse if needed

## Example 2 - Procurement user
1. Review low-stock/reorder suggestions
2. Create purchase order
3. Track PO status and supplier updates
4. Generate related documents/reports

## Example 3 - Manager or executive
1. Open dashboard and KPI pages
2. Review finance and intelligence views
3. Monitor risks and recommendations
4. Use Copilot for quick command-driven actions

---

## 9) Important entry points in codebase

- API startup: `Khidmah_Inventory.API/Program.cs`
- API routes constants: `Khidmah_Inventory.API/Constants/ApiRoutes.cs`
- Frontend routing: `khidmah_inventory.client/src/app/app-routing.module.ts`
- Copilot UI: `khidmah_inventory.client/src/app/features/copilot/chat-assistant/chat-assistant.component.ts`

---

## 10) Short summary

Khidmah Inventory is an enterprise-grade, multi-tenant inventory platform that combines operations, warehouse execution, finance, analytics, automation, and AI/voice assistant functionality in one integrated system.
