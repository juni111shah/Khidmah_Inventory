import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { ReorderApiService } from '../../../core/services/reorder-api.service';
import { WarehouseApiService } from '../../../core/services/warehouse-api.service';
import { ReorderSuggestion } from '../../../core/models/reorder.model';
import { ApiResponse } from '../../../core/models/api-response.model';
import { ToastComponent } from '../../../shared/components/toast/toast.component';
import { LoadingSpinnerComponent } from '../../../shared/components/loading-spinner/loading-spinner.component';
import { UnifiedCardComponent } from '../../../shared/components/unified-card/unified-card.component';
import { IconComponent } from '../../../shared/components/icon/icon.component';
import { Warehouse } from '../../../core/models/warehouse.model';
import { ContentLoaderComponent } from '../../../shared/components/content-loader/content-loader.component';
import { SkeletonFormComponent } from '../../../shared/components/skeleton-form/skeleton-form.component';
import { SkeletonTableComponent } from '../../../shared/components/skeleton-table/skeleton-table.component';
import { DataTableComponent } from '../../../shared/components/data-table/data-table.component';
import { DataTableColumn, DataTableAction, DataTableConfig } from '../../../shared/models/data-table.model';
import { FilterRequest } from '../../../core/models/user.model';
import { SearchMode } from '../../../core/models/user.model';
import { applyClientSideTable } from '../../../shared/utils/client-side-table.util';

@Component({
  selector: 'app-reorder-list',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    ToastComponent,
    LoadingSpinnerComponent,
    UnifiedCardComponent,
    IconComponent,
    ContentLoaderComponent,
    SkeletonFormComponent,
    SkeletonTableComponent,
    DataTableComponent
  ],
  templateUrl: './reorder-list.component.html'
})
export class ReorderListComponent implements OnInit {
  loading = true;
  suggestions: ReorderSuggestion[] = [];
  pagedResult: any = null;
  selectedSuggestions: ReorderSuggestion[] = [];
  warehouses: Warehouse[] = [];
  selectedWarehouseId: string | null = null;
  selectedPriority: string | null = null;
  showToast = false;
  toastMessage = '';
  toastType: 'success' | 'error' | 'info' = 'success';

  filterRequest: FilterRequest = {
    pagination: { pageNo: 1, pageSize: 10, sortBy: 'priority', sortOrder: 'descending' },
    search: { term: '', searchFields: ['productName', 'productSKU', 'warehouseName', 'priority'], mode: SearchMode.Contains, isCaseSensitive: false },
    filters: []
  };

  columns: DataTableColumn<ReorderSuggestion>[] = [
    { key: 'productName', label: 'Product', sortable: true, searchable: true, width: '200px', render: (s) => `${s.productName} (${s.productSKU})` },
    { key: 'warehouseName', label: 'Warehouse', sortable: true, searchable: true, width: '120px' },
    { key: 'currentStock', label: 'Current', sortable: true, width: '80px', type: 'number' },
    { key: 'minStockLevel', label: 'Min / Reorder', sortable: true, width: '120px', render: (s) => `${s.minStockLevel ?? '-'} / ${s.reorderPoint ?? '-'}` },
    { key: 'suggestedQuantity', label: 'Suggested', sortable: true, width: '90px', type: 'number' },
    { key: 'daysOfStockRemaining', label: 'Days left', sortable: true, width: '80px', type: 'number' },
    { key: 'priority', label: 'Priority', sortable: true, searchable: true, type: 'badge', width: '90px', filterable: true, filterType: 'select', filterOptions: [{ label: 'All', value: '' }, { label: 'Critical', value: 'Critical' }, { label: 'High', value: 'High' }, { label: 'Medium', value: 'Medium' }, { label: 'Low', value: 'Low' }] },
    { key: 'supplierSuggestions', label: 'Supplier', sortable: true, searchable: true, width: '120px', render: (s) => s.supplierSuggestions?.length ? s.supplierSuggestions[0].supplierName : '-' }
  ];

  actions: DataTableAction<ReorderSuggestion>[] = [
    { label: 'Review', icon: 'eye', action: (row) => this.router.navigate(['/reorder/review'], { queryParams: { productId: row.productId, warehouseId: row.warehouseId } }) }
  ];

  config: DataTableConfig = {
    showSearch: true,
    showFilters: true,
    showColumnToggle: false,
    showPagination: true,
    showActions: true,
    showCheckbox: true,
    pageSize: 10,
    pageSizeOptions: [5, 10, 25, 50, 100],
    searchPlaceholder: 'Search suggestions...',
    emptyMessage: 'No reorder suggestions.'
  };

  constructor(
    private reorderApi: ReorderApiService,
    private warehouseApi: WarehouseApiService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadWarehouses();
    this.loadSuggestions();
  }

  loadWarehouses(): void {
    this.warehouseApi.getWarehouses({ filterRequest: { pagination: { pageNo: 1, pageSize: 100 } } }).subscribe({
      next: (res: ApiResponse<any>) => {
        if (res.success && res.data?.items) this.warehouses = res.data.items;
      },
      error: () => {}
    });
  }

  loadSuggestions(): void {
    this.loading = true;
    const query: any = { includeInStock: false };
    if (this.selectedWarehouseId) query.warehouseId = this.selectedWarehouseId;
    if (this.selectedPriority) query.priority = this.selectedPriority;
    this.reorderApi.getSuggestions(query).subscribe({
      next: (res: ApiResponse<ReorderSuggestion[]>) => {
        this.loading = false;
        if (res.success && res.data) this.suggestions = res.data;
        else this.suggestions = [];
        this.selectedSuggestions = [];
        this.applyTable();
      },
      error: () => {
        this.loading = false;
        this.suggestions = [];
        this.applyTable();
      }
    });
  }

  applyFilters(): void {
    this.loadSuggestions();
  }

  private getCellValue(row: ReorderSuggestion, key: string): any {
    if (key === 'supplierSuggestions') return row.supplierSuggestions?.length ? row.supplierSuggestions[0].supplierName : '';
    return (row as any)[key];
  }

  private applyTable(): void {
    const { pagedResult } = applyClientSideTable(this.suggestions, this.filterRequest, ['productName', 'productSKU', 'warehouseName', 'priority', 'supplierSuggestions'] as (keyof ReorderSuggestion)[], (r, k) => this.getCellValue(r, k));
    this.pagedResult = pagedResult;
  }

  onFilterChange(req?: FilterRequest): void {
    if (req) this.filterRequest = req;
    this.applyTable();
  }

  onPageChange(pageNo: number): void {
    if (this.filterRequest.pagination) this.filterRequest.pagination.pageNo = pageNo;
    this.applyTable();
  }

  onSortChange(sort: { column: string; direction: 'asc' | 'desc' }): void {
    if (this.filterRequest.pagination) {
      this.filterRequest.pagination.sortBy = sort.column;
      this.filterRequest.pagination.sortOrder = sort.direction === 'asc' ? 'ascending' : 'descending';
    }
    this.applyTable();
  }

  itemKey(s: ReorderSuggestion): string {
    return `${s.productId}|${s.warehouseId}`;
  }

  getSelectedIdsParam(): string {
    return this.selectedSuggestions.map(s => this.itemKey(s)).join(',');
  }

  getPriorityClass(priority: string): string {
    switch (priority) {
      case 'Critical': return 'danger';
      case 'High': return 'warning';
      case 'Medium': return 'info';
      default: return 'secondary';
    }
  }
}
