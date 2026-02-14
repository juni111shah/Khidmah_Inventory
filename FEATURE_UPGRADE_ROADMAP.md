# Feature Upgrade Roadmap – Implemented vs Next Steps

This document tracks the upgrades you requested: what is **done**, and what **remains** with concrete file paths and steps.

---

## 1. Global Search (Universal & Fast) – **DONE**

### Backend
- **SearchController** – Still `GET api/search/global`; now returns grouped result.
- **GlobalSearchQuery** – Uses `SearchTerm` and `LimitPerGroup` (default 10). Returns `GlobalSearchResultDto`.
- **GlobalSearchResultDto** – Grouped: `Products`, `Customers`, `Suppliers`, `PurchaseOrders`, `SalesOrders`. Each item: `Id`, `NameOrNumber`, `Route`, `ExtraInfo`.
- **SearchService.SearchGroupedAsync** – Uses `EF.Functions.Like` for SQL LIKE; per-group limit; AsNoTracking.
- **Files:**  
  `Khidmah_Inventory.Application/Features/Search/Models/GlobalSearchItemDto.cs`,  
  `GlobalSearchResultDto.cs`,  
  `Khidmah_Inventory.Application/Common/Interfaces/ISearchService.cs` (new method),  
  `Khidmah_Inventory.Infrastructure/Services/SearchService.cs` (SearchGroupedAsync),  
  `GlobalSearchQuery.cs`, `GlobalSearchQueryHandler.cs`.

### Frontend
- **Search overlay** – Full overlay with input, grouped sections, recent searches.
- **Ctrl+K** (and Cmd+K) – Opens overlay; handled in `app.component.ts` via `SearchOverlayService`.
- **Debounced API** – 280ms in `search-overlay.component.ts`.
- **Grouped UI** – Sections: Products, Customers, Suppliers, Purchase Orders, Sales Orders.
- **Enter** – Navigates to first result; **Click** – Navigates to clicked item.
- **Recent searches** – Cached in `localStorage` under `global_search_recent` (max 8).
- **Files:**  
  `core/models/search.model.ts` (GlobalSearchResultDto, GlobalSearchItemDto),  
  `core/services/search-api.service.ts`,  
  `core/services/search-overlay.service.ts`,  
  `shared/components/search-overlay/*`,  
  `shared/components/global-search/*` (now only opens overlay),  
  `app.component.ts` (HostListener for Ctrl+K),  
  `app.component.html` (search trigger + overlay),  
  `app.module.ts` (SearchOverlayComponent).

---

## 2. Rule-Based Automation Engine – **PARTIALLY DONE**

### Backend – Done
- **AutomationRule** entity – `CompanyId`, `Name`, `Trigger` (e.g. StockBelowThreshold, POApproved, SaleCreated), `ConditionJson`, `ActionJson`, `IsActive`.
- **AutomationRuleHistory** entity – `AutomationRuleId`, `Trigger`, `TriggerContextJson`, `ActionExecuted`, `Success`, `ErrorMessage`.
- **DbSets** – `IApplicationDbContext` / `ApplicationDbContext`: `AutomationRules`, `AutomationRuleHistories`.
- **EF configs** – `AutomationRuleConfiguration.cs`, `AutomationRuleHistoryConfiguration.cs`.
- **IAutomationExecutor** – Interface with `ExecuteStockBelowThresholdAsync`, `ExecutePOApprovedAsync`, `ExecuteSaleCreatedAsync`.
- **Migration** – `20260212100000_AddAutomationRules.cs`; snapshot updated.
- **Files:**  
  `Domain/Entities/AutomationRule.cs`, `AutomationRuleHistory.cs`,  
  `Application/Common/Interfaces/IAutomationExecutor.cs`,  
  `Infrastructure/Data/Configurations/*`,  
  `Infrastructure/Data/ApplicationDbContext.cs`,  
  `Application/Common/Interfaces/IApplicationDbContext.cs`.

### Backend – To Do
- **AutomationExecutor implementation** – In Infrastructure (or API): load rules by trigger, evaluate `ConditionJson`, execute actions from `ActionJson`: create notification (existing command), create PO draft (new command or stub), send webhook (existing `IWebhookDispatchService`). Log to `AutomationRuleHistory`.
- **Call executor** – From `CreateStockTransactionCommandHandler` (when stock below threshold), from workflow approve (POApproved), from `CreateSalesOrderCommandHandler` (SaleCreated).
- **api/automation-rules CRUD** – Controller: List, Get, Create, Update, Delete, Toggle Active. Commands/Queries in Application.

### Frontend – To Do
- **Rule builder UI** – IF [trigger dropdown] AND [conditions editor] THEN [action dropdown]. Simple dropdowns; persist `ConditionJson` / `ActionJson`.
- **History page** – List `AutomationRuleHistory` with filters (rule, date, success).

---

## 3. Inventory Timeline – **TO DO**

### Backend
- **Endpoint** – e.g. `GET api/inventory/timeline?productId=&warehouseId=&from=&to=`.
- **Query** – Build chronological list from: `StockTransaction`, `SalesOrder` (items), `PurchaseOrder` (items), adjustments. DTO: `Date`, `Type`, `QuantityChange`, `Reference`, `User`.
- **Files to add:**  
  `Application/Features/Inventory/Queries/GetInventoryTimeline/`,  
  `Application/Features/Inventory/Models/InventoryTimelineItemDto.cs`,  
  `API/Controllers/InventoryController.cs` (new action).

### Frontend
- **Product detail tab** – “Timeline” tab; call timeline API for current product (and optional warehouse).
- **Vertical timeline UI** – Date, type, quantity change, reference, user; optional filters by date range.
- **Files:**  
  Product detail component (add tab),  
  `inventory-api.service.ts` (timeline method),  
  New timeline component or inline template.

---

## 4. Executive Dashboard – **TO DO**

### Backend
- **New query** – e.g. `GetExecutiveDashboardQuery` returning: revenue today, profit today, low stock count, pending approvals count, top products, dead inventory (e.g. no movement in X days).
- **Endpoint** – e.g. `GET api/dashboard/executive` or extend existing dashboard.
- **Files:**  
  `Application/Features/Dashboard/Queries/GetExecutiveDashboard/`,  
  DTO for the above metrics.

### Frontend
- **Command center page** – You already have `/command-center` and `CommandCenterComponent`. Enhance with big cards for each metric and charts.
- **Auto refresh** – Subscribe to SignalR (e.g. `DashboardUpdated` or new event) and refresh executive data.

---

## 5. Product Intelligence – **TO DO**

### Backend
- **Extend** `api/intelligence/product/{productId}` (or equivalent) with: velocity, days until out of stock, recommended reorder qty, margin trend.
- **Files:**  
  `Application/Features/Intelligence/` (extend existing product intelligence query/DTO),  
  `API/Controllers/IntelligenceController.cs`.

### Frontend
- **Product detail** – “Insights” or “Intelligence” panel; show the new fields with colored indicators (e.g. red/yellow/green for stock days).

---

## 6. Bulk Actions – **TO DO**

### Backend
- **Bulk endpoints** – e.g. `POST api/products/bulk-activate`, `bulk-deactivate`, `bulk-delete` with body `{ ids: Guid[] }`. Same pattern for other entities (categories, customers, etc.) as needed.
- **Files:**  
  New commands: `BulkActivateProductsCommand`, etc.;  
  Controllers: new actions.

### Frontend
- **Checkbox column** – Add to data tables (e.g. products list); selection state.
- **Bulk bar** – When selection not empty: “Activate”, “Deactivate”, “Delete”, “Export” calling bulk APIs.
- **Files:**  
  `shared/components/data-table/*` (optional checkbox column + selection events),  
  Feature list components (e.g. products-list) for bulk bar and API calls.

---

## 7. Activity Heatmap – **TO DO**

### Backend
- **Endpoint** – e.g. `GET api/analytics/activity-heatmap?from=&to=` returning counts grouped by day (and optionally entity/user).
- **Files:**  
  New query in Analytics (or new controller);  
  DTO: list of { date, count } or similar.

### Frontend
- **Calendar heatmap** – GitHub-style; use a library (e.g. ngx-calendar-heatmap or custom grid); bind to API.

---

## 8. Recycle Bin – **TO DO**

### Backend
- **List deleted** – Queries that return soft-deleted items (e.g. `GetDeletedProductsQuery`) with `IsDeleted == true`.
- **Restore** – RestoreProductCommand, etc., setting `IsDeleted = false` and clearing `DeletedAt`/`DeletedBy`.
- **Files:**  
  Application commands/queries for each entity;  
  Controller actions, e.g. `api/products/deleted`, `api/products/{id}/restore`.

### Frontend
- **Recycle bin page** – Route e.g. `/recycle-bin`; list deleted items by type; “Restore” button calling restore API.

---

## 9. Product Tour – **TO DO**

### Frontend
- **Library** – e.g. intro.js or shepherd.js (or Angular-specific wrapper).
- **First-login tips** – Show tour when user has a “first login” or “tour dismissed” flag.
- **User setting** – e.g. “Disable product tour” in user/settings; persist and skip tour when set.
- **Files:**  
  New tour service/component;  
  User settings model + API for “disable tour”;  
  Call tour from app component or main layout when conditions met.

---

## Summary

| Feature              | Backend      | Frontend     |
|----------------------|-------------|-------------|
| Global Search        | Done        | Done        |
| Automation Engine    | Entity + IF | CRUD + UI   |
| Inventory Timeline   | To do       | To do       |
| Executive Dashboard  | To do       | To do       |
| Product Intelligence | To do       | To do       |
| Bulk Actions         | To do       | To do       |
| Activity Heatmap     | To do       | To do       |
| Recycle Bin          | To do       | To do       |
| Product Tour         | –           | To do       |

**Apply migrations** (after stopping any process that locks the API/Infrastructure DLLs):

```bash
dotnet ef database update --project Khidmah_Inventory.Infrastructure --startup-project Khidmah_Inventory.API
```

This will apply both `AddNotificationsTable` and `AddAutomationRules` if not already applied.
