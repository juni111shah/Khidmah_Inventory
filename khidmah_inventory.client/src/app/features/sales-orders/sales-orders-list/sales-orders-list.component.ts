import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { SalesOrderApiService } from '../../../core/services/sales-order-api.service';
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

@Component({
  selector: 'app-sales-orders-list',
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
  templateUrl: './sales-orders-list.component.html'
})
export class SalesOrdersListComponent implements OnInit {
  salesOrders: SalesOrder[] = [];
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
      searchFields: ['orderNumber', 'customerName'],
      mode: 'Contains',
      isCaseSensitive: false
    }
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
    loadingMessage: 'Loading sales orders...'
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

  constructor(
    private salesOrderApiService: SalesOrderApiService,
    public router: Router,
    public permissionService: PermissionService,
    private headerService: HeaderService
  ) {}

  ngOnInit(): void {
    this.headerService.setHeaderInfo({
      title: 'Sales Orders',
      description: 'Manage client orders and deliveries'
    });
    this.initializeFilterPanel();
    this.loadSalesOrders();
  }

  private initializeFilterPanel(): void {
    // Already configured above, but this ensures it's called
  }

  onSearch(term: string): void {
    this.searchTerm = term;
    if (this.filterRequest.search) {
      this.filterRequest.search.term = term;
    }
    if (this.filterRequest.pagination) {
      this.filterRequest.pagination.pageNo = 1;
    }
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
  }

  addSalesOrder(): void {
    this.router.navigate(['/sales-orders/new']);
  }

  loadSalesOrders(): void {
    this.loading = true;
    const query: GetSalesOrdersListQuery = {
      filterRequest: this.filterRequest,
      status: this.statusFilter || undefined
    };

    this.salesOrderApiService.getSalesOrders(query).subscribe({
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

  onFilterChange(filterRequest?: any): void {
    if (filterRequest) {
      this.filterRequest = filterRequest;
    }
    this.loadSalesOrders();
  }

  onPageChange(pageNo: number): void {
    if (this.filterRequest.pagination) {
      this.filterRequest.pagination.pageNo = pageNo;
    }
    this.loadSalesOrders();
  }

  onSortChange(sort: any): void {
    if (this.filterRequest.pagination) {
      this.filterRequest.pagination.sortBy = sort.column;
      this.filterRequest.pagination.sortOrder = sort.direction === 'asc' ? 'ascending' : 'descending';
    }
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
}



