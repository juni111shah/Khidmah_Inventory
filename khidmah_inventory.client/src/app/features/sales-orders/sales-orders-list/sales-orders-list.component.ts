import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { Subject, Subscription } from 'rxjs';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { SalesOrder, GetSalesOrdersListQuery } from '../../../core/models/sales-order.model';
import { DataTableComponent } from '../../../shared/components/data-table/data-table.component';
import { DataTableColumn, DataTableAction, DataTableConfig } from '../../../shared/models/data-table.model';
import { ToastComponent } from '../../../shared/components/toast/toast.component';
import { HasPermissionDirective } from '../../../shared/directives/has-permission.directive';
import { IconComponent } from '../../../shared/components/icon/icon.component';
import { PermissionService } from '../../../core/services/permission.service';
import { ListingContainerComponent } from '../../../shared/components/listing-container/listing-container.component';
import { UnifiedButtonComponent } from '../../../shared/components/unified-button/unified-button.component';
import { HeaderService } from '../../../core/services/header.service';
import { FilterFieldComponent } from '../../../shared/components/filter-field/filter-field.component';
import { FilterPanelComponent, FilterPanelConfig, FilterPanelField } from '../../../shared/components/filter-panel/filter-panel.component';
import { ExportComponent } from '../../../shared/components/export/export.component';
import { TableStateService } from '../../../core/services/table-state.service';
import { SavedViewsService } from '../../../core/services/saved-views.service';
import { FilterBuilderComponent } from '../../../shared/components/filter-builder/filter-builder.component';
import { FilterBuilderColumn } from '../../../core/models/table-state.model';
import { SavedViewsDropdownComponent } from '../../../shared/components/saved-views-dropdown/saved-views-dropdown.component';
import { ContentLoaderComponent } from '../../../shared/components/content-loader/content-loader.component';
import { SkeletonListingHeaderComponent } from '../../../shared/components/skeleton-listing-header/skeleton-listing-header.component';
import { SkeletonTableComponent } from '../../../shared/components/skeleton-table/skeleton-table.component';
import { FilterRequest, FilterDto, SearchMode } from '../../../core/models/user.model';
import { TableState, SortColumn } from '../../../core/models/table-state.model';
import { RealtimeSyncService } from '../../../core/services/realtime-sync.service';
import { OrderStore } from '../../../core/stores/order.store';

@Component({
  selector: 'app-sales-orders-list',
  standalone: true,
  imports: [
    CommonModule, FormsModule, ReactiveFormsModule, DataTableComponent, ToastComponent, HasPermissionDirective, IconComponent,
    ListingContainerComponent, UnifiedButtonComponent, FilterFieldComponent, FilterPanelComponent, ExportComponent,
    FilterBuilderComponent, SavedViewsDropdownComponent,
    ContentLoaderComponent, SkeletonListingHeaderComponent, SkeletonTableComponent
  ],
  templateUrl: './sales-orders-list.component.html'
})
export class SalesOrdersListComponent implements OnInit, OnDestroy {
  readonly tableId = 'sales-orders';
  salesOrders: SalesOrder[] = [];
  loading = false;
  searchTerm = '';
  statusFilter: string | null = null;
  private searchSubject = new Subject<string>();
  private realtimeRefreshSubject = new Subject<void>();
  private subs = new Subscription();
  showFilterBuilder = false;
  private touchedAtById = new Map<string, string>();
  pagedResult: any = null;
  filterRequest: FilterRequest = {
    pagination: { pageNo: 1, pageSize: 10, sortBy: 'orderDate', sortOrder: 'descending' },
    search: { term: '', searchFields: ['orderNumber', 'customerName'], mode: SearchMode.Contains, isCaseSensitive: false },
    filters: []
  };

  columns: DataTableColumn<SalesOrder>[] = [
    { key: 'orderNumber', label: 'Order #', sortable: true, searchable: true, filterable: true, width: '150px' },
    { key: 'customerName', label: 'Customer', sortable: true, searchable: true, filterable: true, width: '200px' },
    { key: 'orderDate', label: 'Order Date', sortable: true, filterable: false, type: 'date', width: '120px', format: (value) => new Date(value).toLocaleDateString() },
    { 
      key: 'status', 
      label: 'Status', 
      sortable: true, 
      filterable: true,
      filterType: 'select',
      filterOptions: [
        { label: 'All', value: '' },
        { label: 'Draft', value: 'Draft' },
        { label: 'Confirmed', value: 'Confirmed' },
        { label: 'Partially Delivered', value: 'PartiallyDelivered' },
        { label: 'Delivered', value: 'Delivered' }
      ],
      type: 'badge', 
      width: '120px' 
    },
    { key: 'totalAmount', label: 'Total', sortable: true, filterable: false, type: 'number', width: '120px', format: (value) => `$${value.toFixed(2)}` }
  ];

  actions: DataTableAction<SalesOrder>[] = [
    { label: 'View', icon: 'eye', action: (row) => this.viewOrder(row), condition: () => this.permissionService.hasPermission('SalesOrders:Read') }
  ];

  config: DataTableConfig = {
    showSearch: false,
    showFilters: false,
    showColumnToggle: false,
    showPagination: true,
    showActions: true,
    showCheckbox: false,
    pageSize: 10,
    pageSizeOptions: [5, 10, 25, 50, 100],
    searchPlaceholder: 'Search sales orders...',
    emptyMessage: 'No sales orders found',
    loadingMessage: 'Loading sales orders...',
    rowClass: (row: SalesOrder) => this.getRealtimeRowClass(row.id)
  };

  showToast = false;
  toastMessage = '';
  toastType: 'success' | 'error' | 'warning' | 'info' = 'success';

  get visibleColumnsCount(): number {
    return this.columns.filter(c => c.visible !== false).length;
  }

  showColumnToggle = false;
  showFilterPanel = false;

  // Filter panel configuration
  filterPanelConfig: FilterPanelConfig = {
    fields: [
      {
        key: 'status',
        label: 'Status',
        type: 'select',
        placeholder: 'All Statuses',
        colSize: 'col-md-6',
        defaultValue: null,
        options: [
          { label: 'All Statuses', value: null },
          { label: 'Draft', value: 'Draft' },
          { label: 'Confirmed', value: 'Confirmed' },
          { label: 'Partially Delivered', value: 'PartiallyDelivered' },
          { label: 'Delivered', value: 'Delivered' }
        ]
      }
    ],
    showResetButton: true,
    showApplyButton: true,
    layout: 'row'
  };

  filterPanelValues: { [key: string]: any } = { status: null };

  filterBuilderColumns: FilterBuilderColumn[] = [
    { key: 'orderNumber', label: 'Order #', type: 'text' },
    { key: 'customerName', label: 'Customer', type: 'text' },
    { key: 'orderDate', label: 'Order Date', type: 'date' },
    { key: 'status', label: 'Status', type: 'text' },
    { key: 'totalAmount', label: 'Total', type: 'number' }
  ];

  get currentTableState(): { filterRequest: FilterRequest; sortColumns: { column: string; direction: string }[]; visibleColumnKeys: string[] } {
    const sortColumns = this.filterRequest.pagination?.sortBy ? [{ column: this.filterRequest.pagination.sortBy, direction: this.filterRequest.pagination.sortOrder === 'descending' ? 'desc' : 'asc' }] : [];
    return { filterRequest: this.filterRequest, sortColumns, visibleColumnKeys: this.columns.filter(c => c.visible !== false).map(c => c.key) };
  }

  constructor(
    public router: Router,
    public permissionService: PermissionService,
    private headerService: HeaderService,
    private tableState: TableStateService,
    private savedViews: SavedViewsService,
    private realtimeSync: RealtimeSyncService,
    private orderStore: OrderStore
  ) {}

  ngOnInit(): void {
    this.headerService.setHeaderInfo({ title: 'Sales Orders', description: 'Manage client orders and deliveries' });
    this.initializeFilterPanel();
    this.restoreTableState();
    this.loadSalesOrders();
    this.searchSubject.pipe(debounceTime(300), distinctUntilChanged()).subscribe(term => {
      this.searchTerm = term;
      if (this.filterRequest.search) this.filterRequest.search.term = term;
      if (this.filterRequest.pagination) this.filterRequest.pagination.pageNo = 1;
      this.persistTableState();
      this.loadSalesOrders();
    });
    this.subs.add(this.realtimeSync.watchEntityType('SalesOrder').subscribe(() => this.realtimeRefreshSubject.next()));
    this.subs.add(this.realtimeSync.watchEntityType('WorkflowInstance').subscribe(() => this.realtimeRefreshSubject.next()));
    this.subs.add(this.realtimeRefreshSubject.pipe(debounceTime(220)).subscribe(() => this.loadSalesOrders()));
    this.subs.add(this.orderStore.touchedAt$.subscribe(map => { this.touchedAtById = map; }));
  }

  ngOnDestroy(): void { this.subs.unsubscribe(); }

  private restoreTableState(): void {
    const state = this.tableState.getState(this.tableId);
    if (!state) return;
    if (state.filterRequest?.pagination) this.filterRequest.pagination = { ...this.filterRequest.pagination, ...state.filterRequest.pagination };
    if (state.filterRequest?.search?.term != null) {
      if (!this.filterRequest.search) this.filterRequest.search = { term: '', searchFields: ['orderNumber', 'customerName'], mode: SearchMode.Contains, isCaseSensitive: false };
      this.filterRequest.search.term = state.filterRequest.search.term;
      this.searchTerm = state.filterRequest.search.term;
    }
    if (state.filterRequest?.filters?.length) this.filterRequest.filters = [...state.filterRequest.filters];
    if (state.sortColumns?.length && this.filterRequest.pagination) {
      this.filterRequest.pagination.sortBy = state.sortColumns[0].column;
      this.filterRequest.pagination.sortOrder = state.sortColumns[0].direction === 'asc' ? 'ascending' : 'descending';
    }
    if (state.visibleColumnKeys?.length) this.columns.forEach(col => { col.visible = state.visibleColumnKeys!.includes(col.key); });
  }

  private persistTableState(): void {
    this.tableState.patchState(this.tableId, {
      filterRequest: this.filterRequest,
      sortColumns: this.filterRequest.pagination?.sortBy ? [{ column: this.filterRequest.pagination.sortBy, direction: this.filterRequest.pagination.sortOrder === 'descending' ? 'desc' : 'asc' }] : [],
      visibleColumnKeys: this.columns.filter(c => c.visible !== false).map(c => c.key)
    });
  }

  private initializeFilterPanel(): void {}

  onSearchTermChange(term: string): void { this.searchSubject.next(term ?? ''); }
  onSearchImmediate(term: string): void {
    this.searchTerm = term ?? '';
    if (this.filterRequest.search) this.filterRequest.search.term = term ?? '';
    if (this.filterRequest.pagination) this.filterRequest.pagination.pageNo = 1;
    this.persistTableState();
    this.loadSalesOrders();
  }

  onFilterToggle(): void {
    this.showFilterPanel = !this.showFilterPanel;
  }

  // Filter panel event handlers
  onFilterApplied(filters: { [key: string]: any }): void {
    this.statusFilter = filters['status'] || null;

    if (this.filterRequest.pagination) {
      this.filterRequest.pagination.pageNo = 1;
    }
    this.loadSalesOrders();
  }

  onFilterReset(): void {
    this.statusFilter = null;
    this.searchTerm = '';
    if (this.filterRequest.search) {
      this.filterRequest.search.term = '';
    }
    if (this.filterRequest.pagination) {
      this.filterRequest.pagination.pageNo = 1;
    }
    this.loadSalesOrders();
  }

  onFilterClose(): void {
    this.showFilterPanel = false;
  }

  resetFilters(): void {
    this.onFilterReset();
  }

  onColumnsToggle(): void {
    this.showColumnToggle = !this.showColumnToggle;
  }

  onColumnToggle(column: DataTableColumn<SalesOrder>): void {
    column.visible = column.visible === false ? true : false;
    this.persistTableState();
  }

  addSalesOrder(): void {
    this.router.navigate(['/sales-orders/new']);
  }

  loadSalesOrders(): void {
    this.loading = true;
    const query: GetSalesOrdersListQuery = {
      filterRequest: this.filterRequest as any,
      status: this.statusFilter || undefined
    };

    this.orderStore.fetchSales(query).subscribe({
      next: (response) => {
        if (response.success && response.data) {
          this.pagedResult = response.data;
          this.salesOrders = response.data.items;
        } else {
          this.showToastMessage('error', response.message || 'Failed to load sales orders');
        }
        this.loading = false;
      },
      error: () => {
        this.showToastMessage('error', 'Error loading sales orders');
        this.loading = false;
      }
    });
  }

  onFilterChange(filterRequest?: FilterRequest): void {
    if (filterRequest) this.filterRequest = filterRequest;
    this.persistTableState();
    this.loadSalesOrders();
  }
  onPageChange(pageNo: number): void {
    if (this.filterRequest.pagination) this.filterRequest.pagination.pageNo = pageNo;
    this.persistTableState();
    this.loadSalesOrders();
  }
  onSortChange(sort: any): void {
    if (this.filterRequest.pagination) {
      this.filterRequest.pagination.sortBy = sort.column;
      this.filterRequest.pagination.sortOrder = sort.direction === 'asc' ? 'ascending' : 'descending';
    }
    this.persistTableState();
    this.loadSalesOrders();
  }
  onSavedViewLoad(state: Partial<{ filterRequest: FilterRequest; sortColumns: { column: string; direction: string }[]; visibleColumnKeys: string[] }>): void {
    if (state.filterRequest) {
      this.filterRequest = { ...this.filterRequest, ...state.filterRequest };
      if (state.filterRequest.pagination) this.filterRequest.pagination = { ...this.filterRequest.pagination, ...state.filterRequest.pagination };
      if (state.filterRequest.search) { this.filterRequest.search = { ...this.filterRequest.search, ...state.filterRequest.search }; this.searchTerm = this.filterRequest.search.term ?? ''; }
      if (state.filterRequest.filters) this.filterRequest.filters = [...state.filterRequest.filters];
    }
    if (state.sortColumns?.length && this.filterRequest.pagination) {
      this.filterRequest.pagination!.sortBy = state.sortColumns[0].column;
      this.filterRequest.pagination!.sortOrder = state.sortColumns[0].direction === 'asc' ? 'ascending' : 'descending';
    }
    if (state.visibleColumnKeys?.length) this.columns.forEach(col => { col.visible = state.visibleColumnKeys!.includes(col.key); });
    const patch: Partial<TableState> = { filterRequest: state.filterRequest, visibleColumnKeys: state.visibleColumnKeys };
    if (state.sortColumns?.length) patch.sortColumns = state.sortColumns.map(s => ({ column: s.column, direction: (s.direction === 'desc' ? 'desc' : 'asc') as SortColumn['direction'] }));
    this.tableState.patchState(this.tableId, patch);
    this.loadSalesOrders();
  }
  onFilterBuilderApply(filters: FilterDto[]): void {
    this.filterRequest.filters = filters;
    if (this.filterRequest.pagination) this.filterRequest.pagination.pageNo = 1;
    this.persistTableState();
    this.loadSalesOrders();
    this.showFilterBuilder = false;
  }
  onFilterBuilderClear(): void {
    this.filterRequest.filters = [];
    this.persistTableState();
    this.loadSalesOrders();
  }

  viewOrder(order: SalesOrder): void {
    this.router.navigate(['/sales-orders', order.id]);
  }


  showToastMessage(type: 'success' | 'error' | 'warning' | 'info', message: string): void {
    this.toastType = type;
    this.toastMessage = message;
    this.showToast = true;
    setTimeout(() => { this.showToast = false; }, 3000);
  }

  private getRealtimeRowClass(rowId: string): string {
    const touchedAt = this.touchedAtById.get(rowId);
    if (!touchedAt) return '';
    return Date.now() - new Date(touchedAt).getTime() <= 8000 ? 'row-realtime-updated' : '';
  }
}



