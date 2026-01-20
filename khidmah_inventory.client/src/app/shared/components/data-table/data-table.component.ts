import { Component, Input, Output, EventEmitter, OnInit, HostListener } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { FilterFieldComponent } from '../filter-field/filter-field.component';
import { 
  DataTableColumn, 
  DataTableAction, 
  DataTableConfig, 
  DataTableFilter,
  DataTableSort 
} from '../../models/data-table.model';
import { FilterRequest, SearchMode } from '../../../core/models/user.model';
import { IconComponent } from '../icon/icon.component';
import { LoadingSpinnerComponent } from '../loading-spinner/loading-spinner.component';
import { PaginationComponent } from '../pagination/pagination.component';
import { EmptyStateComponent } from '../empty-state/empty-state.component';
import { DropdownComponent } from '../dropdown/dropdown.component';
import { SkeletonTableComponent } from '../skeleton-table/skeleton-table.component';
import { PermissionService } from '../../../core/services/permission.service';
import { UnifiedButtonComponent } from '../unified-button/unified-button.component';
import { UnifiedCheckboxComponent } from '../unified-checkbox/unified-checkbox.component';
import { ListingHeaderComponent } from '../listing-header/listing-header.component';

@Component({
  selector: 'app-data-table',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    IconComponent,
    LoadingSpinnerComponent,
    PaginationComponent,
    EmptyStateComponent,
    DropdownComponent,
    SkeletonTableComponent,
    FilterFieldComponent,
    UnifiedButtonComponent,
    UnifiedCheckboxComponent,
    ListingHeaderComponent
  ],
  templateUrl: './data-table.component.html',
  styleUrls: ['./data-table.component.scss']
})
export class DataTableComponent<T = any> implements OnInit {
  @Input() columns: DataTableColumn<T>[] = [];
  @Input() data: T[] = [];
  @Input() loading: boolean = false;
  @Input() config: DataTableConfig = {};
  @Input() actions: DataTableAction<T>[] = [];
  @Input() pagedResult: any = null; // PagedResult<T>
  @Input() filterRequest: FilterRequest = {};
  
  @Output() filterChange = new EventEmitter<FilterRequest>();
  @Output() pageChange = new EventEmitter<number>();
  @Output() sortChange = new EventEmitter<DataTableSort>();
  @Output() rowClick = new EventEmitter<T>();
  @Output() rowSelect = new EventEmitter<T[]>();
  @Output() exportData = new EventEmitter<{ format: string; options: any }>();

  searchTerm: string = '';
  showColumnToggle: boolean = false;
  showFilters: boolean = false;
  selectedRows = new Set<T>();
  currentSort: DataTableSort | null = null;
  columnFilters: Map<string, any> = new Map();
  tempColumnFilters: Map<string, any> = new Map();
  currentActionDropdownRow: T | null = null;
  currentActionDropdownActions: DataTableAction<T>[] = [];

  @HostListener('document:click', ['$event'])
  onDocumentClick(event: MouseEvent): void {
    const target = event.target as HTMLElement;
    if (!target.closest('.column-toggle-dropdown') && this.showColumnToggle) {
      this.showColumnToggle = false;
    }
    // Close action dropdown when clicking outside
    if (!target.closest('.action-dropdown-menu') && !target.closest('.dropdown-toggle')) {
      this.closeActionDropdown();
    }
  }

  defaultConfig: DataTableConfig = {
    showSearch: true,
    showFilters: true,
    showColumnToggle: true,
    showPagination: true,
    showActions: true,
    showCheckbox: false,
    pageSize: 10,
    pageSizeOptions: [5, 10, 25, 50, 100],
    searchPlaceholder: 'Search...',
    emptyMessage: 'No data available',
    loadingMessage: 'Loading...'
  };

  get effectiveConfig(): DataTableConfig {
    return { ...this.defaultConfig, ...this.config };
  }

  get visibleColumns(): DataTableColumn<T>[] {
    return this.columns.filter(col => col.visible !== false);
  }

  get filterableColumns(): DataTableColumn<T>[] {
    return this.columns.filter(col => col.filterable !== false);
  }

  ngOnInit(): void {
    this.columns.forEach(col => {
      if (col.visible === undefined) {
        col.visible = true;
      }
    });

    if (!this.filterRequest.pagination) {
      this.filterRequest.pagination = {
        pageNo: 1,
        pageSize: this.effectiveConfig.pageSize || 10
      };
    }

    if (!this.filterRequest.search) {
      this.filterRequest.search = {
        term: '',
        searchFields: this.columns.filter(col => col.searchable !== false).map(col => col.key),
        mode: SearchMode.Contains,
        isCaseSensitive: false
      };
    }

    if (!this.filterRequest.filters) {
      this.filterRequest.filters = [];
    }
  }

  onSearch(): void {
    if (this.filterRequest.search) {
      this.filterRequest.search.term = this.searchTerm;
      if (this.filterRequest.pagination) {
        this.filterRequest.pagination.pageNo = 1;
      }
      this.filterChange.emit(this.filterRequest);
    }
  }

  getFilterValue(key: string): any {
    return this.tempColumnFilters.get(key) || '';
  }

  onColumnFilter(column: DataTableColumn<T>, value: any): void {
    this.tempColumnFilters.set(column.key, value);
  }

  applyFilters(): void {
    this.columnFilters.clear();
    this.tempColumnFilters.forEach((value, key) => {
      this.columnFilters.set(key, value);
    });

    if (!this.filterRequest.filters) {
      this.filterRequest.filters = [];
    }

    this.filterRequest.filters = [];

    this.tempColumnFilters.forEach((value, key) => {
      if (value !== null && value !== undefined && value !== '') {
        if (this.filterRequest.filters) {
          this.filterRequest.filters.push({
            column: key,
            operator: '=',
            value: value
          });
        }
      }
    });

    if (this.filterRequest.pagination) {
      this.filterRequest.pagination.pageNo = 1;
    }

    this.filterChange.emit(this.filterRequest);
    this.showFilters = false;
  }

  clearFilters(): void {
    this.searchTerm = '';
    this.columnFilters.clear();
    this.tempColumnFilters.clear();
    this.filterRequest.filters = [];
    if (this.filterRequest.search) {
      this.filterRequest.search.term = '';
    }
    if (this.filterRequest.pagination) {
      this.filterRequest.pagination.pageNo = 1;
    }
    this.filterChange.emit(this.filterRequest);
    this.showFilters = false;
  }

  openFilterDrawer(): void {
    this.tempColumnFilters.clear();
    this.columnFilters.forEach((value, key) => {
      this.tempColumnFilters.set(key, value);
    });
    this.showFilters = true;
  }

  onSort(column: DataTableColumn<T>): void {
    if (!column.sortable) return;

    if (this.filterRequest.pagination) {
      if (this.currentSort?.column === column.key) {
        this.currentSort.direction = this.currentSort.direction === 'asc' ? 'desc' : 'asc';
      } else {
        this.currentSort = {
          column: column.key,
          direction: 'asc'
        };
      }

      this.filterRequest.pagination.sortBy = this.currentSort.column;
      this.filterRequest.pagination.sortOrder = this.currentSort.direction === 'asc' ? 'ascending' : 'descending';
      
      this.sortChange.emit(this.currentSort);
      this.filterChange.emit(this.filterRequest);
    }
  }

  onPageChange(pageNo: number): void {
    if (this.filterRequest.pagination) {
      this.filterRequest.pagination.pageNo = pageNo;
      this.filterChange.emit(this.filterRequest);
      this.pageChange.emit(pageNo);
    }
  }

  onPageSizeChange(size: number): void {
    if (this.filterRequest.pagination) {
      this.filterRequest.pagination.pageSize = size;
      this.filterRequest.pagination.pageNo = 1;
      this.filterChange.emit(this.filterRequest);
    }
  }

  toggleColumnVisibility(column: DataTableColumn<T>): void {
    column.visible = !column.visible;
  }

  toggleAllSelection(): void {
    if (this.isAllSelected()) {
      this.selectedRows.clear();
    } else {
      this.data.forEach(row => this.selectedRows.add(row));
    }
    this.emitSelection();
  }

  toggleRowSelection(row: T): void {
    if (this.selectedRows.has(row)) {
      this.selectedRows.delete(row);
    } else {
      this.selectedRows.add(row);
    }
    this.emitSelection();
  }

  isAllSelected(): boolean {
    return this.selectedRows.size === this.data.length && this.data.length > 0;
  }

  isRowSelected(row: T): boolean {
    return this.selectedRows.has(row);
  }

  private emitSelection(): void {
    this.rowSelect.emit(Array.from(this.selectedRows));
  }

  getCellValue(row: T, column: DataTableColumn<T>): string {
    if (column.render) {
      return column.render(row, column);
    }

    const value = (row as any)[column.key];
    
    if (column.format) {
      return column.format(value);
    }

    if (value === null || value === undefined) {
      return '-';
    }

    if (column.type === 'date' && value) {
      return new Date(value).toLocaleDateString();
    }

    if (column.type === 'boolean') {
      return value ? 'Yes' : 'No';
    }

    return String(value);
  }

  getCellBooleanValue(row: T, column: DataTableColumn<T>): boolean {
    const value = (row as any)[column.key];
    return value === true || value === 'true' || value === 1;
  }

  getSortIcon(column: DataTableColumn<T>): string {
    if (!column.sortable) return '';
    if (!this.currentSort || this.currentSort.column !== column.key) return 'bi-arrow-down-up text-muted ms-1';
    return this.currentSort.direction === 'asc' ? 'bi-arrow-up text-primary ms-1' : 'bi-arrow-down text-primary ms-1';
  }

  canShowAction(action: DataTableAction<T>, row: T): boolean {
    return !action.condition || action.condition(row);
  }

  getVisibleActions(row: T): DataTableAction<T>[] {
    return this.actions.filter(action => this.canShowAction(action, row));
  }

  onActionClick(action: DataTableAction<T>, row: T, event: Event): void {
    event.stopPropagation();
    if (this.canShowAction(action, row)) {
      action.action(row);
      this.closeActionDropdown(); // Close dropdown after action
    }
  }

  toggleActionDropdown(row: T, event?: Event): void {
    if (event) {
      event.stopPropagation();
    }

    if (this.currentActionDropdownRow === row) {
      // Close if already open
      this.closeActionDropdown();
    } else {
      // Open for this row
      this.currentActionDropdownRow = row;
      this.currentActionDropdownActions = this.getVisibleActions(row);

      // Store the button position for dropdown positioning
      if (event && event.target) {
        const button = (event.target as HTMLElement).closest('button');
        if (button) {
          const rect = button.getBoundingClientRect();
          this.dropdownPosition = {
            top: rect.bottom + window.scrollY,
            left: rect.right + window.scrollX - 200 // Align to right edge of button
          };
        }
      }
    }
  }

  dropdownPosition: { top: number; left: number } | null = null;

  closeActionDropdown(): void {
    this.currentActionDropdownRow = null;
    this.currentActionDropdownActions = [];
  }

  hasOpenActionDropdown(): boolean {
    return this.currentActionDropdownRow !== null;
  }

  getCurrentDropdownRow(): T {
    return this.currentActionDropdownRow!;
  }

  getCurrentDropdownActions(): DataTableAction<T>[] {
    return this.currentActionDropdownActions;
  }

  getActionIcon(action: DataTableAction<T>): string {
    if (action.icon) {
      // If it's already a bootstrap icon name like 'bi-eye' or just 'eye'
      return action.icon.startsWith('bi-') ? action.icon.substring(3) : action.icon;
    }
    const label = action.label.toLowerCase();
    if (label.includes('view') || label.includes('detail')) return 'eye';
    if (label.includes('edit') || label.includes('update')) return 'pencil';
    if (label.includes('delete') || label.includes('remove')) return 'trash';
    if (label.includes('activate') || label.includes('enable')) return 'check-circle';
    if (label.includes('deactivate') || label.includes('disable')) return 'slash-circle';
    if (label.includes('download')) return 'download';
    if (label.includes('print')) return 'printer';
    if (label.includes('copy')) return 'content-copy';
    if (label.includes('archive')) return 'archive';
    return 'three-dots-vertical';
  }

  onRowClick(row: T): void {
    this.rowClick.emit(row);
  }

  trackByFn(index: number, item: T): any {
    return (item as any).id || index;
  }

  get Math() {
    return Math;
  }

  getSkeletonHeaders(): Array<{ width?: string }> {
    const headers: Array<{ width?: string }> = [];
    if (this.effectiveConfig.showCheckbox) headers.push({ width: '50px' });
    this.visibleColumns.forEach(column => headers.push({ width: column.width || 'auto' }));
    if (this.effectiveConfig.showActions && this.actions.length > 0) headers.push({ width: '120px' });
    return headers;
  }

  getDefaultPageSize(): number {
    const options = this.effectiveConfig.pageSizeOptions;
    return options && options.length > 0 ? options[0] : 10;
  }

  get searchPlaceholderText(): string {
    return this.effectiveConfig.searchPlaceholder || 'Search...';
  }

  constructor(public permissionService: PermissionService) {}
}
