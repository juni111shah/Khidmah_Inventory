import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { InventoryApiService } from '../../../core/services/inventory-api.service';
import { UnifiedButtonComponent } from '../../../shared/components/unified-button/unified-button.component';
import { UnifiedCardComponent } from '../../../shared/components/unified-card/unified-card.component';
import { BadgeComponent } from '../../../shared/components/badge/badge.component';
import { AssignSerialNumbersComponent } from '../assign-serial-numbers/assign-serial-numbers.component';
import { DataTableComponent } from '../../../shared/components/data-table/data-table.component';
import { DataTableColumn, DataTableConfig } from '../../../shared/models/data-table.model';
import { FilterRequest } from '../../../core/models/user.model';
import { SearchMode } from '../../../core/models/user.model';
import { applyClientSideTable } from '../../../shared/utils/client-side-table.util';

@Component({
  selector: 'app-serial-numbers-list',
  standalone: true,
  imports: [CommonModule, UnifiedButtonComponent, UnifiedCardComponent, BadgeComponent, AssignSerialNumbersComponent, DataTableComponent],
  templateUrl: './serial-numbers-list.component.html'
})
export class SerialNumbersListComponent implements OnInit {
  serialNumbers: any[] = [];
  pagedResult: any = null;
  loading: boolean = false;
  showAssignModal: boolean = false;

  filterRequest: FilterRequest = {
    pagination: { pageNo: 1, pageSize: 10, sortBy: 'serialNumber', sortOrder: 'ascending' },
    search: { term: '', searchFields: ['serialNumber', 'productName', 'productSKU', 'warehouseName', 'status', 'batchNumber'], mode: SearchMode.Contains, isCaseSensitive: false },
    filters: []
  };

  columns: DataTableColumn<any>[] = [
    { key: 'serialNumber', label: 'Serial Number', sortable: true, searchable: true, width: '140px' },
    { key: 'productName', label: 'Product', sortable: true, searchable: true, width: '180px', render: (s) => s.productSKU ? `${s.productName} — ${s.productSKU}` : s.productName },
    { key: 'warehouseName', label: 'Warehouse', sortable: true, searchable: true, width: '120px' },
    { key: 'status', label: 'Status', sortable: true, searchable: true, type: 'badge', width: '100px', filterable: true, filterType: 'select', filterOptions: [{ label: 'All', value: '' }, { label: 'In Stock', value: 'InStock' }, { label: 'Sold', value: 'Sold' }, { label: 'Returned', value: 'Returned' }, { label: 'Damaged', value: 'Damaged' }] },
    { key: 'warrantyExpiryDate', label: 'Warranty Until', sortable: true, width: '130px', format: (v) => v ? new Date(v).toLocaleDateString(undefined, { dateStyle: 'medium' }) : 'No Warranty' },
    { key: 'batchNumber', label: 'Batch', sortable: true, searchable: true, width: '100px', format: (v) => v || 'N/A' }
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
    searchPlaceholder: 'Search serial numbers...',
    emptyMessage: 'No serial numbers found'
  };

  constructor(private inventoryApi: InventoryApiService) { }

  ngOnInit(): void {
    this.loadSerialNumbers();
  }

  loadSerialNumbers() {
    this.loading = true;
    this.inventoryApi.getSerialNumbers({}).subscribe((res: any) => {
      this.loading = false;
      if (res.success) {
        this.serialNumbers = res.data.items ?? [];
      } else {
        this.serialNumbers = [];
      }
      this.applyTable();
    });
  }

  private getCellValue(row: any, key: string): any {
    if (key === 'productName') return row.productSKU ? `${row.productName ?? ''} ${row.productSKU}` : (row.productName ?? '');
    if (key === 'warrantyExpiryDate') return row.warrantyExpiryDate ? new Date(row.warrantyExpiryDate).getTime() : 0;
    return row[key];
  }

  private applyTable(): void {
    const searchFields = ['serialNumber', 'productName', 'productSKU', 'warehouseName', 'status', 'batchNumber'] as (keyof any)[];
    const { pagedResult } = applyClientSideTable(this.serialNumbers, this.filterRequest, searchFields, (r, k) => this.getCellValue(r, k));
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

  getStatusColor(status: string): string {
    switch (status?.toLowerCase()) {
      case 'instock': return 'success';
      case 'sold': return 'primary';
      case 'returned': return 'warning';
      case 'damaged': return 'danger';
      default: return 'secondary';
    }
  }

  openCreateModal() {
     this.showAssignModal = true;
  }
}
