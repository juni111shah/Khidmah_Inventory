import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { PurchaseOrderApiService } from '../../../core/services/purchase-order-api.service';
import { PurchaseOrder, GetPurchaseOrdersListQuery } from '../../../core/models/purchase-order.model';
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

@Component({
  selector: 'app-purchase-orders-list',
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
  templateUrl: './purchase-orders-list.component.html'
})
export class PurchaseOrdersListComponent implements OnInit {
  purchaseOrders: PurchaseOrder[] = [];
  loading = false;
  searchTerm = '';
  statusFilter: string | null = null;
  pagedResult: any = null;
  filterRequest: any = {
    pagination: {
      pageNo: 1,
      pageSize: 10,
      sortBy: 'orderDate',
      sortOrder: 'descending'
    },
    search: {
      term: '',
      searchFields: ['orderNumber', 'supplierName'],
      mode: 'Contains',
      isCaseSensitive: false
    }
  };

  columns: DataTableColumn<PurchaseOrder>[] = [
    { key: 'orderNumber', label: 'Order #', sortable: true, searchable: true, filterable: true, width: '150px' },
    { key: 'supplierName', label: 'Supplier', sortable: true, searchable: true, filterable: true, width: '200px' },
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
        { label: 'Sent', value: 'Sent' },
        { label: 'Partially Received', value: 'PartiallyReceived' },
        { label: 'Completed', value: 'Completed' }
      ],
      type: 'badge', 
      width: '120px' 
    },
    { key: 'totalAmount', label: 'Total', sortable: true, filterable: false, type: 'number', width: '120px', format: (value) => `$${value.toFixed(2)}` }
  ];

  actions: DataTableAction<PurchaseOrder>[] = [
    { label: 'View', icon: 'eye', action: (row) => this.viewOrder(row), condition: () => this.permissionService.hasPermission('PurchaseOrders:Read') }
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
    searchPlaceholder: 'Search purchase orders...',
    emptyMessage: 'No purchase orders found',
    loadingMessage: 'Loading purchase orders...'
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
    private purchaseOrderApiService: PurchaseOrderApiService,
    public router: Router,
    public permissionService: PermissionService,
    private headerService: HeaderService
  ) {}

  ngOnInit(): void {
    this.headerService.setHeaderInfo({
      title: 'Purchase Orders',
      description: 'Manage inventory supply orders'
    });
    this.initializeFilterPanel();
    this.loadPurchaseOrders();
  }

  private initializeFilterPanel(): void {
    this.filterPanelConfig = {
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
            { label: 'Sent', value: 'Sent' },
            { label: 'Partially Received', value: 'PartiallyReceived' },
            { label: 'Completed', value: 'Completed' }
          ]
        }
      ],
      showResetButton: true,
      showApplyButton: true,
      layout: 'row'
    };

    // Initialize filter values
    this.filterPanelValues = {
      status: null
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
    this.loadPurchaseOrders();
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
    this.loadPurchaseOrders();
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
    this.loadPurchaseOrders();
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

  onColumnToggle(column: DataTableColumn<PurchaseOrder>): void {
    column.visible = column.visible === false ? true : false;
  }

  addPurchaseOrder(): void {
    this.router.navigate(['/purchase-orders/new']);
  }

  loadPurchaseOrders(): void {
    this.loading = true;
    const query: GetPurchaseOrdersListQuery = {
      filterRequest: this.filterRequest,
      status: this.statusFilter || undefined
    };

    this.purchaseOrderApiService.getPurchaseOrders(query).subscribe({
      next: (response) => {
        if (response.success && response.data) {
          this.pagedResult = response.data;
          this.purchaseOrders = response.data.items;
        } else {
          this.showToastMessage('error', response.message || 'Failed to load purchase orders');
        }
        this.loading = false;
      },
      error: () => {
        this.showToastMessage('error', 'Error loading purchase orders');
        this.loading = false;
      }
    });
  }

  onFilterChange(filterRequest?: any): void {
    if (filterRequest) {
      this.filterRequest = filterRequest;
    }
    this.loadPurchaseOrders();
  }

  onPageChange(pageNo: number): void {
    if (this.filterRequest.pagination) {
      this.filterRequest.pagination.pageNo = pageNo;
    }
    this.loadPurchaseOrders();
  }

  onSortChange(sort: any): void {
    if (this.filterRequest.pagination) {
      this.filterRequest.pagination.sortBy = sort.column;
      this.filterRequest.pagination.sortOrder = sort.direction === 'asc' ? 'ascending' : 'descending';
    }
    this.loadPurchaseOrders();
  }

  viewOrder(order: PurchaseOrder): void {
    this.router.navigate(['/purchase-orders', order.id]);
  }


  showToastMessage(type: 'success' | 'error' | 'warning' | 'info', message: string): void {
    this.toastType = type;
    this.toastMessage = message;
    this.showToast = true;
    setTimeout(() => { this.showToast = false; }, 3000);
  }
}



