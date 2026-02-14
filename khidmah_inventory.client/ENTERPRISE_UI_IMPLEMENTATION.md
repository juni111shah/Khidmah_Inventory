# Enterprise UI Modules – Implementation Summary

This document describes the new enterprise UI modules added to the Angular client. All modules follow existing patterns: standalone components, permission guards, `ApiResponse<T>`, reactive forms, and shared components.

---

## 1. Reorder Management Center

**Routes (lazy-loaded under `/reorder`):**
- `/reorder` – Dashboard (stat cards, charts, low-stock preview)
- `/reorder/list` – Full list with filters (warehouse, priority), selection, “Generate PO”
- `/reorder/review` – Single suggestion review with supplier suggestions
- `/reorder/generate-po` – Generate purchase order from selected suggestions (supplier, delivery date, notes)

**Features:** Low stock display, forecast usage, suggested reorder quantity, supplier suggestions, filters, risk badges (Critical/High/Medium/Low), charts for items by priority and days remaining.

**Permissions:** `Reordering:Suggestions:Read`, `Reordering:GeneratePO:Create`

**New files:**
- `core/models/reorder.model.ts`
- `core/services/reorder-api.service.ts`
- `features/reorder/reorder-dashboard/*`
- `features/reorder/reorder-list/*`
- `features/reorder/reorder-review/*`
- `features/reorder/generate-po-from-suggestion/*`
- `features/reorder/reorder-routing.module.ts`, `reorder.module.ts`

**Navigation:** Under Inventory → Reorder.

---

## 2. Company Management UI

**Routes:**
- `/companies` – Company list (falls back to current user’s companies if list API is unavailable)
- `/companies/new` – New company
- `/companies/:id` – Edit company (logo upload via shared image-upload, `uploadType="company-logo"`)
- `/companies/:id/users` – Company users (list; links to user edit)

**Permissions:** `Companies:Update`

**New files:**
- `core/models/company.model.ts`
- `core/services/company-api.service.ts`
- `features/companies/company-list/*`
- `features/companies/company-form/*`
- `features/companies/company-users/*`

**Navigation:** Top-level “Companies”.

---

## 3. Activity Feed & Comments

**Shared component:** `app-activity-feed-panel`

- **Inputs:** `entityType`, `entityId`, `open`, `title`, `canComment`
- **Output:** `openChange`
- **Content:** Activity log list + comments list; “Add comment” textarea and send.

**Integration:**
- **Product detail:** “Activity & comments” button; panel with `entityType="Product"`, `entityId="product.id"`.
- **Purchase order form:** Same pattern for `PurchaseOrder`.
- **Sales order form:** Same pattern for `SalesOrder`.

**API:** `CollaborationApiService` – `getActivityFeed`, `getComments`, `createComment`.

**Permissions:** Collaboration/Activity permissions apply on the API; guard as needed.

**New files:**
- `core/models/collaboration.model.ts`
- `core/services/collaboration-api.service.ts`
- `shared/components/activity-feed-panel/*`

---

## 4. Workflow / Approval UI

**Routes:**
- `/workflows` – Workflow list (calls `getWorkflows()`; may 404 until backend adds endpoint)
- `/workflows/designer` – Simple workflow designer (name, description, entity type, steps JSON)
- `/workflows/inbox` – Approval inbox (calls `getPendingApprovals()`; approve with optional comment)

**Permissions:** `Workflows:Create`, `Workflows:Approve`

**New files:**
- `core/models/workflow.model.ts`
- `core/services/workflow-api.service.ts`
- `features/workflows/workflow-list/*`
- `features/workflows/workflow-designer/*`
- `features/workflows/approval-inbox/*`

**Navigation:** “Workflows”. Notification center links to `/workflows/inbox`.

---

## 5. Price Optimization (Product Detail)

**Location:** Product form – new tab **“AI Price Suggestion”** (when viewing/editing a product).

- Shows current price, recommended price, margin impact.
- “Apply suggested price” fills the sale price in the form (user must click Save to persist).
- Uses `PricingApiService.getSuggestions([productId])`.

**Permission:** Product edit (e.g. `Products:Update`) for apply.

**New files:**
- `core/models/pricing.model.ts`
- `core/services/pricing-api.service.ts`

**Modified:** `features/products/product-form/*` (tab + logic).

---

## 6. Global Search

**Location:** Header (via `ng-content` “actions”), next to notification bell.

- **Component:** `app-global-search`
- **Behaviour:** Input triggers debounced `SearchApiService.globalSearch()`. Results show entity type icon, title, description; click navigates via `item.url`. Recent searches stored in `localStorage` and shown when input is focused and term length &lt; 2.

**New files:**
- `core/models/search.model.ts`
- `core/services/search-api.service.ts`
- `shared/components/global-search/*`

**Integration:** `app.component.html` – “actions” slot; `AppModule` imports `GlobalSearchComponent`.

---

## 7. Alert & Notification Center

**Location:** Header – bell icon with optional badge; dropdown with list of notifications.

- **Component:** `app-notification-center`
- **Content:** Built from reorder suggestions (low stock / critical count) and a placeholder “Approvals” item linking to `/workflows/inbox`. Expandable later (expiring batches, sync failed, etc.).

**New files:**
- `core/models/notification.model.ts`
- `shared/components/notification-center/*`

**Integration:** `app.component.html` – “actions” slot; `AppModule` imports `NotificationCenterComponent`.

---

## 8. Numbering Sequences

**Location:** Settings – new tab **“Numbering”**.

- **Fields:** Purchase order prefix, Sales order prefix, Invoice prefix, Delivery note prefix, GRN prefix.
- **API:** `SettingsApiService.getSystemSettings()` / `saveSystemSettings()` (existing system settings model: `purchaseOrderPrefix`, `salesOrderPrefix`, `invoicePrefix`, etc.).
- **Load:** When “Numbering” tab is selected.

**Modified:** `features/settings/settings.component.ts` (tab, load/save numbering), `settings.component.html` (tab button + form).

---

## Backend Assumptions

- **Reorder:** `GET /api/reordering/suggestions`, `POST /api/reordering/generate-po` (existing).
- **Companies:** `POST /api/companies/list`, `GET /api/companies/:id`, `POST/PUT`, `POST /api/companies/:id/logo` (list/get/create/update may need to be added; list falls back to current user companies).
- **Collaboration:** `GET /api/collaboration/activity-feed`, `GET/POST /api/collaboration/comments` (existing).
- **Workflows:** `POST /api/workflows`, `POST /api/workflows/start`, `POST /api/workflows/:id/approve` (existing). Optional: `GET /api/workflows`, `GET /api/workflows/pending` for list and inbox.
- **Pricing:** `GET /api/pricing/suggestions` (existing).
- **Search:** `GET /api/search/global` (existing).
- **Settings:** `GET/POST /api/settings/system` (existing) used for numbering.

---

## Build Note

The project’s Angular budget limits (bundle size, component styles) may fail the build. If so, adjust `angular.json` budgets or leave as-is for dev; the new code compiles successfully.
