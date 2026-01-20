import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { CategoryApiService } from '../../../core/services/category-api.service';
import { Category, GetCategoriesListQuery } from '../../../core/models/category.model';
import { DataTableComponent } from '../../../shared/components/data-table/data-table.component';
import { DataTableColumn, DataTableAction, DataTableConfig } from '../../../shared/models/data-table.model';
import { ToastComponent } from '../../../shared/components/toast/toast.component';
import { PermissionService } from '../../../core/services/permission.service';
import { HasPermissionDirective } from '../../../shared/directives/has-permission.directive';
import { IconComponent } from '../../../shared/components/icon/icon.component';
import { FilterRequest, SearchMode } from '../../../core/models/user.model';
import { FormFieldComponent, FormFieldOption } from '../../../shared/components/form-field/form-field.component';
import { UnifiedButtonComponent } from '../../../shared/components/unified-button/unified-button.component';
import { HeaderService } from '../../../core/services/header.service';
import { ListingContainerComponent } from '../../../shared/components/listing-container/listing-container.component';
import { FilterFieldComponent } from '../../../shared/components/filter-field/filter-field.component';
import { FilterPanelComponent, FilterPanelConfig, FilterPanelField } from '../../../shared/components/filter-panel/filter-panel.component';
import { ExportComponent } from '../../../shared/components/export/export.component';

@Component({
  selector: 'app-categories-list',
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
  templateUrl: './categories-list.component.html'
})
export class CategoriesListComponent implements OnInit {
  categories: Category[] = [];
  loading = false;
  searchTerm = '';
  selectedParentId: string | null = null;
  parentCategories: Category[] = [];

  columns: DataTableColumn<Category>[] = [
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
      key: 'parentCategoryName',
      label: 'Parent Category',
      sortable: true,
      filterable: true,
      width: '150px'
    },
    {
      key: 'productCount',
      label: 'Products',
      sortable: true,
      filterable: false,
      type: 'number',
      width: '100px'
    },
    {
      key: 'subCategoryCount',
      label: 'Subcategories',
      sortable: true,
      filterable: false,
      type: 'number',
      width: '120px'
    },
    {
      key: 'displayOrder',
      label: 'Order',
      sortable: true,
      filterable: false,
      type: 'number',
      width: '80px'
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

  actions: DataTableAction<Category>[] = [
    {
      label: 'View',
      icon: 'eye',
      action: (row) => this.viewCategory(row),
      tooltip: 'View category details',
      condition: () => this.permissionService.hasPermission('Categories:Read')
    },
    {
      label: 'Edit',
      icon: 'pencil',
      action: (row) => this.editCategory(row),
      tooltip: 'Edit category',
      condition: () => this.permissionService.hasPermission('Categories:Update')
    },
    {
      label: 'Delete',
      icon: 'trash',
      action: (row) => this.deleteCategory(row),
      condition: (row) => row.productCount === 0 && row.subCategoryCount === 0 && this.permissionService.hasPermission('Categories:Delete'),
      tooltip: 'Delete category',
      class: 'danger'
    }
  ];

  pagedResult: any = null;
  filterRequest: FilterRequest = {
    pagination: {
      pageNo: 1,
      pageSize: 10,
      sortBy: 'displayOrder',
      sortOrder: 'ascending'
    },
    search: {
      term: '',
      searchFields: ['name', 'code', 'description'],
      mode: SearchMode.Contains,
      isCaseSensitive: false
    }
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
    searchPlaceholder: 'Search categories...',
    emptyMessage: 'No categories found',
    loadingMessage: 'Loading categories...'
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
    private categoryApiService: CategoryApiService,
    public router: Router,
    public permissionService: PermissionService,
    private headerService: HeaderService
  ) {}

  ngOnInit(): void {
    this.headerService.setHeaderInfo({
      title: 'Categories',
      description: 'Manage product categories and hierarchy'
    });
    this.initializeFilterPanel();
    this.loadCategories();
    this.loadParentCategories();
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
          key: 'parentId',
          label: 'Parent Category',
          type: 'select',
          placeholder: 'All Categories',
          colSize: 'col-md-6',
          defaultValue: null
        },
        {
          key: 'productCount',
          label: 'Products',
          type: 'number',
          placeholder: 'Min products',
          colSize: 'col-md-6',
          defaultValue: null
        },
        {
          key: 'subCategoryCount',
          label: 'Subcategories',
          type: 'number',
          placeholder: 'Min subcategories',
          colSize: 'col-md-6',
          defaultValue: null
        },
        {
          key: 'displayOrder',
          label: 'Display Order',
          type: 'number',
          placeholder: 'Min order',
          colSize: 'col-md-6',
          defaultValue: null
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
      code: '',
      parentId: null,
      productCount: null,
      subCategoryCount: null,
      displayOrder: null,
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
    this.loadCategories();
  }

  onFilterToggle(): void {
    this.showFilterPanel = !this.showFilterPanel;
  }

  // Filter panel event handlers
  onFilterApplied(filters: { [key: string]: any }): void {
    this.selectedParentId = filters['parentId'] || null;

    if (this.filterRequest.pagination) {
      this.filterRequest.pagination.pageNo = 1;
    }
    this.loadCategories();
  }

  onFilterReset(): void {
    this.selectedParentId = null;
    this.searchTerm = '';
    if (this.filterRequest.search) {
      this.filterRequest.search.term = '';
    }
    if (this.filterRequest.pagination) {
      this.filterRequest.pagination.pageNo = 1;
    }
    this.loadCategories();
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

  onColumnToggle(column: DataTableColumn<Category>): void {
    column.visible = column.visible === false ? true : false;
  }

  addCategory(): void {
    this.router.navigate(['/categories/new']);
  }

  loadCategories(): void {
    this.loading = true;
    const filterRequest: GetCategoriesListQuery = {
      filterRequest: this.filterRequest,
      parentCategoryId: this.selectedParentId || undefined
    };

    this.categoryApiService.getCategories(filterRequest).subscribe({
      next: (response) => {
        if (response.success && response.data) {
          this.pagedResult = response.data;
          this.categories = response.data.items;
        } else {
          this.showToastMessage('error', response.message || 'Failed to load categories');
        }
        this.loading = false;
      },
      error: () => {
        this.showToastMessage('error', 'Error loading categories');
        this.loading = false;
      }
    });
  }

  loadParentCategories(): void {
    this.categoryApiService.getCategories({}).subscribe({
      next: (response) => {
        if (response.success && response.data) {
          this.parentCategories = response.data.items;
          // Update filter panel options when parent categories are loaded
          this.updateParentCategoryOptions();
        }
      }
    });
  }

  private updateParentCategoryOptions(): void {
    const parentField = this.filterPanelConfig.fields.find(f => f.key === 'parentId');
    if (parentField) {
      parentField.options = [
        { label: 'All Categories', value: null },
        { label: 'Root Categories Only', value: '' },
        ...this.parentCategories.map(cat => ({ label: cat.name, value: cat.id }))
      ];
    }
  }

  onParentFilterChange(): void {
    this.loadCategories();
  }

  onFilterChange(filterRequest?: FilterRequest): void {
    if (filterRequest) {
      this.filterRequest = filterRequest;
    }
    this.loadCategories();
  }

  onPageChange(pageNo: number): void {
    if (this.filterRequest.pagination) {
      this.filterRequest.pagination.pageNo = pageNo;
    }
    this.loadCategories();
  }

  onSortChange(sort: any): void {
    if (this.filterRequest.pagination) {
      this.filterRequest.pagination.sortBy = sort.column;
      this.filterRequest.pagination.sortOrder = sort.direction === 'asc' ? 'ascending' : 'descending';
    }
    this.loadCategories();
  }

  viewCategory(category: Category): void {
    this.router.navigate(['/categories', category.id]);
  }

  editCategory(category: Category): void {
    this.router.navigate(['/categories', category.id, 'edit']);
  }

  deleteCategory(category: Category): void {
    if (confirm(`Are you sure you want to delete category "${category.name}"?`)) {
      this.loading = true;
      this.categoryApiService.deleteCategory(category.id).subscribe({
        next: (response) => {
          if (response.success) {
            this.showToastMessage('success', 'Category deleted successfully');
            this.loadCategories();
          } else {
            this.showToastMessage('error', response.message || 'Failed to delete category');
          }
          this.loading = false;
        },
        error: () => {
          this.showToastMessage('error', 'Error deleting category');
          this.loading = false;
        }
      });
    }
  }

  getParentFilterOptions(): FormFieldOption[] {
    const options: FormFieldOption[] = [
      { value: null, label: 'All Categories' },
      { value: '', label: 'Root Categories Only' }
    ];
    
    this.parentCategories.forEach(parent => {
      options.push({ value: parent.id, label: parent.name });
    });
    
    return options;
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
