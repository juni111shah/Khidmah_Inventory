import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { InventoryApiService } from '../../../core/services/inventory-api.service';
import { ProductApiService } from '../../../core/services/product-api.service';
import { WarehouseApiService } from '../../../core/services/warehouse-api.service';
import { StockLevel, GetStockLevelsListQuery } from '../../../core/models/inventory.model';
import { DataTableComponent } from '../../../shared/components/data-table/data-table.component';
import { DataTableColumn, DataTableConfig } from '../../../shared/models/data-table.model';
import { ToastComponent } from '../../../shared/components/toast/toast.component';
import { IconComponent } from '../../../shared/components/icon/icon.component';
import { HasPermissionDirective } from '../../../shared/directives/has-permission.directive';
import { PermissionService } from '../../../core/services/permission.service';
import { ListingContainerComponent } from '../../../shared/components/listing-container/listing-container.component';
import { UnifiedButtonComponent } from '../../../shared/components/unified-button/unified-button.component';
import { FilterFieldComponent } from '../../../shared/components/filter-field/filter-field.component';
import { FilterPanelComponent, FilterPanelConfig, FilterPanelField } from '../../../shared/components/filter-panel/filter-panel.component';
import { ExportComponent } from '../../../shared/components/export/export.component';
import { HeaderService } from '../../../core/services/header.service';

@Component({
  selector: 'app-stock-levels-list',
  standalone: true,
  imports: [
    CommonModule, 
    FormsModule, 
    ReactiveFormsModule,
    DataTableComponent, 
    ToastComponent, 
    HasPermissionDirective, 
    IconComponent,
    ListingContainerComponent,
    UnifiedButtonComponent,
    FilterFieldComponent,
    FilterPanelComponent,
    ExportComponent
  ],
  templateUrl: './stock-levels-list.component.html'
})
export class StockLevelsListComponent implements OnInit {
  stockLevels: StockLevel[] = [];
  loading = false;
  searchTerm = '';
  selectedProductId: string | null = null;
  selectedWarehouseId: string | null = null;
  lowStockOnly = false;
  pagedResult: any = null;
  filterRequest: any = {
    pagination: {
      pageNo: 1,
      pageSize: 10,
      sortBy: 'productName',
      sortOrder: 'ascending'
    },
    search: {
      term: '',
      searchFields: ['productName', 'productSKU', 'warehouseName'],
      mode: 'Contains',
      isCaseSensitive: false
    }
  };

  columns: DataTableColumn<StockLevel>[] = [
    { key: 'productName', label: 'Product', sortable: true, searchable: true, filterable: true, width: '200px' },
    { key: 'productSKU', label: 'SKU', sortable: true, searchable: true, filterable: true, width: '120px' },
    { key: 'warehouseName', label: 'Warehouse', sortable: true, searchable: true, filterable: true, width: '150px' },
    { key: 'quantity', label: 'Quantity', sortable: true, filterable: false, type: 'number', width: '100px' },
    { key: 'availableQuantity', label: 'Available', sortable: true, filterable: false, type: 'number', width: '100px' },
    { key: 'reservedQuantity', label: 'Reserved', sortable: true, filterable: false, type: 'number', width: '100px' },
    { key: 'averageCost', label: 'Avg Cost', sortable: true, filterable: false, type: 'number', width: '120px', format: (value) => value ? `$${value.toFixed(2)}` : '-' }
  ];

  config: DataTableConfig = {
    showSearch: false,
    showFilters: false,
    showColumnToggle: false,
    showPagination: true,
    showActions: false,
    showCheckbox: false,
    pageSize: 10,
    pageSizeOptions: [5, 10, 25, 50, 100],
    searchPlaceholder: 'Search stock levels...',
    emptyMessage: 'No stock levels found',
    loadingMessage: 'Loading stock levels...'
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

  constructor(
    private inventoryApiService: InventoryApiService,
    public router: Router,
    public permissionService: PermissionService,
    private headerService: HeaderService
  ) {}

  ngOnInit(): void {
    this.headerService.setHeaderInfo({
      title: 'Stock Levels',
      description: 'Monitor product quantities across warehouses'
    });
    this.initializeFilterPanel();
    this.loadStockLevels();
  }

  private initializeFilterPanel(): void {
    this.filterPanelConfig = {
      fields: [
        {
          key: 'lowStockOnly',
          label: 'Low Stock Only',
          type: 'boolean',
          placeholder: 'Show only low stock items',
          colSize: 'col-md-6',
          defaultValue: false
        }
      ],
      showResetButton: true,
      showApplyButton: true,
      layout: 'row'
    };

    // Initialize filter values
    this.filterPanelValues = {
      lowStockOnly: false
    };
  }

  onSearch(term: string): void {
    this.searchTerm = term;
    if (this.filterRequest.search) {
      this.filterRequest.search.term = term;
    }
    if (this.filterRequest.pagination) {
      this.filterRequest.pagination.pageNo = 1;
    }
    this.loadStockLevels();
  }

  onFilterToggle(): void {
    this.showFilterPanel = !this.showFilterPanel;
  }

  // Filter panel event handlers
  onFilterApplied(filters: { [key: string]: any }): void {
    this.lowStockOnly = filters['lowStockOnly'] || false;

    if (this.filterRequest.pagination) {
      this.filterRequest.pagination.pageNo = 1;
    }
    this.loadStockLevels();
  }

  onFilterReset(): void {
    this.lowStockOnly = false;
    this.selectedProductId = null;
    this.selectedWarehouseId = null;
    this.searchTerm = '';
    if (this.filterRequest.search) {
      this.filterRequest.search.term = '';
    }
    if (this.filterRequest.pagination) {
      this.filterRequest.pagination.pageNo = 1;
    }
    this.loadStockLevels();
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

  onColumnToggle(column: DataTableColumn<StockLevel>): void {
    column.visible = column.visible === false ? true : false;
  }

  loadStockLevels(): void {
    this.loading = true;
    const query: GetStockLevelsListQuery = {
      filterRequest: this.filterRequest,
      productId: this.selectedProductId || undefined,
      warehouseId: this.selectedWarehouseId || undefined,
      lowStockOnly: this.lowStockOnly
    };

    this.inventoryApiService.getStockLevels(query).subscribe({
      next: (response) => {
        if (response.success && response.data) {
          this.pagedResult = response.data;
          this.stockLevels = response.data.items;
        } else {
          this.showToastMessage('error', response.message || 'Failed to load stock levels');
        }
        this.loading = false;
      },
      error: () => {
        this.showToastMessage('error', 'Error loading stock levels');
        this.loading = false;
      }
    });
  }

  onFilterChange(filterRequest?: any): void {
    if (filterRequest) {
      this.filterRequest = filterRequest;
    }
    this.loadStockLevels();
  }

  onPageChange(pageNo: number): void {
    if (this.filterRequest.pagination) {
      this.filterRequest.pagination.pageNo = pageNo;
    }
    this.loadStockLevels();
  }

  onSortChange(sort: any): void {
    if (this.filterRequest.pagination) {
      this.filterRequest.pagination.sortBy = sort.column;
      this.filterRequest.pagination.sortOrder = sort.direction === 'asc' ? 'ascending' : 'descending';
    }
    this.loadStockLevels();
  }


  showToastMessage(type: 'success' | 'error' | 'warning' | 'info', message: string): void {
    this.toastType = type;
    this.toastMessage = message;
    this.showToast = true;
    setTimeout(() => { this.showToast = false; }, 3000);
  }
}



