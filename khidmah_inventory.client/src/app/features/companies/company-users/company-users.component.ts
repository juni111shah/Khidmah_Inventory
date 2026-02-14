import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';
import { UserApiService } from '../../../core/services/user-api.service';
import { User } from '../../../core/models/user.model';
import { ApiResponse, PagedResult } from '../../../core/models/api-response.model';
import { ToastComponent } from '../../../shared/components/toast/toast.component';
import { LoadingSpinnerComponent } from '../../../shared/components/loading-spinner/loading-spinner.component';
import { UnifiedCardComponent } from '../../../shared/components/unified-card/unified-card.component';
import { ContentLoaderComponent } from '../../../shared/components/content-loader/content-loader.component';
import { SkeletonDetailHeaderComponent } from '../../../shared/components/skeleton-detail-header/skeleton-detail-header.component';
import { SkeletonTableComponent } from '../../../shared/components/skeleton-table/skeleton-table.component';
import { DataTableComponent } from '../../../shared/components/data-table/data-table.component';
import { DataTableColumn, DataTableAction, DataTableConfig } from '../../../shared/models/data-table.model';
import { FilterRequest } from '../../../core/models/user.model';
import { SearchMode } from '../../../core/models/user.model';
import { applyClientSideTable } from '../../../shared/utils/client-side-table.util';

@Component({
  selector: 'app-company-users',
  standalone: true,
  imports: [CommonModule, RouterModule, ToastComponent, LoadingSpinnerComponent, UnifiedCardComponent, ContentLoaderComponent, SkeletonDetailHeaderComponent, SkeletonTableComponent, DataTableComponent],
  templateUrl: './company-users.component.html'
})
export class CompanyUsersComponent implements OnInit {
  loading = true;
  companyId: string | null = null;
  companyName = '';
  users: User[] = [];
  pagedResult: any = null;

  filterRequest: FilterRequest = {
    pagination: { pageNo: 1, pageSize: 10, sortBy: 'firstName', sortOrder: 'ascending' },
    search: { term: '', searchFields: ['firstName', 'lastName', 'email', 'roles'], mode: SearchMode.Contains, isCaseSensitive: false },
    filters: []
  };

  columns: DataTableColumn<User>[] = [
    { key: 'firstName', label: 'User', sortable: true, searchable: true, width: '180px', render: (u) => `${u.firstName || ''} ${u.lastName || ''}`.trim() },
    { key: 'email', label: 'Email', sortable: true, searchable: true, width: '220px' },
    { key: 'roles', label: 'Roles', sortable: true, searchable: true, width: '200px', render: (u) => (u.roles && u.roles.length) ? u.roles.join(', ') : '-' }
  ];

  actions: DataTableAction<User>[] = [
    { label: 'Edit', icon: 'pencil', action: (row) => this.router.navigate(['/users', row.id]) }
  ];

  config: DataTableConfig = {
    showSearch: true,
    showFilters: false,
    showColumnToggle: false,
    showPagination: true,
    showActions: true,
    showCheckbox: false,
    pageSize: 10,
    pageSizeOptions: [5, 10, 25, 50, 100],
    searchPlaceholder: 'Search users...',
    emptyMessage: 'No users assigned.'
  };

  constructor(
    private route: ActivatedRoute,
    private authService: AuthService,
    private userApi: UserApiService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.companyId = this.route.snapshot.paramMap.get('id');
    if (!this.companyId) {
      this.loading = false;
      return;
    }
    const user = this.authService.getCurrentUser();
    const company = user?.companies?.find(c => c.id === this.companyId);
    if (company) this.companyName = company.name;
    this.userApi.getUsers({ pagination: { pageNo: 1, pageSize: 500 } }).subscribe({
      next: (res: ApiResponse<PagedResult<User>>) => {
        this.loading = false;
        if (res.success && res.data?.items) {
          this.users = res.data.items.filter(u => u.companies?.some((c: any) => c.id === this.companyId));
          if (this.users.length === 0) this.users = res.data.items;
        } else this.users = [];
        this.applyTable();
      },
      error: () => { this.loading = false; this.applyTable(); }
    });
  }

  private getCellValue(row: User, key: string): any {
    if (key === 'roles') return (row.roles && row.roles.length) ? row.roles.join(', ') : '';
    return (row as any)[key];
  }

  private applyTable(): void {
    const { pagedResult } = applyClientSideTable(this.users, this.filterRequest, ['firstName', 'lastName', 'email', 'roles'] as (keyof User)[], (r, k) => this.getCellValue(r, k));
    this.pagedResult = pagedResult;
  }

  onFilterChange(req?: FilterRequest): void {
    if (req) this.filterRequest = req;
    this.applyTable();
  }

  onPageChange(pageNo: number): void {
    if (this.filterRequest.pagination) this.filterRequest.pagination.pageNo = pageNo;
    this.applyTable();
  }

  onSortChange(sort: { column: string; direction: 'asc' | 'desc' }): void {
    if (this.filterRequest.pagination) {
      this.filterRequest.pagination.sortBy = sort.column;
      this.filterRequest.pagination.sortOrder = sort.direction === 'asc' ? 'ascending' : 'descending';
    }
    this.applyTable();
  }
}
