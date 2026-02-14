import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterModule } from '@angular/router';
import { AutomationApiService } from '../../../core/services/automation-api.service';
import { AutomationExecution } from '../../../core/models/automation.model';
import { ApiResponse } from '../../../core/models/api-response.model';
import { ToastComponent } from '../../../shared/components/toast/toast.component';
import { LoadingSpinnerComponent } from '../../../shared/components/loading-spinner/loading-spinner.component';
import { UnifiedCardComponent } from '../../../shared/components/unified-card/unified-card.component';
import { ContentLoaderComponent } from '../../../shared/components/content-loader/content-loader.component';
import { SkeletonDetailHeaderComponent } from '../../../shared/components/skeleton-detail-header/skeleton-detail-header.component';
import { SkeletonTableComponent } from '../../../shared/components/skeleton-table/skeleton-table.component';
import { DataTableComponent } from '../../../shared/components/data-table/data-table.component';
import { DataTableColumn, DataTableConfig } from '../../../shared/models/data-table.model';
import { FilterRequest } from '../../../core/models/user.model';
import { SearchMode } from '../../../core/models/user.model';
import { applyClientSideTable } from '../../../shared/utils/client-side-table.util';

@Component({
  selector: 'app-automation-history',
  standalone: true,
  imports: [CommonModule, RouterModule, ToastComponent, LoadingSpinnerComponent, UnifiedCardComponent, ContentLoaderComponent, SkeletonDetailHeaderComponent, SkeletonTableComponent, DataTableComponent],
  templateUrl: './automation-history.component.html'
})
export class AutomationHistoryComponent implements OnInit {
  executions: AutomationExecution[] = [];
  pagedResult: any = null;
  loading = true;
  ruleId: string | null = null;

  filterRequest: FilterRequest = {
    pagination: { pageNo: 1, pageSize: 10, sortBy: 'triggeredAt', sortOrder: 'descending' },
    search: { term: '', searchFields: ['ruleName', 'status', 'message'], mode: SearchMode.Contains, isCaseSensitive: false },
    filters: []
  };

  columns: DataTableColumn<AutomationExecution>[] = [
    { key: 'triggeredAt', label: 'Time', sortable: true, type: 'date', width: '150px', format: (v) => v ? new Date(v).toLocaleString() : '' },
    { key: 'ruleName', label: 'Rule', sortable: true, searchable: true, width: '180px' },
    { key: 'status', label: 'Status', sortable: true, searchable: true, type: 'badge', width: '100px', filterable: true, filterType: 'select', filterOptions: [{ label: 'All', value: '' }, { label: 'Success', value: 'Success' }, { label: 'Failed', value: 'Failed' }, { label: 'Skipped', value: 'Skipped' }] },
    { key: 'message', label: 'Message', sortable: false, searchable: true, width: '200px', format: (v) => v || '-' }
  ];

  config: DataTableConfig = {
    showSearch: true,
    showFilters: true,
    showColumnToggle: false,
    showPagination: true,
    showActions: false,
    showCheckbox: false,
    pageSize: 10,
    pageSizeOptions: [5, 10, 25, 50, 100],
    searchPlaceholder: 'Search executions...',
    emptyMessage: 'No executions yet.'
  };

  constructor(
    private route: ActivatedRoute,
    private automationApi: AutomationApiService
  ) {}

  ngOnInit(): void {
    this.ruleId = this.route.snapshot.queryParamMap.get('ruleId');
    this.automationApi.getExecutionHistory(this.ruleId || undefined).subscribe({
      next: (res: ApiResponse<AutomationExecution[]>) => {
        this.loading = false;
        if (res.success && res.data) this.executions = res.data;
        else this.loadMock();
        this.applyTable();
      },
      error: () => {
        this.loading = false;
        this.loadMock();
      }
    });
  }

  loadMock(): void {
    this.automationApi.getExecutionHistoryMock().subscribe(res => {
      if (res.success && res.data) {
        this.executions = this.ruleId ? res.data.filter(e => e.ruleId === this.ruleId) : res.data;
        this.applyTable();
      }
    });
  }

  private applyTable(): void {
    const { pagedResult } = applyClientSideTable(this.executions, this.filterRequest, ['ruleName', 'status', 'message'] as (keyof AutomationExecution)[]);
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
