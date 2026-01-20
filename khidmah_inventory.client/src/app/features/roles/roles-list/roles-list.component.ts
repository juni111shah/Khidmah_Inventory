import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { RoleApiService } from '../../../core/services/role-api.service';
import { Role } from '../../../core/models/role.model';
import { ApiResponse } from '../../../core/models/api-response.model';
import { DataTableComponent } from '../../../shared/components/data-table/data-table.component';
import { DataTableColumn, DataTableAction, DataTableConfig } from '../../../shared/models/data-table.model';
import { ToastComponent } from '../../../shared/components/toast/toast.component';
import { PermissionService } from '../../../core/services/permission.service';
import { HasPermissionDirective } from '../../../shared/directives/has-permission.directive';
import { IconComponent } from '../../../shared/components/icon/icon.component';
import { ListingContainerComponent } from '../../../shared/components/listing-container/listing-container.component';
import { UnifiedButtonComponent } from '../../../shared/components/unified-button/unified-button.component';
import { HeaderService } from '../../../core/services/header.service';
import { FilterFieldComponent } from '../../../shared/components/filter-field/filter-field.component';
import { FilterPanelComponent, FilterPanelConfig, FilterPanelField } from '../../../shared/components/filter-panel/filter-panel.component';
import { ExportComponent } from '../../../shared/components/export/export.component';

@Component({
  selector: 'app-roles-list',
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
  templateUrl: './roles-list.component.html'
})
export class RolesListComponent implements OnInit {
  roles: Role[] = [];
  allRoles: Role[] = [];
  loading = false;
  searchTerm = '';
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
      searchFields: ['name', 'description'],
      mode: 'Contains',
      isCaseSensitive: false
    },
    filters: []
  };

  columns: DataTableColumn<Role>[] = [
    // ... existing columns ...
    {
      key: 'name',
      label: 'Role Name',
      sortable: true,
      searchable: true,
      filterable: true,
      width: '200px'
    },
    {
      key: 'description',
      label: 'Description',
      sortable: false,
      searchable: true,
      filterable: true
    },
    {
      key: 'isSystemRole',
      label: 'Type',
      sortable: true,
      filterable: true,
      filterType: 'select',
      filterOptions: [
        { label: 'All', value: '' },
        { label: 'System', value: true },
        { label: 'Custom', value: false }
      ],
      type: 'badge',
      width: '120px',
      render: (row: Role) => row.isSystemRole ? 'System' : 'Custom'
    },
    {
      key: 'userCount',
      label: 'Users',
      sortable: true,
      filterable: false,
      type: 'number',
      width: '100px'
    },
    {
      key: 'permissionCount',
      label: 'Permissions',
      sortable: true,
      filterable: false,
      type: 'number',
      width: '120px'
    },
    {
      key: 'createdAt',
      label: 'Created',
      sortable: true,
      filterable: false,
      type: 'date',
      width: '150px',
      format: (value: any) => new Date(value).toLocaleDateString()
    }
  ];

  actions: DataTableAction<Role>[] = [
    {
      label: 'View',
      icon: 'eye',
      action: (row: Role) => this.viewRole(row),
      tooltip: 'View role details',
      condition: () => this.permissionService.hasPermission('Roles:Read')
    },
    {
      label: 'Edit',
      icon: 'pencil',
      action: (row: Role) => this.editRole(row),
      tooltip: 'Edit role',
      condition: (row: Role) => !row.isSystemRole && this.permissionService.hasPermission('Roles:Update')
    },
    {
      label: 'Delete',
      icon: 'trash',
      action: (row: Role) => this.deleteRole(row),
      condition: (row: Role) => !row.isSystemRole && row.userCount === 0 && this.permissionService.hasPermission('Roles:Delete'),
      class: 'danger',
      tooltip: 'Delete role'
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
    searchPlaceholder: 'Search roles...',
    emptyMessage: 'No roles found',
    loadingMessage: 'Loading roles...'
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
  isSystemRoleFilter: any = '';

  constructor(
    private roleApiService: RoleApiService,
    public router: Router,
    public permissionService: PermissionService,
    private headerService: HeaderService
  ) {}

  ngOnInit(): void {
    this.headerService.setHeaderInfo({
      title: 'Roles',
      description: 'Manage system roles and permissions'
    });
    this.initializeFilterPanel();
    this.loadRoles();
  }

  private initializeFilterPanel(): void {
    this.filterPanelConfig = {
      fields: [
        {
          key: 'name',
          label: 'Role Name',
          type: 'text',
          placeholder: 'Filter by role name',
          colSize: 'col-md-4',
          defaultValue: ''
        },
        {
          key: 'description',
          label: 'Description',
          type: 'text',
          placeholder: 'Filter by description',
          colSize: 'col-md-4',
          defaultValue: ''
        },
        {
          key: 'isSystemRole',
          label: 'Type',
          type: 'select',
          placeholder: 'All Types',
          colSize: 'col-md-4',
          defaultValue: null,
          options: [
            { label: 'All Types', value: null },
            { label: 'System', value: true },
            { label: 'Custom', value: false }
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
      description: '',
      isSystemRole: null
    };
  }

  onSearch(term: string): void {
    this.searchTerm = term;
    if (this.filterRequest.search) {
      this.filterRequest.search.term = term;
    }
    this.filterRequest.pagination.pageNo = 1;
    this.applyFiltersAndPagination();
  }

  onFilterToggle(): void {
    this.showFilterPanel = !this.showFilterPanel;
  }

  // Filter panel event handlers
  onFilterApplied(filters: { [key: string]: any }): void {
    this.isSystemRoleFilter = filters['isSystemRole'] !== undefined ? filters['isSystemRole'] : '';
    this.applyFiltersAndPagination();
  }

  onFilterReset(): void {
    this.searchTerm = '';
    this.isSystemRoleFilter = '';
    if (this.filterRequest.search) {
      this.filterRequest.search.term = '';
    }
    this.filterRequest.filters = [];
    this.filterRequest.pagination.pageNo = 1;
    this.applyFiltersAndPagination();
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

  onColumnToggle(column: DataTableColumn<Role>): void {
    column.visible = column.visible === false ? true : false;
  }

  addRole(): void {
    this.router.navigate(['/roles/new']);
  }

  loadRoles(): void {
    this.loading = true;
    this.roleApiService.getRoles().subscribe({
      next: (response: ApiResponse<Role[]>) => {
        if (response.success && response.data) {
          this.allRoles = response.data;
          this.applyFiltersAndPagination();
        } else {
          this.showToastMessage('error', response.message || 'Failed to load roles');
          this.loading = false;
        }
      },
      error: () => {
        this.showToastMessage('error', 'Error loading roles');
        this.loading = false;
      }
    });
  }

  applyFiltersAndPagination(): void {
    let filtered = [...this.allRoles];

    // Apply search
    if (this.filterRequest.search?.term) {
      const term = this.filterRequest.search.term.toLowerCase();
      filtered = filtered.filter(role => {
        return (role.name?.toLowerCase().includes(term) || 
                role.description?.toLowerCase().includes(term));
      });
    }

    // Apply filters
    if (this.isSystemRoleFilter !== null && this.isSystemRoleFilter !== '') {
      filtered = filtered.filter(role => role.isSystemRole === (this.isSystemRoleFilter === 'true' || this.isSystemRoleFilter === true));
    }

    // Apply sorting
    if (this.filterRequest.pagination?.sortBy) {
      const sortBy = this.filterRequest.pagination.sortBy;
      const sortOrder = this.filterRequest.pagination.sortOrder === 'descending' ? -1 : 1;
      filtered.sort((a, b) => {
        const aVal = (a as any)[sortBy];
        const bVal = (b as any)[sortBy];
        if (aVal < bVal) return -1 * sortOrder;
        if (aVal > bVal) return 1 * sortOrder;
        return 0;
      });
    }

    // Apply pagination
    const pageNo = this.filterRequest.pagination?.pageNo || 1;
    const pageSize = this.filterRequest.pagination?.pageSize || 10;
    const startIndex = (pageNo - 1) * pageSize;
    const endIndex = startIndex + pageSize;
    
    this.roles = filtered.slice(startIndex, endIndex);
    
    this.pagedResult = {
      items: this.roles,
      totalCount: filtered.length,
      pageNo: pageNo,
      pageSize: pageSize,
      totalPages: Math.ceil(filtered.length / pageSize)
    };

    this.loading = false;
  }

  onFilterChange(filterRequest?: any): void {
    if (filterRequest) {
      this.filterRequest = filterRequest;
    }
    this.filterRequest.pagination.pageNo = 1;
    this.applyFiltersAndPagination();
  }

  onPageChange(pageNo: number): void {
    if (this.filterRequest.pagination) {
      this.filterRequest.pagination.pageNo = pageNo;
    }
    this.applyFiltersAndPagination();
  }

  onSortChange(sort: any): void {
    if (this.filterRequest.pagination) {
      this.filterRequest.pagination.sortBy = sort.column;
      this.filterRequest.pagination.sortOrder = sort.direction === 'asc' ? 'ascending' : 'descending';
    }
    this.applyFiltersAndPagination();
  }

  viewRole(role: Role): void {
    this.router.navigate(['/roles', role.id]);
  }

  editRole(role: Role): void {
    this.router.navigate(['/roles', role.id, 'edit']);
  }

  deleteRole(role: Role): void {
    if (confirm(`Are you sure you want to delete role "${role.name}"?`)) {
      this.roleApiService.deleteRole(role.id).subscribe({
        next: (response: ApiResponse<void>) => {
          if (response.success) {
            this.showToastMessage('success', 'Role deleted successfully');
            this.loadRoles();
          } else {
            this.showToastMessage('error', response.message || 'Failed to delete role');
          }
        },
        error: () => {
          this.showToastMessage('error', 'Error deleting role');
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



