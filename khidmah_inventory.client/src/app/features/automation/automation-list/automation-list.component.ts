import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Router } from '@angular/router';
import { AutomationApiService } from '../../../core/services/automation-api.service';
import { AutomationRule } from '../../../core/models/automation.model';
import { ApiResponse } from '../../../core/models/api-response.model';
import { ToastComponent } from '../../../shared/components/toast/toast.component';
import { LoadingSpinnerComponent } from '../../../shared/components/loading-spinner/loading-spinner.component';
import { UnifiedCardComponent } from '../../../shared/components/unified-card/unified-card.component';
import { ContentLoaderComponent } from '../../../shared/components/content-loader/content-loader.component';
import { SkeletonListingHeaderComponent } from '../../../shared/components/skeleton-listing-header/skeleton-listing-header.component';
import { SkeletonTableComponent } from '../../../shared/components/skeleton-table/skeleton-table.component';
import { DataTableComponent } from '../../../shared/components/data-table/data-table.component';
import { DataTableColumn, DataTableAction, DataTableConfig } from '../../../shared/models/data-table.model';
import { FilterRequest } from '../../../core/models/user.model';
import { SearchMode } from '../../../core/models/user.model';
import { applyClientSideTable } from '../../../shared/utils/client-side-table.util';

@Component({
  selector: 'app-automation-list',
  standalone: true,
  imports: [CommonModule, RouterModule, ToastComponent, LoadingSpinnerComponent, UnifiedCardComponent, ContentLoaderComponent, SkeletonListingHeaderComponent, SkeletonTableComponent, DataTableComponent],
  templateUrl: './automation-list.component.html'
})
export class AutomationListComponent implements OnInit {
  rules: AutomationRule[] = [];
  pagedResult: any = null;
  loading = true;
  showToast = false;
  toastMessage = '';
  toastType: 'success' | 'error' | 'info' = 'success';

  filterRequest: FilterRequest = {
    pagination: { pageNo: 1, pageSize: 10, sortBy: 'name', sortOrder: 'ascending' },
    search: { term: '', searchFields: ['name', 'condition', 'action'], mode: SearchMode.Contains, isCaseSensitive: false },
    filters: []
  };

  columns: DataTableColumn<AutomationRule>[] = [
    { key: 'name', label: 'Name', sortable: true, searchable: true, width: '180px', render: (r) => r.description ? `${r.name} — ${r.description}` : r.name },
    { key: 'condition', label: 'If', sortable: true, searchable: true, width: '120px', render: (r) => r.condition?.type ?? '' },
    { key: 'action', label: 'Then', sortable: true, searchable: true, width: '120px', render: (r) => r.action?.type ?? '' },
    { key: 'runCount', label: 'Runs', sortable: true, width: '80px', type: 'number' },
    { key: 'isActive', label: 'Status', sortable: true, type: 'badge', width: '90px', format: (v) => v ? 'Active' : 'Inactive', filterable: true, filterType: 'select', filterOptions: [{ label: 'All', value: '' }, { label: 'Active', value: 'true' }, { label: 'Inactive', value: 'false' }] }
  ];

  actions: DataTableAction<AutomationRule>[] = [
    { label: 'History', icon: 'clock-history', action: (row) => this.router.navigate(['/automation', 'history'], { queryParams: { ruleId: row.id } }) },
    { label: 'Delete', icon: 'trash', class: 'danger', action: (row) => this.deleteRule(row.id) }
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
    searchPlaceholder: 'Search rules...',
    emptyMessage: 'No automation rules. Create one to get started.'
  };

  constructor(
    private automationApi: AutomationApiService,
    private router: Router
  ) {}

  private getCellValue(row: AutomationRule, key: string): any {
    if (key === 'condition') return row.condition?.type ?? '';
    if (key === 'action') return row.action?.type ?? '';
    if (key === 'isActive') return row.isActive ? 'true' : 'false';
    return (row as any)[key];
  }

  ngOnInit(): void {
    this.automationApi.getRules().subscribe({
      next: (res: ApiResponse<AutomationRule[]>) => {
        this.loading = false;
        if (res.success && res.data) this.rules = res.data;
        else this.loadFromMock();
        this.applyTable();
      },
      error: () => {
        this.loading = false;
        this.loadFromMock();
      }
    });
  }

  loadFromMock(): void {
    this.automationApi.getRulesMock().subscribe(res => {
      if (res.success && res.data) {
        this.rules = res.data;
        this.applyTable();
      }
    });
  }

  private applyTable(): void {
    const { pagedResult } = applyClientSideTable(this.rules, this.filterRequest, ['name', 'condition', 'action'] as (keyof AutomationRule)[], (r, k) => this.getCellValue(r, k));
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

  deleteRule(id: string): void {
    if (!confirm('Delete this rule?')) return;
    this.automationApi.deleteRule(id).subscribe({
      next: () => {
        this.rules = this.rules.filter(r => r.id !== id);
        this.applyTable();
        this.toastMessage = 'Rule deleted';
        this.toastType = 'success';
        this.showToast = true;
      },
      error: () => {
        this.rules = this.rules.filter(r => r.id !== id);
        this.applyTable();
        this.toastMessage = 'Deleted (mock)';
        this.showToast = true;
      }
    });
  }
}
