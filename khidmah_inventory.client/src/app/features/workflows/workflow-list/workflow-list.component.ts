import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Router } from '@angular/router';
import { WorkflowApiService } from '../../../core/services/workflow-api.service';
import { Workflow } from '../../../core/models/workflow.model';
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
  selector: 'app-workflow-list',
  standalone: true,
  imports: [CommonModule, RouterModule, ToastComponent, LoadingSpinnerComponent, UnifiedCardComponent, ContentLoaderComponent, SkeletonListingHeaderComponent, SkeletonTableComponent, DataTableComponent],
  templateUrl: './workflow-list.component.html'
})
export class WorkflowListComponent implements OnInit {
  loading = true;
  workflows: Workflow[] = [];
  pagedResult: any = null;
  showToast = false;
  toastMessage = '';
  toastType: 'success' | 'error' | 'info' = 'success';

  filterRequest: FilterRequest = {
    pagination: { pageNo: 1, pageSize: 10, sortBy: 'name', sortOrder: 'ascending' },
    search: { term: '', searchFields: ['name', 'entityType'], mode: SearchMode.Contains, isCaseSensitive: false },
    filters: []
  };

  columns: DataTableColumn<Workflow>[] = [
    { key: 'name', label: 'Name', sortable: true, searchable: true, width: '200px' },
    { key: 'entityType', label: 'Entity type', sortable: true, searchable: true, width: '120px' },
    { key: 'isActive', label: 'Status', sortable: true, type: 'badge', width: '100px', format: (v) => v ? 'Active' : 'Inactive', filterable: true, filterType: 'select', filterOptions: [{ label: 'All', value: '' }, { label: 'Active', value: 'true' }, { label: 'Inactive', value: 'false' }] },
    { key: 'createdAt', label: 'Created', sortable: true, type: 'date', width: '140px', format: (v) => v ? new Date(v).toLocaleString() : '' }
  ];

  actions: DataTableAction<Workflow>[] = [
    { label: 'View', icon: 'eye', action: (row) => this.router.navigate(['/workflows', row.id]) }
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
    searchPlaceholder: 'Search workflows...',
    emptyMessage: 'No workflows. Create one to get started.'
  };

  constructor(
    private workflowApi: WorkflowApiService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.workflowApi.getWorkflows().subscribe({
      next: (res: ApiResponse<Workflow[]>) => {
        this.loading = false;
        if (res.success && res.data) this.workflows = res.data;
        else this.workflows = [];
        this.applyTable();
      },
      error: () => { this.loading = false; this.workflows = []; this.applyTable(); }
    });
  }

  private applyTable(): void {
    const { pagedResult } = applyClientSideTable(
      this.workflows,
      this.filterRequest,
      ['name', 'entityType'] as (keyof Workflow)[],
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
}
