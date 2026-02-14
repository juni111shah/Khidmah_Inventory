# Advanced Filtering & Table State Framework

Reusable infrastructure for **global search**, **filter builder**, **saved views**, **table state persistence** (URL + localStorage), **column visibility**, and **live refresh** across list pages.

## Core pieces

| Item | Location | Purpose |
|------|----------|--------|
| **TableStateService** | `core/services/table-state.service.ts` | Per-table state; persist/restore URL + localStorage |
| **SavedViewsService** | `core/services/saved-views.service.ts` | Save/load/rename/delete/set-default views per tableId |
| **Table state types** | `core/models/table-state.model.ts` | `TableState`, `SavedView`, `FilterGroup`, `FilterRule`, `SortColumn`, helpers |
| **FilterBuilderComponent** | `shared/components/filter-builder/` | AND/OR rules: equals, contains, &gt;/&lt;, between, in list → `FilterDto[]` |
| **SavedViewsDropdownComponent** | `shared/components/saved-views-dropdown/` | UI: load view, save current, set default, rename, delete |
| **DataTableComponent** | `shared/components/data-table/` | Syncs `currentSort` from `filterRequest.pagination` (ngOnChanges) |

## Example: products list

See `features/products/products-list/`:

1. **Table state** – `tableId = 'products'`; in `ngOnInit`: `restoreTableState()` from `TableStateService.getState(tableId)`; on filter/sort/page/column toggle: `persistTableState()`.
2. **Debounced global search** – `searchTermChange` → `Subject` + `debounceTime(300)` → update `filterRequest.search.term` and `loadProducts()`; `search` (Enter) → immediate load.
3. **Filter builder** – Tab “Filter builder” in filter panel; `[columns]="filterBuilderColumns"`, `(apply)="onFilterBuilderApply($event)"`, `(clear)="onFilterBuilderClear()"`. Columns use API names (e.g. `Name`, `Sku`, `PurchasePrice`).
4. **Saved views** – `<app-saved-views-dropdown [tableId]="tableId" [currentState]="currentTableState" (loadView)="onSavedViewLoad($event)">`. Apply loaded state to `filterRequest`, sort, visible columns, then `loadProducts()` and `tableState.patchState()`.
5. **SignalR** – `signalR.getProductUpdated().subscribe(() => this.loadProducts())` so the list refreshes when products change.
6. **Export** – Existing export already uses `[filters]="filterRequest.filters"` and current data; export reflects current view.

## Reusing on another list

1. Give the list a **tableId** (e.g. `'orders'`).
2. Inject **TableStateService**, **SavedViewsService**; optionally **SignalRService** for live refresh.
3. **Restore**: In `ngOnInit`, call `tableState.getState(tableId)` and apply `filterRequest`, `sortColumns` (→ `pagination.sortBy`/`sortOrder`), `visibleColumnKeys` (→ column.visible).
4. **Persist**: On filter/sort/page/column change, call `tableState.patchState(tableId, { filterRequest, sortColumns, visibleColumnKeys })`.
5. Add **SavedViewsDropdownComponent** with `[tableId]`, `[currentState]`, `(loadView)`.
6. Add **FilterBuilderComponent** (or reuse existing filter panel); define `FilterBuilderColumn[]` with keys matching your API.
7. Optional: **Debounced search** via `Subject` + `debounceTime(300)` on search term input.
8. Optional: **SignalR** subscribe to the relevant event and call your `loadData()`.

All solutions used are **free** (no paid libraries). UX: clean, fast, keyboard-friendly (Enter to search, standard focus).
