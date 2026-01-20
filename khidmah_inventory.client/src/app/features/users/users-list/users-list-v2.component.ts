import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { UserApiService } from '../../../core/services/user-api.service';
import { User, FilterRequest, PaginationDto, SearchRequest, SearchMode } from '../../../core/models/user.model';
import { DataTableComponent } from '../../../shared/components/data-table/data-table.component';
import { DataTableColumn, DataTableAction, DataTableConfig } from '../../../shared/models/data-table.model';
import { ToastComponent } from '../../../shared/components/toast/toast.component';
import { PermissionService } from '../../../core/services/permission.service';
import { HasPermissionDirective } from '../../../shared/directives/has-permission.directive';

@Component({
  selector: 'app-users-list-v2',
  standalone: true,
  imports: [CommonModule, DataTableComponent, ToastComponent, HasPermissionDirective],
  templateUrl: './users-list-v2.component.html'
})
export class UsersListV2Component implements OnInit {
  users: User[] = [];
  loading = false;
  pagedResult: any = null;

  filterRequest: FilterRequest = {
    pagination: {
      pageNo: 1,
      pageSize: 10,
      sortBy: 'FirstName',
      sortOrder: 'ascending'
    },
    search: {
      term: '',
      searchFields: ['FirstName', 'LastName', 'Email', 'UserName'],
      mode: SearchMode.Contains,
      isCaseSensitive: false
    }
  };

  columns: DataTableColumn<User>[] = [
    {
      key: 'firstName',
      label: 'First Name',
      sortable: true,
      searchable: true,
      filterable: true,
      width: '150px'
    },
    {
      key: 'lastName',
      label: 'Last Name',
      sortable: true,
      searchable: true,
      filterable: true,
      width: '150px'
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
      width: '120px'
    },
    {
      key: 'roles',
      label: 'Roles',
      sortable: false,
      type: 'badge',
      width: '150px',
      render: (row) => row.roles.join(', ')
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
      type: 'boolean',
      width: '100px',
      render: (row) => row.isActive ? 'Active' : 'Inactive'
    },
    {
      key: 'lastLoginAt',
      label: 'Last Login',
      sortable: true,
      type: 'date',
      width: '150px',
      format: (value) => value ? new Date(value).toLocaleString() : 'Never'
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
      condition: (row) => row.isActive && this.permissionService.hasPermission('Users:Update'),
      class: 'danger',
      tooltip: 'Deactivate user'
    },
    {
      label: 'Activate',
      icon: 'check',
      action: (row) => this.activateUser(row),
      condition: (row) => !row.isActive && this.permissionService.hasPermission('Users:Update'),
      class: 'success',
      tooltip: 'Activate user'
    }
  ];

  config: DataTableConfig = {
    showSearch: true,
    showFilters: true,
    showColumnToggle: true,
    showPagination: true,
    showActions: true,
    showCheckbox: false,
    pageSize: 10,
    pageSizeOptions: [5, 10, 25, 50, 100],
    searchPlaceholder: 'Search users...',
    emptyMessage: 'No users found',
    loadingMessage: 'Loading users...'
  };

  showToast = false;
  toastMessage = '';
  toastType: 'success' | 'error' | 'warning' | 'info' = 'success';

  constructor(
    private userApiService: UserApiService,
    private router: Router,
    public permissionService: PermissionService
  ) {}

  ngOnInit(): void {
    this.loadUsers();
  }

  loadUsers(): void {
    this.loading = true;
    this.userApiService.getUsers(this.filterRequest).subscribe({
      next: (response) => {
        if (response.success && response.data) {
          this.pagedResult = response.data;
          this.users = response.data.items;
        } else {
          this.showToastMessage('error', response.message || 'Failed to load users');
        }
        this.loading = false;
      },
      error: (error) => {
        this.showToastMessage('error', 'Error loading users');
        this.loading = false;
      }
    });
  }

  onFilterChange(filterRequest: FilterRequest): void {
    this.filterRequest = filterRequest;
    this.loadUsers();
  }

  onPageChange(pageNo: number): void {
    this.loadUsers();
  }

  onSortChange(sort: any): void {
    this.loadUsers();
  }

  onRowClick(user: User): void {
    this.viewUser(user);
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
        next: (response) => {
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
        next: (response) => {
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


