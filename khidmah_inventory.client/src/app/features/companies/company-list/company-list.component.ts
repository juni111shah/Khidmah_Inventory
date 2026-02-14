import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Router } from '@angular/router';
import { CompanyApiService } from '../../../core/services/company-api.service';
import { AuthService } from '../../../core/services/auth.service';
import { Company } from '../../../core/models/company.model';
import { ApiResponse, PagedResult } from '../../../core/models/api-response.model';
import { ToastComponent } from '../../../shared/components/toast/toast.component';
import { LoadingSpinnerComponent } from '../../../shared/components/loading-spinner/loading-spinner.component';
import { UnifiedCardComponent } from '../../../shared/components/unified-card/unified-card.component';
import { KpiStatCardComponent } from '../../../shared/components/kpi-stat-card/kpi-stat-card.component';
import { ContentLoaderComponent } from '../../../shared/components/content-loader/content-loader.component';
import { SkeletonStatCardsComponent } from '../../../shared/components/skeleton-stat-cards/skeleton-stat-cards.component';
import { SkeletonTableComponent } from '../../../shared/components/skeleton-table/skeleton-table.component';
import { DataTableComponent } from '../../../shared/components/data-table/data-table.component';
import { DataTableColumn, DataTableAction, DataTableConfig } from '../../../shared/models/data-table.model';
import { FilterRequest } from '../../../core/models/user.model';
import { SearchMode } from '../../../core/models/user.model';
import { applyClientSideTable } from '../../../shared/utils/client-side-table.util';

@Component({
  selector: 'app-company-list',
  standalone: true,
  imports: [CommonModule, RouterModule, ToastComponent, LoadingSpinnerComponent, UnifiedCardComponent, KpiStatCardComponent, ContentLoaderComponent, SkeletonStatCardsComponent, SkeletonTableComponent, DataTableComponent],
  templateUrl: './company-list.component.html'
})
export class CompanyListComponent implements OnInit {
  loading = true;
  companies: Company[] = [];
  pagedResult: any = null;
  showToast = false;
  toastMessage = '';
  toastType: 'success' | 'error' | 'info' = 'success';

  filterRequest: FilterRequest = {
    pagination: { pageNo: 1, pageSize: 10, sortBy: 'name', sortOrder: 'ascending' },
    search: { term: '', searchFields: ['name'], mode: SearchMode.Contains, isCaseSensitive: false },
    filters: []
  };

  columns: DataTableColumn<Company>[] = [
    { key: 'logoUrl', label: 'Logo', sortable: false, type: 'image', width: '60px' },
    { key: 'name', label: 'Name', sortable: true, searchable: true, width: '200px' },
    { key: 'isActive', label: 'Status', sortable: true, type: 'badge', width: '100px', format: (v) => v ? 'Active' : 'Inactive', filterable: true, filterType: 'select', filterOptions: [{ label: 'All', value: '' }, { label: 'Active', value: 'true' }, { label: 'Inactive', value: 'false' }] }
  ];

  actions: DataTableAction<Company>[] = [
    { label: 'Edit', icon: 'pencil', action: (row) => this.router.navigate(['/companies', row.id]) },
    { label: 'Users', icon: 'people', action: (row) => this.router.navigate(['/companies', row.id, 'users']) }
  ];

  config: DataTableConfig = {
    showSearch: true,
    showFilters: true,
    showColumnToggle: false,
    showPagination: true,
    showActions: true,
    showCheckbox: false,
    pageSize: 10,
    pageSizeOptions: [5, 10, 25, 50, 100],
    searchPlaceholder: 'Search companies...',
    emptyMessage: 'No companies.'
  };

  constructor(
    private companyApi: CompanyApiService,
    private authService: AuthService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadCompanies();
  }

  private applyTable(): void {
    const { pagedResult } = applyClientSideTable(
      this.companies,
      this.filterRequest,
      ['name'] as (keyof Company)[],
      (row, key) => key === 'isActive' ? (row.isActive ? 'true' : 'false') : (row as any)[key]
    );
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

  get activeCount(): number {
    return this.companies.filter(c => c.isActive).length;
  }
  get inactiveCount(): number {
    return this.companies.filter(c => !c.isActive).length;
  }

  loadCompanies(): void {
    this.companyApi.getList({ pagination: { pageNo: 1, pageSize: 500 } }).subscribe({
      next: (res: ApiResponse<PagedResult<Company>>) => {
        this.loading = false;
        if (res.success && res.data?.items) this.companies = res.data.items;
        else this.companies = [];
        this.applyTable();
      },
      error: () => {
        this.loading = false;
        const user = this.authService.getCurrentUser();
        if (user?.companies?.length) {
          this.companies = user.companies.map(c => ({
            id: c.id,
            name: c.name,
            isActive: c.isActive,
            logoUrl: undefined
          })) as Company[];
        } else this.companies = [];
        this.applyTable();
      }
    });
  }

  showToastMsg(msg: string, type: 'success' | 'error' | 'info'): void {
    this.toastMessage = msg;
    this.toastType = type;
    this.showToast = true;
  }
}
