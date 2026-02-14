# Skeleton Loading System

Professional, reusable skeleton placeholders used across the app. CSS + Angular only (no paid libraries). Layouts match real UI for better perceived performance.

## What the skeletons represent (actual content)

- **Card grid (e.g. first reference)** – Each card is a **list/grid item**: product, customer, category, or dashboard widget. The circle = avatar/icon, short lines = title & subtitle, longer lines = description, small rectangle = button/tag/value. Used for product lists, customer lists, or a grid of stat/summary cards.
- **Dashboard (e.g. second reference)** – **Top**: quick action buttons (New Sale, Add Product, Purchase Order, Customer). **Middle**: four **stat cards** (Products, Warehouses, Stock Value, Low Stock) with icon, value, trend. **Below**: “Analyze → Predict → Act” panel with **Predictions**, **Risks**, **Opportunities** (headings, text lines, tags, buttons). Skeletons mirror this layout so placeholders match size and spacing of the real dashboard.

## Global wrapper: `app-content-loader`

Use this to switch between skeleton and real content with a fade transition and minimal layout jump:

```html
<app-content-loader [loading]="isLoading">
  <ng-container skeleton>
    <!-- Skeleton placeholders that match this page -->
    <app-skeleton-stat-cards [cardCount]="4"></app-skeleton-stat-cards>
    <app-skeleton-table [rows]="10" [columns]="5"></app-skeleton-table>
  </ng-container>
  <!-- Real content (shown when loading is false) -->
  <div class="real-content">
    ...
  </div>
</app-content-loader>
```

- **`[loading]`** – when `true`, the `skeleton` slot is shown; when `false`, the default slot (real content) is shown.
- **`[preserveMinHeight]`** (optional, default `true`) – keeps a min-height on the skeleton wrapper to reduce layout shift.

## Skeleton components

| Component | Selector | Main inputs |
|-----------|----------|-------------|
| **skeleton-table** | `app-skeleton-table` | `rows`, `columns`, `showHeader`, `headerWidths` |
| **skeleton-form** | `app-skeleton-form` | `fieldCount`, `showLabels`, `showActions`, `fieldHeight` |
| **skeleton-detail-header** | `app-skeleton-detail-header` | `showTitle`, `titleWidth`, `actionCount` |
| **skeleton-stat-cards** | `app-skeleton-stat-cards` | `cardCount` |
| **skeleton-chart** | `app-skeleton-chart` | `titleWidth`, `showSubtitle`, `chartHeight` |
| **skeleton-list** | `app-skeleton-list` | `itemCount`, `showAvatar`, `layout`, `gridColumns` |
| **skeleton-side-panel** | `app-skeleton-side-panel` | `lineCount`, `showBlock` |
| **skeleton-activity-feed** | `app-skeleton-activity-feed` | `activityCount` |
| **skeleton-listing-header** | `app-skeleton-listing-header` | `buttonCount`, `buttonWidth` |

All skeletons support **`[animation]="'shimmer'"`** or **`'pulse'`** (default: `shimmer`).

## Global theme

Variables and keyframes live in `app/shared/styles/_skeleton-theme.scss` (imported in `styles.scss`):

- `--skeleton-base`, `--skeleton-highlight`, `--skeleton-border`, `--skeleton-radius`
- Shimmer and pulse keyframes for a soft, consistent look

## Usage across the app

Skeleton loaders are used on all major list, form, and dashboard-style pages:

- **List pages**: Products, Categories, Customers, Suppliers, Warehouses, Users, Roles, Purchase Orders, Sales Orders, Stock Levels (listing header + table skeleton).
- **Dashboard-style**: Dashboard, Command Center (quick actions + stat cards + content cards/list skeletons).
- **Form pages**: Product, Category, Customer, Supplier, Warehouse, Purchase Order, Sales Order, User, Role, Company (detail header + form skeleton).
- **Other**: Daily Briefing, Intelligence (e.g. Profit, Predictive Risk, Decision Support), Reorder, Reports, Analytics, Approval Inbox, Workflow list, Automation list, Company list/users (skeleton type chosen to match layout).

## Responsive behaviour

Skeleton layout uses the same grid/utility classes as real content (e.g. `row`, `col-6`, `col-md-3`) so spacing and breakpoints stay aligned.
