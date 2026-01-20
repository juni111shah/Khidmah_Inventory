import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { CustomerApiService } from '../../../core/services/customer-api.service';
import { Customer, GetCustomersListQuery } from '../../../core/models/customer.model';
import { DataTableComponent } from '../../../shared/components/data-table/data-table.component';
import { DataTableColumn, DataTableAction, DataTableConfig } from '../../../shared/models/data-table.model';
import { ToastComponent } from '../../../shared/components/toast/toast.component';
import { HasPermissionDirective } from '../../../shared/directives/has-permission.directive';
import { IconComponent } from '../../../shared/components/icon/icon.component';
import { PermissionService } from '../../../core/services/permission.service';
import { ListingContainerComponent } from '../../../shared/components/listing-container/listing-container.component';
import { UnifiedButtonComponent } from '../../../shared/components/unified-button/unified-button.component';
import { FilterFieldComponent } from '../../../shared/components/filter-field/filter-field.component';
import { FilterPanelComponent, FilterPanelConfig, FilterPanelField } from '../../../shared/components/filter-panel/filter-panel.component';
import { ExportComponent } from '../../../shared/components/export/export.component';
import { HeaderService } from '../../../core/services/header.service';

@Component({
  selector: 'app-customers-list',
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
  templateUrl: './customers-list.component.html'
})
export class CustomersListComponent implements OnInit {
  customers: Customer[] = [];
  loading = false;
  searchTerm = '';
  isActiveFilter: any = '';
  pagedResult: any = null;
  filterRequest: any = {
    pagination: {
      pageNo: 1,
      pageSize: 10,
      sortBy: 'name',
      sortOrder: 'ascending'
    },
    search: {
      term: '',
      searchFields: ['name', 'code', 'email', 'phoneNumber'],
      mode: 'Contains',
      isCaseSensitive: false
    }
  };

  columns: DataTableColumn<Customer>[] = [
    { key: 'name', label: 'Name', sortable: true, searchable: true, filterable: true, width: '200px' },
    { key: 'code', label: 'Code', sortable: true, searchable: true, filterable: true, width: '120px' },
    { key: 'email', label: 'Email', sortable: true, searchable: true, filterable: true, width: '180px' },
    { key: 'phoneNumber', label: 'Phone', sortable: true, searchable: true, filterable: true, width: '120px' },
    { key: 'city', label: 'City', sortable: true, filterable: true, width: '120px' },
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
    }
  ];

  actions: DataTableAction<Customer>[] = [
    { label: 'View', icon: 'eye', action: (row) => this.viewCustomer(row), condition: () => this.permissionService.hasPermission('Customers:Read') },
    { label: 'Edit', icon: 'pencil', action: (row) => this.editCustomer(row), condition: () => this.permissionService.hasPermission('Customers:Update') }
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
    searchPlaceholder: 'Search customers...',
    emptyMessage: 'No customers found',
    loadingMessage: 'Loading customers...'
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
    resetButtonText: 'Reset',
    applyButtonText: 'Apply Filters',
    layout: 'row'
  };

  filterPanelValues: { [key: string]: any } = {};

  filterFields: FilterPanelField[] = [
    {
      key: 'name',
      label: 'Name',
      type: 'text',
      placeholder: 'Filter by name',
      colSize: 'col-md-3'
    },
    {
      key: 'code',
      label: 'Code',
      type: 'text',
      placeholder: 'Filter by code',
      colSize: 'col-md-3'
    },
    {
      key: 'email',
      label: 'Email',
      type: 'text',
      placeholder: 'Filter by email',
      colSize: 'col-md-3'
    },
    {
      key: 'phoneNumber',
      label: 'Phone',
      type: 'text',
      placeholder: 'Filter by phone',
      colSize: 'col-md-3'
    },
    {
      key: 'city',
      label: 'City',
      type: 'text',
      placeholder: 'Filter by city',
      colSize: 'col-md-3'
    },
    {
      key: 'isActive',
      label: 'Status',
      type: 'select',
      placeholder: 'All Status',
      options: [
        { label: 'All', value: '' },
        { label: 'Active', value: true },
        { label: 'Inactive', value: false }
      ],
      colSize: 'col-md-3'
    }
  ];

  constructor(
    private customerApiService: CustomerApiService,
    public router: Router,
    public permissionService: PermissionService,
    private headerService: HeaderService
  ) {}

  ngOnInit(): void {
    this.headerService.setHeaderInfo({
      title: 'Customers',
      description: 'Manage client information and order history'
    });
    this.initializeFilterPanel();
    this.loadCustomers();
  }

  onFilterToggle(): void {
    this.showFilterPanel = !this.showFilterPanel;
  }

  private initializeFilterPanel(): void {
    this.filterPanelConfig = {
      fields: this.filterFields,
      showResetButton: true,
      showApplyButton: true,
      resetButtonText: 'Reset',
      applyButtonText: 'Apply Filters',
      layout: 'row'
    };

    // Initialize filter values
    this.filterPanelValues = {
      name: '',
      code: '',
      email: '',
      phoneNumber: '',
      city: '',
      isActive: ''
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
    this.loadCustomers();
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

    if (filters['email'] && filters['email'].trim()) {
      this.filterRequest.filters.push({
        column: 'Email',
        operator: 'Contains',
        value: filters['email'].trim()
      });
    }

    if (filters['phoneNumber'] && filters['phoneNumber'].trim()) {
      this.filterRequest.filters.push({
        column: 'PhoneNumber',
        operator: 'Contains',
        value: filters['phoneNumber'].trim()
      });
    }

    if (filters['city'] && filters['city'].trim()) {
      this.filterRequest.filters.push({
        column: 'City',
        operator: 'Contains',
        value: filters['city'].trim()
      });
    }

    // Handle isActive filter separately for API compatibility
    this.isActiveFilter = filters['isActive'] !== undefined ? filters['isActive'] : '';

    if (this.filterRequest.pagination) {
      this.filterRequest.pagination.pageNo = 1;
    }

    this.loadCustomers();
  }

  onFilterReset(): void {
    this.isActiveFilter = '';
    if (this.filterRequest.search) {
      this.filterRequest.search.term = '';
    }
    if (this.filterRequest.pagination) {
      this.filterRequest.pagination.pageNo = 1;
    }
    this.loadCustomers();
  }

  onFilterClose(): void {
    this.showFilterPanel = false;
  }


  onColumnsToggle(): void {
    this.showColumnToggle = !this.showColumnToggle;
  }

  onColumnToggle(column: DataTableColumn<Customer>): void {
    column.visible = column.visible === false ? true : false;
  }

  addCustomer(): void {
    this.router.navigate(['/customers/new']);
  }

  loadCustomers(): void {
    this.loading = true;
    const query: GetCustomersListQuery = {
      filterRequest: this.filterRequest,
      isActive: (this.isActiveFilter === '' || this.isActiveFilter === null) ? undefined : (this.isActiveFilter === 'true' || this.isActiveFilter === true)
    };

    this.customerApiService.getCustomers(query).subscribe({
      next: (response) => {
        if (response.success && response.data) {
          this.pagedResult = response.data;
          this.customers = response.data.items;
        } else {
          this.showToastMessage('error', response.message || 'Failed to load customers');
        }
        this.loading = false;
      },
      error: () => {
        this.showToastMessage('error', 'Error loading customers');
        this.loading = false;
      }
    });
  }

  onFilterChange(filterRequest?: any): void {
    if (filterRequest) {
      this.filterRequest = filterRequest;
    }
    this.loadCustomers();
  }

  onPageChange(pageNo: number): void {
    if (this.filterRequest.pagination) {
      this.filterRequest.pagination.pageNo = pageNo;
    }
    this.loadCustomers();
  }

  onSortChange(sort: any): void {
    if (this.filterRequest.pagination) {
      this.filterRequest.pagination.sortBy = sort.column;
      this.filterRequest.pagination.sortOrder = sort.direction === 'asc' ? 'ascending' : 'descending';
    }
    this.loadCustomers();
  }

  viewCustomer(customer: Customer): void {
    this.router.navigate(['/customers', customer.id]);
  }

  editCustomer(customer: Customer): void {
    this.router.navigate(['/customers', customer.id, 'edit']);
  }


  showToastMessage(type: 'success' | 'error' | 'warning' | 'info', message: string): void {
    this.toastType = type;
    this.toastMessage = message;
    this.showToast = true;
    setTimeout(() => { this.showToast = false; }, 3000);
  }
}



