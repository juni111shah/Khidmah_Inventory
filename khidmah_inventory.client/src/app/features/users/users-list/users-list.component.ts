import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { Subject, Subscription } from 'rxjs';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { User, PagedResult, FilterRequest, FilterDto, SearchMode } from '../../../core/models/user.model';
import { ApiResponse } from '../../../core/models/api-response.model';
import { IconComponent } from '../../../shared/components/icon/icon.component';
import { ToastComponent } from '../../../shared/components/toast/toast.component';
import { DataTableComponent } from '../../../shared/components/data-table/data-table.component';
import { DataTableColumn, DataTableAction, DataTableConfig } from '../../../shared/models/data-table.model';
import { HasPermissionDirective } from '../../../shared/directives/has-permission.directive';
import { PermissionService } from '../../../core/services/permission.service';
import { UserApiService } from '../../../core/services/user-api.service';
import { ListingContainerComponent } from '../../../shared/components/listing-container/listing-container.component';
import { UnifiedButtonComponent } from '../../../shared/components/unified-button/unified-button.component';
import { HeaderService } from '../../../core/services/header.service';
import { FilterFieldComponent } from '../../../shared/components/filter-field/filter-field.component';
import { FilterPanelComponent, FilterPanelConfig, FilterPanelField } from '../../../shared/components/filter-panel/filter-panel.component';
import { ExportComponent } from '../../../shared/components/export/export.component';
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
  selector: 'app-users-list',
  standalone: true,
  imports: [
    CommonModule, FormsModule, ReactiveFormsModule, IconComponent, ToastComponent, DataTableComponent, HasPermissionDirective,
    ListingContainerComponent, UnifiedButtonComponent, FilterFieldComponent, FilterPanelComponent, ExportComponent,
    FilterBuilderComponent, SavedViewsDropdownComponent,
    ContentLoaderComponent, SkeletonListingHeaderComponent, SkeletonTableComponent
  ],
  templateUrl: './users-list.component.html'
})
export class UsersListComponent implements OnInit, OnDestroy {
  readonly tableId = 'users';
  users: User[] = [];
  loading = false;
  searchTerm = '';
  private searchSubject = new Subject<string>();
  private subs = new Subscription();
  showFilterBuilder = false;
  filterRequest: FilterRequest = {
    pagination: { pageNo: 1, pageSize: 10, sortBy: 'FirstName', sortOrder: 'ascending' },
    search: { term: '', searchFields: ['FirstName', 'LastName', 'Email', 'UserName'], mode: SearchMode.Contains, isCaseSensitive: false },
    filters: []
  };

  pagedResult: PagedResult<User> | null = null;

  columns: DataTableColumn<User>[] = [
    {
      key: 'name',
      label: 'Name',
      sortable: true,
      searchable: true,
      filterable: true,
      width: '200px',
      render: (row) => `${row.firstName} ${row.lastName}`
    },
    {
      key: 'email',
      label: 'Email',
      sortable: true,
      searchable: true,
      filterable: true,
      width: '200px'
    },
    {
      key: 'userName',
      label: 'Username',
      sortable: true,
      searchable: true,
      filterable: true,
      width: '150px'
    },
    {
      key: 'phoneNumber',
      label: 'Phone',
      sortable: false,
      filterable: true,
      width: '120px',
      render: (row) => row.phoneNumber || '-'
    },
    {
      key: 'roles',
      label: 'Roles',
      sortable: false,
      filterable: false,
      width: '150px',
      render: (row) => row.roles?.join(', ') || '-'
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
      key: 'lastLoginAt',
      label: 'Last Login',
      sortable: true,
      filterable: false,
      type: 'date',
      width: '150px',
      render: (row) => row.lastLoginAt ? new Date(row.lastLoginAt).toLocaleString() : 'Never'
    }
  ];

  actions: DataTableAction<User>[] = [
    {
      label: 'View',
      icon: 'eye',
      action: (row) => this.viewUser(row),
      tooltip: 'View user details',
      condition: () => this.permissionService.hasPermission('Users:Read')
    },
    {
      label: 'Edit',
      icon: 'edit',
      action: (row) => this.editUser(row),
      tooltip: 'Edit user',
      condition: () => this.permissionService.hasPermission('Users:Update')
    },
    {
      label: 'Deactivate',
      icon: 'ban',
      action: (row) => this.deactivateUser(row),
      tooltip: 'Deactivate user',
      condition: (row) => row.isActive && this.permissionService.hasPermission('Users:Deactivate'),
      class: 'danger'
    },
    {
      label: 'Activate',
      icon: 'check',
      action: (row) => this.activateUser(row),
      tooltip: 'Activate user',
      condition: (row) => !row.isActive && this.permissionService.hasPermission('Users:Activate'),
      class: 'success'
    }
  ];

  config: DataTableConfig = {
    showSearch: false, // Handled by parent ListingContainer
    showFilters: false, // Handled by parent ListingContainer
    showColumnToggle: false, // Handled by parent ListingContainer
    showPagination: true,
    showActions: true,
    showCheckbox: false,
    pageSize: 10,
    pageSizeOptions: [5, 10, 25, 50, 100],
    searchPlaceholder: 'Search users...',
    emptyMessage: 'No users found'
  };

  showToast = false;
  toastMessage = '';
  toastType: 'success' | 'error' | 'warning' | 'info' = 'success';

  get tableConfig(): DataTableConfig {
    return this.config;
  }

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
  isActiveFilter: any = '';

  filterBuilderColumns: FilterBuilderColumn[] = [
    { key: 'FirstName', label: 'First Name', type: 'text' },
    { key: 'LastName', label: 'Last Name', type: 'text' },
    { key: 'email', label: 'Email', type: 'text' },
    { key: 'userName', label: 'Username', type: 'text' },
    { key: 'phoneNumber', label: 'Phone', type: 'text' },
    { key: 'isActive', label: 'Status', type: 'boolean', options: [{ label: 'Active', value: true }, { label: 'Inactive', value: false }] },
    { key: 'lastLoginAt', label: 'Last Login', type: 'date' }
  ];

  get currentTableState(): { filterRequest: FilterRequest; sortColumns: { column: string; direction: string }[]; visibleColumnKeys: string[] } {
    const sortColumns = this.filterRequest.pagination?.sortBy ? [{ column: this.filterRequest.pagination.sortBy, direction: this.filterRequest.pagination.sortOrder === 'descending' ? 'desc' : 'asc' }] : [];
    return { filterRequest: this.filterRequest, sortColumns, visibleColumnKeys: this.columns.filter(c => c.visible !== false).map(c => c.key) };
  }

  constructor(
    private userApiService: UserApiService,
    private router: Router,
    public permissionService: PermissionService,
    private headerService: HeaderService,
    private tableState: TableStateService,
    private savedViews: SavedViewsService
  ) {}

  ngOnInit(): void {
    this.headerService.setHeaderInfo({ title: 'Users', description: 'Manage and monitor system users' });
    this.initializeFilterPanel();
    this.restoreTableState();
    this.loadUsers();
    this.searchSubject.pipe(debounceTime(300), distinctUntilChanged()).subscribe(term => {
      this.searchTerm = term;
      if (this.filterRequest.search) this.filterRequest.search.term = term;
      if (this.filterRequest.pagination) this.filterRequest.pagination.pageNo = 1;
      this.persistTableState();
      this.loadUsers();
    });
  }

  ngOnDestroy(): void { this.subs.unsubscribe(); }

  private restoreTableState(): void {
    const state = this.tableState.getState(this.tableId);
    if (!state) return;
    if (state.filterRequest?.pagination) this.filterRequest.pagination = { ...this.filterRequest.pagination, ...state.filterRequest.pagination };
    if (state.filterRequest?.search?.term != null) {
      if (!this.filterRequest.search) this.filterRequest.search = { term: '', searchFields: ['FirstName', 'LastName', 'Email', 'UserName'], mode: SearchMode.Contains, isCaseSensitive: false };
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
          key: 'email',
          label: 'Email',
          type: 'text',
          placeholder: 'Filter by email',
          colSize: 'col-md-6',
          defaultValue: ''
        },
        {
          key: 'userName',
          label: 'Username',
          type: 'text',
          placeholder: 'Filter by username',
          colSize: 'col-md-6',
          defaultValue: ''
        },
        {
          key: 'phoneNumber',
          label: 'Phone',
          type: 'text',
          placeholder: 'Filter by phone',
          colSize: 'col-md-6',
          defaultValue: ''
        },
        {
          key: 'roles',
          label: 'Roles',
          type: 'text',
          placeholder: 'Filter by roles',
          colSize: 'col-md-6',
          defaultValue: ''
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
          key: 'lastLoginAt',
          label: 'Last Login',
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
      email: '',
      userName: '',
      phoneNumber: '',
      roles: '',
      isActive: null,
      lastLoginAt: null
    };
  }

  onSearchTermChange(term: string): void { this.searchSubject.next(term ?? ''); }
  onSearchImmediate(term: string): void {
    this.searchTerm = term ?? '';
    if (this.filterRequest.search) this.filterRequest.search.term = term ?? '';
    if (this.filterRequest.pagination) this.filterRequest.pagination.pageNo = 1;
    this.persistTableState();
    this.loadUsers();
  }

  onFilterToggle(): void {
    this.showFilterPanel = !this.showFilterPanel;
  }

  onStatusFilterChange(value: any): void {
    if (!this.filterRequest.filters) {
      this.filterRequest.filters = [];
    }

    this.filterRequest.filters = this.filterRequest.filters.filter(f => f.column !== 'isActive');

    if (value !== null && value !== '') {
      this.filterRequest.filters.push({
        column: 'isActive',
        operator: 'Equals',
        value: value === 'true' || value === true
      });
    }

    if (this.filterRequest.pagination) {
      this.filterRequest.pagination.pageNo = 1;
    }
    this.loadUsers();
  }

  // Filter panel event handlers
  onFilterApplied(filters: { [key: string]: any }): void {
    // Clear existing filters
    this.filterRequest.filters = [];

    // Apply filters for each field
    if (filters['name'] && filters['name'].trim()) {
      this.filterRequest.filters.push({
        column: 'FirstName',
        operator: 'Contains',
        value: filters['name'].trim()
      });
      this.filterRequest.filters.push({
        column: 'LastName',
        operator: 'Contains',
        value: filters['name'].trim()
      });
    }

    if (filters['email'] && filters['email'].trim()) {
      this.filterRequest.filters.push({
        column: 'Email',
        operator: 'Contains',
        value: filters['email'].trim()
      });
    }

    if (filters['userName'] && filters['userName'].trim()) {
      this.filterRequest.filters.push({
        column: 'UserName',
        operator: 'Contains',
        value: filters['userName'].trim()
      });
    }

    if (filters['phoneNumber'] && filters['phoneNumber'].trim()) {
      this.filterRequest.filters.push({
        column: 'PhoneNumber',
        operator: 'Contains',
        value: filters['phoneNumber'].trim()
      });
    }

    if (filters['roles'] && filters['roles'].trim()) {
      // For roles, we'll search in the roles array (this might need adjustment based on backend)
      this.filterRequest.filters.push({
        column: 'Roles',
        operator: 'Contains',
        value: filters['roles'].trim()
      });
    }

    if (filters['isActive'] !== null && filters['isActive'] !== '') {
      this.filterRequest.filters.push({
        column: 'IsActive',
        operator: 'Equals',
        value: filters['isActive'] === 'true' || filters['isActive'] === true
      });
    }

    if (filters['lastLoginAt']) {
      this.filterRequest.filters.push({
        column: 'LastLoginAt',
        operator: 'GreaterThanOrEqual',
        value: filters['lastLoginAt']
      });
    }

    if (this.filterRequest.pagination) {
      this.filterRequest.pagination.pageNo = 1;
    }
    this.loadUsers();
  }

  onFilterReset(): void {
    this.isActiveFilter = null;
    this.searchTerm = '';
    this.filterRequest.filters = [];
    if (this.filterRequest.search) {
      this.filterRequest.search.term = '';
    }
    if (this.filterRequest.pagination) {
      this.filterRequest.pagination.pageNo = 1;
    }
    this.loadUsers();
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

  onColumnToggle(column: DataTableColumn<User>): void {
    column.visible = column.visible === false ? true : false;
    this.persistTableState();
  }

  addUser(): void {
    this.router.navigate(['/users/new']);
  }

  loadUsers(): void {
    this.loading = true;

    // Apply search if not already applied
    if (this.searchTerm && this.filterRequest.search) {
      this.filterRequest.search.term = this.searchTerm;
    }

    this.userApiService.getUsers(this.filterRequest).subscribe({
      next: (response: ApiResponse<PagedResult<User>>) => {
        if (response.success && response.data) {
          this.pagedResult = response.data;
          this.users = response.data.items;
        } else {
          this.showToastMessage('error', response.message || 'Failed to load users');
        }
        this.loading = false;
      },
      error: (error: any) => {
        this.showToastMessage('error', 'Error loading users');
        this.loading = false;
      }
    });
  }

  onFilterChange(filterRequest: FilterRequest): void {
    this.filterRequest = filterRequest;
    this.persistTableState();
    this.loadUsers();
  }
  onPageChange(pageNo: number): void {
    if (this.filterRequest.pagination) this.filterRequest.pagination.pageNo = pageNo;
    this.persistTableState();
    this.loadUsers();
  }
  onSortChange(sort: any): void {
    if (this.filterRequest.pagination) {
      this.filterRequest.pagination.sortBy = sort.column;
      this.filterRequest.pagination.sortOrder = sort.direction === 'asc' ? 'ascending' : 'descending';
    }
    this.persistTableState();
    this.loadUsers();
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
    this.loadUsers();
  }
  onFilterBuilderApply(filters: FilterDto[]): void {
    this.filterRequest.filters = filters;
    if (this.filterRequest.pagination) this.filterRequest.pagination.pageNo = 1;
    this.persistTableState();
    this.loadUsers();
    this.showFilterBuilder = false;
  }
  onFilterBuilderClear(): void {
    this.filterRequest.filters = [];
    this.persistTableState();
    this.loadUsers();
  }

  viewUser(user: User): void {
    this.router.navigate(['/users', user.id]);
  }

  editUser(user: User): void {
    this.router.navigate(['/users', user.id, 'edit']);
  }

  activateUser(user: User): void {
    if (confirm(`Are you sure you want to activate ${user.firstName} ${user.lastName}?`)) {
      this.userApiService.activateUser(user.id).subscribe({
        next: (response: any) => {
          if (response.success) {
            this.showToastMessage('success', 'User activated successfully');
            this.loadUsers();
          } else {
            this.showToastMessage('error', response.message || 'Failed to activate user');
          }
        },
        error: () => {
          this.showToastMessage('error', 'Error activating user');
        }
      });
    }
  }

  deactivateUser(user: User): void {
    if (confirm(`Are you sure you want to deactivate ${user.firstName} ${user.lastName}?`)) {
      this.userApiService.deactivateUser(user.id).subscribe({
        next: (response: any) => {
          if (response.success) {
            this.showToastMessage('success', 'User deactivated successfully');
            this.loadUsers();
          } else {
            this.showToastMessage('error', response.message || 'Failed to deactivate user');
          }
        },
        error: () => {
          this.showToastMessage('error', 'Error deactivating user');
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



