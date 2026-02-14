import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { InventoryApiService } from '../../../core/services/inventory-api.service';
import { UnifiedButtonComponent } from '../../../shared/components/unified-button/unified-button.component';
import { UnifiedCardComponent } from '../../../shared/components/unified-card/unified-card.component';
import { BadgeComponent } from '../../../shared/components/badge/badge.component';
import { CreateBatchComponent } from '../create-batch/create-batch.component';
import { DataTableComponent } from '../../../shared/components/data-table/data-table.component';
import { DataTableColumn, DataTableAction, DataTableConfig } from '../../../shared/models/data-table.model';
import { FilterRequest } from '../../../core/models/user.model';
import { SearchMode } from '../../../core/models/user.model';
import { applyClientSideTable } from '../../../shared/utils/client-side-table.util';

@Component({
  selector: 'app-batches-list',
  standalone: true,
  imports: [CommonModule, UnifiedButtonComponent, UnifiedCardComponent, BadgeComponent, CreateBatchComponent, DataTableComponent],
  templateUrl: './batches-list.component.html'
})
export class BatchesListComponent implements OnInit {
  batches: any[] = [];
  pagedResult: any = null;
  loading: boolean = false;
  showCreateModal: boolean = false;

  filterRequest: FilterRequest = {
    pagination: { pageNo: 1, pageSize: 10, sortBy: 'expiryDate', sortOrder: 'ascending' },
    search: { term: '', searchFields: ['batchNumber', 'lotNumber', 'productName', 'productSKU', 'status'], mode: SearchMode.Contains, isCaseSensitive: false },
    filters: []
  };

  columns: DataTableColumn<any>[] = [
    { key: 'batchNumber', label: 'Batch Number', sortable: true, searchable: true, width: '140px', render: (b) => `${b.batchNumber} — Lot: ${b.lotNumber || 'N/A'}` },
    { key: 'productName', label: 'Product', sortable: true, searchable: true, width: '180px', render: (b) => b.productSKU ? `${b.productName} — ${b.productSKU}` : b.productName },
    { key: 'totalQuantity', label: 'Quantity', sortable: true, width: '90px', type: 'number' },
    { key: 'expiryDate', label: 'Expiry Date', sortable: true, width: '130px', render: (b) => this.formatExpiry(b) },
    { key: 'status', label: 'Status', sortable: true, searchable: true, type: 'badge', width: '100px', filterable: true, filterType: 'select', filterOptions: [{ label: 'All', value: '' }, { label: 'In Stock', value: 'InStock' }, { label: 'Recalled', value: 'Recalled' }, { label: 'Expired', value: 'Expired' }] },
    { key: 'isRecalled', label: 'Recall', sortable: true, width: '90px', format: (v) => v ? 'RECALLED' : 'Active', filterable: true, filterType: 'select', filterOptions: [{ label: 'All', value: '' }, { label: 'Active', value: 'false' }, { label: 'Recalled', value: 'true' }] }
  ];

  actions: DataTableAction<any>[] = [
    { label: 'Recall', icon: 'exclamation-triangle', class: 'danger', condition: (b) => !b.isRecalled, action: (row) => this.recallBatch(row) }
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
    searchPlaceholder: 'Search batches...',
    emptyMessage: 'No batches found'
  };

  constructor(private inventoryApi: InventoryApiService) { }

  formatExpiry(b: any): string {
    if (!b.expiryDate) return '';
    const d = new Date(b.expiryDate).toLocaleDateString(undefined, { dateStyle: 'medium' });
    if (this.isExpired(b.expiryDate)) return d + ' (Expired)';
    if (this.isExpiringSoon(b.expiryDate)) return d + ' (Expiring soon)';
    return d;
  }

  ngOnInit(): void {
    this.loadBatches();
  }

  loadBatches() {
    this.loading = true;
    this.inventoryApi.getBatches({}).subscribe((res: any) => {
      this.loading = false;
      if (res.success) {
        this.batches = res.data.items ?? [];
      } else {
        this.batches = [];
      }
      this.applyTable();
    });
  }

  private getCellValue(row: any, key: string): any {
    if (key === 'batchNumber') return `${row.batchNumber || ''} ${row.lotNumber || ''}`.trim();
    if (key === 'productName') return row.productSKU ? `${row.productName || ''} ${row.productSKU || ''}` : (row.productName || '');
    if (key === 'expiryDate') return row.expiryDate ? new Date(row.expiryDate).getTime() : 0;
    if (key === 'isRecalled') return row.isRecalled === true;
    return row[key];
  }

  private applyTable(): void {
    const searchFields = ['batchNumber', 'lotNumber', 'productName', 'productSKU', 'status'] as (keyof any)[];
    const { pagedResult } = applyClientSideTable(this.batches, this.filterRequest, searchFields, (r, k) => this.getCellValue(r, k));
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

  isExpired(date: string): boolean {
    if (!date) return false;
    return new Date(date) < new Date();
  }

  isExpiringSoon(date: string): boolean {
    if (!date) return false;
    const expiry = new Date(date);
    const today = new Date();
    const diff = expiry.getTime() - today.getTime();
    const days = diff / (1000 * 3600 * 24);
    return days > 0 && days < 30;
  }

  getStatusColor(status: string): string {
    switch (status?.toLowerCase()) {
      case 'instock': return 'success';
      case 'recalled': return 'danger';
      case 'expired': return 'warning';
      default: return 'primary';
    }
  }

  recallBatch(batch: any) {
    if (confirm(`Are you sure you want to recall batch ${batch.batchNumber}? This will mark all items as recalled.`)) {
      this.inventoryApi.recallBatch(batch.id, { reason: 'Manual recall' }).subscribe((res: any) => {
        if (res.success) {
          this.loadBatches();
        }
      });
    }
  }

  openCreateModal() {
    this.showCreateModal = true;
  }
}
