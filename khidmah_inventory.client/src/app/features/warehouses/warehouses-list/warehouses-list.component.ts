import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { Subject, Subscription } from 'rxjs';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { WarehouseApiService } from '../../../core/services/warehouse-api.service';
import { Warehouse, GetWarehousesListQuery } from '../../../core/models/warehouse.model';
import { DataTableComponent } from '../../../shared/components/data-table/data-table.component';
import { DataTableColumn, DataTableAction, DataTableConfig } from '../../../shared/models/data-table.model';
import { ToastComponent } from '../../../shared/components/toast/toast.component';
import { PermissionService } from '../../../core/services/permission.service';
import { HasPermissionDirective } from '../../../shared/directives/has-permission.directive';
import { IconComponent } from '../../../shared/components/icon/icon.component';
import { FilterRequest, FilterDto, SearchMode } from '../../../core/models/user.model';
import { FormFieldComponent, FormFieldOption } from '../../../shared/components/form-field/form-field.component';
import { UnifiedButtonComponent } from '../../../shared/components/unified-button/unified-button.component';
import { ListingContainerComponent } from '../../../shared/components/listing-container/listing-container.component';
import { FilterFieldComponent } from '../../../shared/components/filter-field/filter-field.component';
import { FilterPanelComponent, FilterPanelConfig, FilterPanelField } from '../../../shared/components/filter-panel/filter-panel.component';
import { ExportComponent } from '../../../shared/components/export/export.component';
import { HeaderService } from '../../../core/services/header.service';
import { TableStateService } from '../../../core/services/table-state.service';
import { SavedViewsService } from '../../../core/services/saved-views.service';
import { FilterBuilderComponent } from '../../../shared/components/filter-builder/filter-builder.component';
import { FilterBuilderColumn } from '../../../core/models/table-state.model';
import { SavedViewsDropdownComponent } from '../../../shared/components/saved-views-dropdown/saved-views-dropdown.component';
import { ContentLoaderComponent } from '../../../shared/components/content-loader/content-loader.component';
import { SkeletonListingHeaderComponent } from '../../../shared/components/skeleton-listing-header/skeleton-listing-header.component';
import { SkeletonTableComponent } from '../../../shared/components/skeleton-table/skeleton-table.component';
import { TableState, SortColumn } from '../../../core/models/table-state.model';

@Component({
  selector: 'app-warehouses-list',
  standalone: true,
  imports: [
    CommonModule, FormsModule, ReactiveFormsModule,
    DataTableComponent, ToastComponent, HasPermissionDirective, IconComponent, FormFieldComponent,
    UnifiedButtonComponent, ListingContainerComponent, FilterFieldComponent, FilterPanelComponent, ExportComponent,
    FilterBuilderComponent, SavedViewsDropdownComponent,
    ContentLoaderComponent, SkeletonListingHeaderComponent, SkeletonTableComponent
  ],
  templateUrl: './warehouses-list.component.html'
})
export class WarehousesListComponent implements OnInit, OnDestroy {
  readonly tableId = 'warehouses';
  warehouses: Warehouse[] = [];
  loading = false;
  searchTerm = '';
  isActiveFilter: any = '';
  private searchSubject = new Subject<string>();
  private subs = new Subscription();
  showFilterBuilder = false;

  columns: DataTableColumn<Warehouse>[] = [
    // ... existing columns ...
    {
      key: 'name',
      label: 'Name',
      sortable: true,
      searchable: true,
      filterable: true,
      width: '200px'
    },
    {
      key: 'code',
      label: 'Code',
      sortable: true,
      searchable: true,
      filterable: true,
      width: '120px'
    },
    {
      key: 'city',
      label: 'City',
      sortable: true,
      filterable: true,
      width: '120px'
    },
    {
      key: 'country',
      label: 'Country',
      sortable: true,
      filterable: true,
      width: '120px'
    },
    {
      key: 'zoneCount',
      label: 'Zones',
      sortable: true,
      filterable: false,
      type: 'number',
      width: '80px'
    },
    {
      key: 'isDefault',
      label: 'Default',
      sortable: true,
      filterable: true,
      filterType: 'select',
      filterOptions: [
        { label: 'All', value: '' },
        { label: 'Yes', value: true },
        { label: 'No', value: false }
      ],
      type: 'badge',
      width: '100px',
      render: (row) => row.isDefault ? 'Yes' : 'No'
    },
    {
      key: 'isActive',
      label: 'Status',
      sortable: true,
      filterable: true,
      filterType: 'select',
      filterOptions: [
        { label: 'All', value: '' },
        { label: 'Active', value: true },
        { label: 'Inactive', value: false }
      ],
      type: 'badge',
      width: '100px',
      render: (row) => row.isActive ? 'Active' : 'Inactive'
    },
    {
      key: 'createdAt',
      label: 'Created',
      sortable: true,
      filterable: false,
      type: 'date',
      width: '150px',
      format: (value) => new Date(value).toLocaleDateString()
    }
  ];

  actions: DataTableAction<Warehouse>[] = [
    {
      label: 'View',
      icon: 'eye',
      action: (row) => this.viewWarehouse(row),
      tooltip: 'View warehouse details',
      condition: () => this.permissionService.hasPermission('Warehouses:Read')
    },
    {
      label: 'Edit',
      icon: 'pencil',
      action: (row) => this.editWarehouse(row),
      tooltip: 'Edit warehouse',
      condition: () => this.permissionService.hasPermission('Warehouses:Update')
    },
    {
      label: 'Deactivate',
      icon: 'slash-circle',
      action: (row) => this.deactivateWarehouse(row),
      condition: (row) => row.isActive && this.permissionService.hasPermission('Warehouses:Update'),
      tooltip: 'Deactivate warehouse'
    },
    {
      label: 'Activate',
      icon: 'check-circle',
      action: (row) => this.activateWarehouse(row),
      condition: (row) => !row.isActive && this.permissionService.hasPermission('Warehouses:Update'),
      tooltip: 'Activate warehouse'
    },
    {
      label: 'Delete',
      icon: 'trash',
      action: (row) => this.deleteWarehouse(row),
      condition: (row) => row.zoneCount === 0 && this.permissionService.hasPermission('Warehouses:Delete'),
      tooltip: 'Delete warehouse',
      class: 'danger'
    }
  ];

  pagedResult: any = null;
  filterRequest: FilterRequest = {
    pagination: { pageNo: 1, pageSize: 10, sortBy: 'name', sortOrder: 'ascending' },
    search: { term: '', searchFields: ['name', 'code', 'description', 'city', 'state', 'country'], mode: SearchMode.Contains, isCaseSensitive: false },
    filters: []
  };

  config: DataTableConfig = {
    showSearch: false,
    showFilters: false,
    showColumnToggle: false,
    showPagination: true,
    showActions: true,
    showCheckbox: false,
    pageSize: 10,
    pageSizeOptions: [5, 10, 25, 50, 100],
    searchPlaceholder: 'Search warehouses...',
    emptyMessage: 'No warehouses found',
    loadingMessage: 'Loading warehouses...'
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
    fields: [],
    showResetButton: true,
    showApplyButton: true,
    layout: 'row'
  };

  filterPanelValues: { [key: string]: any } = {};

  filterBuilderColumns: FilterBuilderColumn[] = [
    { key: 'name', label: 'Name', type: 'text' },
    { key: 'code', label: 'Code', type: 'text' },
    { key: 'city', label: 'City', type: 'text' },
    { key: 'country', label: 'Country', type: 'text' },
    { key: 'zoneCount', label: 'Zones', type: 'number' },
    { key: 'isDefault', label: 'Default', type: 'boolean', options: [{ label: 'Yes', value: true }, { label: 'No', value: false }] },
    { key: 'isActive', label: 'Status', type: 'boolean', options: [{ label: 'Active', value: true }, { label: 'Inactive', value: false }] },
    { key: 'createdAt', label: 'Created', type: 'date' }
  ];

  get currentTableState(): { filterRequest: FilterRequest; sortColumns: { column: string; direction: string }[]; visibleColumnKeys: string[] } {
    const sortColumns = this.filterRequest.pagination?.sortBy ? [{ column: this.filterRequest.pagination.sortBy, direction: this.filterRequest.pagination.sortOrder === 'descending' ? 'desc' : 'asc' }] : [];
    return { filterRequest: this.filterRequest, sortColumns, visibleColumnKeys: this.columns.filter(c => c.visible !== false).map(c => c.key) };
  }

  constructor(
    private warehouseApiService: WarehouseApiService,
    public router: Router,
    public permissionService: PermissionService,
    private headerService: HeaderService,
    private tableState: TableStateService,
    private savedViews: SavedViewsService
  ) {}

  ngOnInit(): void {
    this.headerService.setHeaderInfo({ title: 'Warehouses', description: 'Manage physical storage locations' });
    this.initializeFilterPanel();
    this.restoreTableState();
    this.loadWarehouses();
    this.searchSubject.pipe(debounceTime(300), distinctUntilChanged()).subscribe(term => {
      this.searchTerm = term;
      if (this.filterRequest.search) this.filterRequest.search.term = term;
      if (this.filterRequest.pagination) this.filterRequest.pagination.pageNo = 1;
      this.persistTableState();
      this.loadWarehouses();
    });
  }

  ngOnDestroy(): void { this.subs.unsubscribe(); }

  private restoreTableState(): void {
    const state = this.tableState.getState(this.tableId);
    if (!state) return;
    if (state.filterRequest?.pagination) this.filterRequest.pagination = { ...this.filterRequest.pagination, ...state.filterRequest.pagination };
    if (state.filterRequest?.search?.term != null) {
      if (!this.filterRequest.search) this.filterRequest.search = { term: '', searchFields: ['name', 'code', 'description', 'city', 'state', 'country'], mode: SearchMode.Contains, isCaseSensitive: false };
      this.filterRequest.search.term = state.filterRequest.search.term;
      this.searchTerm = state.filterRequest.search.term;
      if (state.filterRequest.search.searchFields?.length) this.filterRequest.search.searchFields = state.filterRequest.search.searchFields;
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

  private initializeFilterPanel(): void {
    this.filterPanelConfig = {
      fields: [
        {
          key: 'name',
          label: 'Name',
          type: 'text',
          placeholder: 'Filter by name',
          colSize: 'col-md-6',
          defaultValue: ''
        },
        {
          key: 'code',
          label: 'Code',
          type: 'text',
          placeholder: 'Filter by code',
          colSize: 'col-md-6',
          defaultValue: ''
        },
        {
          key: 'city',
          label: 'City',
          type: 'text',
          placeholder: 'Filter by city',
          colSize: 'col-md-6',
          defaultValue: ''
        },
        {
          key: 'country',
          label: 'Country',
          type: 'text',
          placeholder: 'Filter by country',
          colSize: 'col-md-6',
          defaultValue: ''
        },
        {
          key: 'zoneCount',
          label: 'Zones',
          type: 'number',
          placeholder: 'Min zones',
          colSize: 'col-md-6',
          defaultValue: null
        },
        {
          key: 'isDefault',
          label: 'Default',
          type: 'select',
          placeholder: 'All',
          colSize: 'col-md-6',
          defaultValue: null,
          options: [
            { label: 'All', value: null },
            { label: 'Yes', value: true },
            { label: 'No', value: false }
          ]
        },
        {
          key: 'isActive',
          label: 'Status',
          type: 'select',
          placeholder: 'All Status',
          colSize: 'col-md-6',
          defaultValue: null,
          options: [
            { label: 'All Status', value: null },
            { label: 'Active', value: true },
            { label: 'Inactive', value: false }
          ]
        }
      ],
      showResetButton: true,
      showApplyButton: true,
      layout: 'row'
    };

    // Initialize filter values
    this.filterPanelValues = {
      name: '',
      code: '',
      city: '',
      country: '',
      zoneCount: null,
      isDefault: null,
      isActive: null
    };
  }

  onSearchTermChange(term: string): void { this.searchSubject.next(term ?? ''); }
  onSearchImmediate(term: string): void {
    this.searchTerm = term ?? '';
    if (this.filterRequest.search) this.filterRequest.search.term = term ?? '';
    if (this.filterRequest.pagination) this.filterRequest.pagination.pageNo = 1;
    this.persistTableState();
    this.loadWarehouses();
  }

  onFilterToggle(): void {
    this.showFilterPanel = !this.showFilterPanel;
  }

  // Filter panel event handlers
  onFilterApplied(filters: { [key: string]: any }): void {
    // Clear existing filters
    this.filterRequest.filters = [];

    // Apply filters for each field
    if (filters['name'] && filters['name'].trim()) {
      this.filterRequest.filters.push({
        column: 'Name',
        operator: 'Contains',
        value: filters['name'].trim()
      });
    }

    if (filters['code'] && filters['code'].trim()) {
      this.filterRequest.filters.push({
        column: 'Code',
        operator: 'Contains',
        value: filters['code'].trim()
      });
    }

    if (filters['city'] && filters['city'].trim()) {
      this.filterRequest.filters.push({
        column: 'City',
        operator: 'Contains',
        value: filters['city'].trim()
      });
    }

    if (filters['country'] && filters['country'].trim()) {
      this.filterRequest.filters.push({
        column: 'Country',
        operator: 'Contains',
        value: filters['country'].trim()
      });
    }

    if (filters['zoneCount'] && filters['zoneCount'] !== null) {
      this.filterRequest.filters.push({
        column: 'ZoneCount',
        operator: 'GreaterThanOrEqual',
        value: filters['zoneCount']
      });
    }

    // Handle specific filters for API compatibility
    this.isActiveFilter = filters['isActive'] !== undefined ? filters['isActive'] : null;

    if (this.filterRequest.pagination) {
      this.filterRequest.pagination.pageNo = 1;
    }
    this.loadWarehouses();
  }

  onFilterReset(): void {
    this.isActiveFilter = null;
    this.searchTerm = '';
    if (this.filterRequest.search) {
      this.filterRequest.search.term = '';
    }
    if (this.filterRequest.pagination) {
      this.filterRequest.pagination.pageNo = 1;
    }
    this.loadWarehouses();
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

  onColumnToggle(column: DataTableColumn<Warehouse>): void {
    column.visible = column.visible === false ? true : false;
    this.persistTableState();
  }

  addWarehouse(): void {
    this.router.navigate(['/warehouses/new']);
  }

  loadWarehouses(): void {
    this.loading = true;
    const filterRequest: GetWarehousesListQuery = {
      filterRequest: this.filterRequest,
      isActive: (this.isActiveFilter === '' || this.isActiveFilter === null) ? undefined : (this.isActiveFilter === 'true' || this.isActiveFilter === true)
    };

    this.warehouseApiService.getWarehouses(filterRequest).subscribe({
      next: (response) => {
        if (response.success && response.data) {
          this.pagedResult = response.data;
          this.warehouses = response.data.items;
        } else {
          this.showToastMessage('error', response.message || 'Failed to load warehouses');
        }
        this.loading = false;
      },
      error: () => {
        this.showToastMessage('error', 'Error loading warehouses');
        this.loading = false;
      }
    });
  }

  onFilterChange(filterRequest?: FilterRequest): void {
    if (filterRequest) this.filterRequest = filterRequest;
    this.persistTableState();
    this.loadWarehouses();
  }
  onPageChange(pageNo: number): void {
    if (this.filterRequest.pagination) this.filterRequest.pagination.pageNo = pageNo;
    this.persistTableState();
    this.loadWarehouses();
  }
  onSortChange(sort: any): void {
    if (this.filterRequest.pagination) {
      this.filterRequest.pagination.sortBy = sort.column;
      this.filterRequest.pagination.sortOrder = sort.direction === 'asc' ? 'ascending' : 'descending';
    }
    this.persistTableState();
    this.loadWarehouses();
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
    this.loadWarehouses();
  }
  onFilterBuilderApply(filters: FilterDto[]): void {
    this.filterRequest.filters = filters;
    if (this.filterRequest.pagination) this.filterRequest.pagination.pageNo = 1;
    this.persistTableState();
    this.loadWarehouses();
    this.showFilterBuilder = false;
  }
  onFilterBuilderClear(): void {
    this.filterRequest.filters = [];
    this.persistTableState();
    this.loadWarehouses();
  }

  viewWarehouse(warehouse: Warehouse): void {
    this.router.navigate(['/warehouses', warehouse.id]);
  }

  editWarehouse(warehouse: Warehouse): void {
    this.router.navigate(['/warehouses', warehouse.id, 'edit']);
  }

  activateWarehouse(warehouse: Warehouse): void {
    this.loading = true;
    this.warehouseApiService.activateWarehouse(warehouse.id).subscribe({
      next: (response) => {
        if (response.success) {
          this.showToastMessage('success', 'Warehouse activated successfully');
          this.loadWarehouses();
        } else {
          this.showToastMessage('error', response.message || 'Failed to activate warehouse');
        }
        this.loading = false;
      },
      error: () => {
        this.showToastMessage('error', 'Error activating warehouse');
        this.loading = false;
      }
    });
  }

  deactivateWarehouse(warehouse: Warehouse): void {
    if (confirm(`Are you sure you want to deactivate "${warehouse.name}"?`)) {
      this.loading = true;
      this.warehouseApiService.deactivateWarehouse(warehouse.id).subscribe({
        next: (response) => {
          if (response.success) {
            this.showToastMessage('success', 'Warehouse deactivated successfully');
            this.loadWarehouses();
          } else {
            this.showToastMessage('error', response.message || 'Failed to deactivate warehouse');
          }
          this.loading = false;
        },
        error: () => {
          this.showToastMessage('error', 'Error deactivating warehouse');
          this.loading = false;
        }
      });
    }
  }

  deleteWarehouse(warehouse: Warehouse): void {
    if (confirm(`Are you sure you want to delete "${warehouse.name}"? This action cannot be undone.`)) {
      this.loading = true;
      this.warehouseApiService.deleteWarehouse(warehouse.id).subscribe({
        next: (response) => {
          if (response.success) {
            this.showToastMessage('success', 'Warehouse deleted successfully');
            this.loadWarehouses();
          } else {
            this.showToastMessage('error', response.message || 'Failed to delete warehouse');
          }
          this.loading = false;
        },
        error: () => {
          this.showToastMessage('error', 'Error deleting warehouse');
          this.loading = false;
        }
      });
    }
  }

  get statusFilterOptions(): FormFieldOption[] {
    return [
      { value: null, label: 'All Status' },
      { value: true, label: 'Active' },
      { value: false, label: 'Inactive' }
    ];
  }


  showToastMessage(type: 'success' | 'error' | 'warning' | 'info', message: string): void {
    this.toastType = type;
    this.toastMessage = message;
    this.showToast = true;
    setTimeout(() => {
      this.showToast = false;
    }, 3000);
  }
}


