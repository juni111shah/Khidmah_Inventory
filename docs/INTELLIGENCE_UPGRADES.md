# Intelligence upgrades (analyze → predict → recommend → act)

This document describes the intelligence layer added to supercharge existing screens using **free, math/statistics-based logic** (no paid AI). All integrations follow existing patterns: ApiResponse, RBAC, multi-tenant, SignalR where applicable.

---

## Delivered

### 1. Product detail – intelligence panel

- **Backend**: `GetProductIntelligenceQuery` returns for a product:
  - **Sales velocity** (units/day over last N days)
  - **Stock days remaining** (total stock / velocity)
  - **Reorder risk** (Critical / High / Medium / Low from reorder point and days left)
  - **Margin trend** (Up / Down / Stable vs previous period)
  - **Price history** (last 10 distinct dates from sales)
  - **ABC classification** (A/B/C by revenue share over last 90 days)
  - **Recommended actions** (e.g. Reorder now, Review price, Ensure stock, Consider promotion)
- **Frontend**: `ProductIntelligencePanelComponent` in product form (right column when viewing/editing a product). Uses `TrendArrowComponent` and `SeverityBadgeComponent`. Quick actions: Reorder, Stock levels.

### 2. Dashboard – predictions, anomalies, risks, opportunities

- **Backend**: `GetDashboardIntelligenceQuery` returns:
  - **Predictions**: e.g. sales (next 7 days from 7d average), low stock count
  - **Anomalies**: e.g. today’s sales significantly above/below daily average
  - **Risks**: low stock count, pending POs
  - **Opportunities**: top seller, reorder suggestions
- **Frontend**: Dashboard loads intelligence and shows an “Analyze → Predict → Act” card with four columns (Predictions, Anomalies, Risks, Opportunities), severity styling, and drill-down/action links.

### 3. Reports – period comparison and variance

- **Backend**: `SalesReportDto` extended with:
  - `PreviousPeriodFromDate`, `PreviousPeriodToDate`
  - `PreviousPeriodTotalSales`, `PreviousPeriodTotalCost`, `PreviousPeriodTotalProfit`
  - `VarianceSalesPercent`, `VarianceProfitPercent`
  - `TrendDirection` (Up / Down / Stable)
- **Handler**: `GetSalesReportQueryHandler` computes the previous period (same length, immediately before the selected range) and fills these fields. Frontend can show “vs previous period” and highlight variance/trend.

### 4. Shared components and API

- **API**: `IntelligenceController` at `api/intelligence`:
  - `GET product/{productId}?daysForVelocity=30` – product intelligence
  - `GET dashboard?predictionDays=7` – dashboard intelligence
- **Angular**: `IntelligenceApiService`, `intelligence.model.ts`, `TrendArrowComponent`, `SeverityBadgeComponent`, `ProductIntelligencePanelComponent`.

---

## Extending further (same pattern)

- **Inventory**: Add `GetInventoryIntelligenceQuery` (inflow/outflow from `StockTransaction`, aging from `StockLevel.LastUpdated`, dead stock, carrying cost). Expose at `GET intelligence/inventory` and add an intelligence card/section to the stock-levels or inventory page.
- **Warehouse**: Add `GetWarehouseMetricsQuery` (utilization, transfer counts, ranking). Expose at `GET intelligence/warehouse-metrics` and add a metrics block to the warehouse list or detail.
- **Purchase / suppliers**: Add `GetSupplierAnalyticsQuery` (on-time from PO dates, lead time, price trend from PO items). Expose and add to supplier or purchase order screens.
- **Sales**: Add margin view and discount effect to existing analytics or a new Sales Intelligence query; cross-sell from order-item co-occurrence.
- **POS**: Add `GetPosHintsQuery` (upsell from frequently bought together, stock warnings for cart items, bundle ideas). Call from POS when cart changes and show hints in the UI.
- **Customers**: Add `GetCustomerIntelligenceQuery` (LTV, orders/month, days since last order for churn risk, preferred categories/products). Expose and add to customer list/detail.

All of the above can reuse the same DTOs in `Application/Features/Intelligence/Models` and the same style: badges, trend arrows, drill-down and quick action buttons, responsive layout.

---

## Real-time (SignalR)

Existing dashboard already subscribes to `DashboardUpdated`, `StockChanged`, `SaleCompleted`, `OrderCreated`, `OrderApproved`. The new intelligence data is loaded on dashboard load and when the user navigates to the product form. To push intelligence updates in real time, you can:

- From the same SignalR hub, broadcast a “IntelligenceUpdated” event (e.g. after stock or order changes) and have the dashboard/product form refresh their intelligence payload when they receive it, or
- Keep the current approach (refresh on focus or on interval) to avoid extra server work.

---

## UX checklist

- Badges: used for reorder risk, severity, ABC, trend.
- Severity: Low/Medium/High/Critical for risks and anomalies.
- Trend arrows: Up/Down/Stable for margin and predictions.
- Drill-down: links to reports, inventory, reorder, products.
- Quick actions: Reorder, Stock levels, Act, View buttons where relevant.
