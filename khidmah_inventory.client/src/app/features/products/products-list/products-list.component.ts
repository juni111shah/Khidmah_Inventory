import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { ProductApiService } from '../../../core/services/product-api.service';
import { CategoryApiService } from '../../../core/services/category-api.service';
import { Product, GetProductsListQuery } from '../../../core/models/product.model';
import { Category } from '../../../core/models/category.model';
import { DataTableComponent } from '../../../shared/components/data-table/data-table.component';
import { DataTableColumn, DataTableAction, DataTableConfig } from '../../../shared/models/data-table.model';
import { ToastComponent } from '../../../shared/components/toast/toast.component';
import { PermissionService } from '../../../core/services/permission.service';
import { HasPermissionDirective } from '../../../shared/directives/has-permission.directive';
import { IconComponent } from '../../../shared/components/icon/icon.component';
import { FilterRequest, SearchMode } from '../../../core/models/user.model';
import { FormFieldComponent, FormFieldOption } from '../../../shared/components/form-field/form-field.component';
import { UnifiedButtonComponent } from '../../../shared/components/unified-button/unified-button.component';
import { ListingContainerComponent } from '../../../shared/components/listing-container/listing-container.component';
import { FilterFieldComponent } from '../../../shared/components/filter-field/filter-field.component';
import { FilterPanelComponent, FilterPanelConfig, FilterPanelField } from '../../../shared/components/filter-panel/filter-panel.component';
import { ExportComponent } from '../../../shared/components/export/export.component';
import { HeaderService } from '../../../core/services/header.service';

@Component({
  selector: 'app-products-list',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    DataTableComponent,
    ToastComponent,
    HasPermissionDirective,
    IconComponent,
    FormFieldComponent,
    UnifiedButtonComponent,
    ListingContainerComponent,
    FilterFieldComponent,
    FilterPanelComponent,
    ExportComponent
  ],
  templateUrl: './products-list.component.html'
})
export class ProductsListComponent implements OnInit {
  products: Product[] = [];
  categories: Category[] = [];
  loading = false;
  searchTerm = '';
  selectedCategoryId: string | null = null;
  selectedBrandId: string | null = null;
  isActiveFilter: any = '';

  pagedResult: any = null;
  filterRequest: FilterRequest = {
    pagination: {
      pageNo: 1,
      pageSize: 10,
      sortBy: 'name',
      sortOrder: 'ascending'
    },
    search: {
      term: '',
      searchFields: ['name', 'sku', 'barcode', 'description'],
      mode: SearchMode.Contains,
      isCaseSensitive: false
    }
  };

  columns: DataTableColumn<Product>[] = [
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
      key: 'sku',
      label: 'SKU',
      sortable: true,
      searchable: true,
      filterable: true,
      width: '120px'
    },
    {
      key: 'categoryName',
      label: 'Category',
      sortable: true,
      filterable: true,
      width: '150px'
    },
    {
      key: 'brandName',
      label: 'Brand',
      sortable: true,
      filterable: true,
      width: '120px'
    },
    {
      key: 'purchasePrice',
      label: 'Purchase Price',
      sortable: true,
      filterable: false,
      type: 'number',
      width: '120px',
      format: (value) => `$${value.toFixed(2)}`
    },
    {
      key: 'salePrice',
      label: 'Sale Price',
      sortable: true,
      filterable: false,
      type: 'number',
      width: '120px',
      format: (value) => `$${value.toFixed(2)}`
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

  actions: DataTableAction<Product>[] = [
    {
      label: 'View',
      icon: 'eye',
      action: (row) => this.viewProduct(row),
      tooltip: 'View product details',
      condition: () => this.permissionService.hasPermission('Products:Read')
    },
    {
      label: 'Edit',
      icon: 'pencil',
      action: (row) => this.editProduct(row),
      tooltip: 'Edit product',
      condition: () => this.permissionService.hasPermission('Products:Update')
    },
    {
      label: 'Deactivate',
      icon: 'slash-circle',
      action: (row) => this.deactivateProduct(row),
      condition: (row) => row.isActive && this.permissionService.hasPermission('Products:Update'),
      tooltip: 'Deactivate product'
    },
    {
      label: 'Activate',
      icon: 'check-circle',
      action: (row) => this.activateProduct(row),
      condition: (row) => !row.isActive && this.permissionService.hasPermission('Products:Update'),
      tooltip: 'Activate product'
    },
    {
      label: 'Delete',
      icon: 'trash',
      action: (row) => this.deleteProduct(row),
      condition: () => this.permissionService.hasPermission('Products:Delete'),
      tooltip: 'Delete product',
      class: 'danger'
    }
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
    searchPlaceholder: 'Search products...',
    emptyMessage: 'No products found',
    loadingMessage: 'Loading products...'
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
    private productApiService: ProductApiService,
    private categoryApiService: CategoryApiService,
    public router: Router,
    public permissionService: PermissionService,
    private headerService: HeaderService
  ) {}

  ngOnInit(): void {
    this.headerService.setHeaderInfo({
      title: 'Products',
      description: 'Inventory product catalog'
    });
    this.initializeFilterPanel();
    this.loadProducts();
    this.loadCategories();
  }

  private initializeFilterPanel(): void {
    this.filterPanelConfig = {
      fields: [
        {
          key: 'name',
          label: 'Product Name',
          type: 'text',
          placeholder: 'Filter by product name',
          colSize: 'col-md-6',
          defaultValue: ''
        },
        {
          key: 'sku',
          label: 'SKU',
          type: 'text',
          placeholder: 'Filter by SKU',
          colSize: 'col-md-6',
          defaultValue: ''
        },
        {
          key: 'categoryId',
          label: 'Category',
          type: 'select',
          placeholder: 'All Categories',
          colSize: 'col-md-6',
          defaultValue: null
        },
        {
          key: 'brandId',
          label: 'Brand',
          type: 'text',
          placeholder: 'Filter by brand',
          colSize: 'col-md-6',
          defaultValue: ''
        },
        {
          key: 'purchasePrice',
          label: 'Purchase Price',
          type: 'number',
          placeholder: 'Min purchase price',
          colSize: 'col-md-6',
          defaultValue: null
        },
        {
          key: 'salePrice',
          label: 'Sale Price',
          type: 'number',
          placeholder: 'Min sale price',
          colSize: 'col-md-6',
          defaultValue: null
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
        },
        {
          key: 'createdAt',
          label: 'Created Date',
          type: 'date',
          placeholder: 'Select date',
          colSize: 'col-md-6',
          defaultValue: null
        }
      ],
      showResetButton: true,
      showApplyButton: true,
      layout: 'row'
    };

    // Initialize filter values
    this.filterPanelValues = {
      name: '',
      sku: '',
      categoryId: null,
      brandId: '',
      purchasePrice: null,
      salePrice: null,
      isActive: null,
      createdAt: null
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
    this.loadProducts();
  }

  onFilterToggle(): void {
    this.showFilterPanel = !this.showFilterPanel;
  }

  // Filter panel event handlers
  onFilterApplied(filters: { [key: string]: any }): void {
    // Handle specific filters for API compatibility
    this.selectedCategoryId = filters['categoryId'] || null;
    this.selectedBrandId = filters['brandId'] || null;
    this.isActiveFilter = filters['isActive'] !== undefined ? filters['isActive'] : null;

    // Add additional filters to filterRequest.filters array
    this.filterRequest.filters = [];

    if (filters['name'] && filters['name'].trim()) {
      this.filterRequest.filters.push({
        column: 'Name',
        operator: 'Contains',
        value: filters['name'].trim()
      });
    }

    if (filters['sku'] && filters['sku'].trim()) {
      this.filterRequest.filters.push({
        column: 'Sku',
        operator: 'Contains',
        value: filters['sku'].trim()
      });
    }

    if (filters['purchasePrice'] && filters['purchasePrice'] !== null) {
      this.filterRequest.filters.push({
        column: 'PurchasePrice',
        operator: 'GreaterThanOrEqual',
        value: filters['purchasePrice']
      });
    }

    if (filters['salePrice'] && filters['salePrice'] !== null) {
      this.filterRequest.filters.push({
        column: 'SalePrice',
        operator: 'GreaterThanOrEqual',
        value: filters['salePrice']
      });
    }

    if (filters['createdAt']) {
      this.filterRequest.filters.push({
        column: 'CreatedAt',
        operator: 'GreaterThanOrEqual',
        value: filters['createdAt']
      });
    }

    if (this.filterRequest.pagination) {
      this.filterRequest.pagination.pageNo = 1;
    }
    this.loadProducts();
  }

  onFilterReset(): void {
    this.selectedCategoryId = null;
    this.isActiveFilter = null;
    this.searchTerm = '';
    if (this.filterRequest.search) {
      this.filterRequest.search.term = '';
    }
    if (this.filterRequest.pagination) {
      this.filterRequest.pagination.pageNo = 1;
    }
    this.loadProducts();
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

  onColumnToggle(column: DataTableColumn<Product>): void {
    column.visible = column.visible === false ? true : false;
  }

  addProduct(): void {
    this.router.navigate(['/products/new']);
  }

  loadProducts(): void {
    this.loading = true;
    const filterRequest: GetProductsListQuery = {
      filterRequest: this.filterRequest,
      categoryId: this.selectedCategoryId || undefined,
      brandId: this.selectedBrandId || undefined,
      isActive: (this.isActiveFilter === '' || this.isActiveFilter === null) ? undefined : (this.isActiveFilter === 'true' || this.isActiveFilter === true)
    };

    this.productApiService.getProducts(filterRequest).subscribe({
      next: (response) => {
        if (response.success && response.data) {
          this.pagedResult = response.data;
          this.products = response.data.items;
        } else {
          this.showToastMessage('error', response.message || 'Failed to load products');
        }
        this.loading = false;
      },
      error: () => {
        this.showToastMessage('error', 'Error loading products');
        this.loading = false;
      }
    });
  }

  loadCategories(): void {
    this.categoryApiService.getCategories({}).subscribe({
      next: (response) => {
        if (response.success && response.data) {
          this.categories = response.data.items;
          // Update filter panel options when categories are loaded
          this.updateCategoryOptions();
        }
      }
    });
  }

  private updateCategoryOptions(): void {
    const categoryField = this.filterPanelConfig.fields.find(f => f.key === 'categoryId');
    if (categoryField) {
      categoryField.options = [
        { label: 'All Categories', value: null },
        ...this.categories.map(cat => ({ label: cat.name, value: cat.id }))
      ];
    }
  }

  onFilterChange(filterRequest?: FilterRequest): void {
    if (filterRequest) {
      this.filterRequest = filterRequest;
    }
    this.loadProducts();
  }

  onPageChange(pageNo: number): void {
    if (this.filterRequest.pagination) {
      this.filterRequest.pagination.pageNo = pageNo;
    }
    this.loadProducts();
  }

  onSortChange(sort: any): void {
    if (this.filterRequest.pagination) {
      this.filterRequest.pagination.sortBy = sort.column;
      this.filterRequest.pagination.sortOrder = sort.direction === 'asc' ? 'ascending' : 'descending';
    }
    this.loadProducts();
  }

  viewProduct(product: Product): void {
    this.router.navigate(['/products', product.id]);
  }

  editProduct(product: Product): void {
    this.router.navigate(['/products', product.id, 'edit']);
  }

  activateProduct(product: Product): void {
    this.loading = true;
    this.productApiService.activateProduct(product.id).subscribe({
      next: (response) => {
        if (response.success) {
          this.showToastMessage('success', 'Product activated successfully');
          this.loadProducts();
        } else {
          this.showToastMessage('error', response.message || 'Failed to activate product');
        }
        this.loading = false;
      },
      error: () => {
        this.showToastMessage('error', 'Error activating product');
        this.loading = false;
      }
    });
  }

  deactivateProduct(product: Product): void {
    if (confirm(`Are you sure you want to deactivate "${product.name}"?`)) {
      this.loading = true;
      this.productApiService.deactivateProduct(product.id).subscribe({
        next: (response) => {
          if (response.success) {
            this.showToastMessage('success', 'Product deactivated successfully');
            this.loadProducts();
          } else {
            this.showToastMessage('error', response.message || 'Failed to deactivate product');
          }
          this.loading = false;
        },
        error: () => {
          this.showToastMessage('error', 'Error deactivating product');
          this.loading = false;
        }
      });
    }
  }

  deleteProduct(product: Product): void {
    if (confirm(`Are you sure you want to delete "${product.name}"? This action cannot be undone.`)) {
      this.loading = true;
      this.productApiService.deleteProduct(product.id).subscribe({
        next: (response) => {
          if (response.success) {
            this.showToastMessage('success', 'Product deleted successfully');
            this.loadProducts();
          } else {
            this.showToastMessage('error', response.message || 'Failed to delete product');
          }
          this.loading = false;
        },
        error: () => {
          this.showToastMessage('error', 'Error deleting product');
          this.loading = false;
        }
      });
    }
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
