import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { ReorderApiService } from '../../../core/services/reorder-api.service';
import { WarehouseApiService } from '../../../core/services/warehouse-api.service';
import { ReorderSuggestion } from '../../../core/models/reorder.model';
import { ApiResponse } from '../../../core/models/api-response.model';
import { ToastComponent } from '../../../shared/components/toast/toast.component';
import { LoadingSpinnerComponent } from '../../../shared/components/loading-spinner/loading-spinner.component';
import { UnifiedCardComponent } from '../../../shared/components/unified-card/unified-card.component';
import { KpiStatCardComponent } from '../../../shared/components/kpi-stat-card/kpi-stat-card.component';
import { StatCardComponent, StatBarData } from '../../../shared/components/stat-card/stat-card.component';
import { IconComponent } from '../../../shared/components/icon/icon.component';
import { Warehouse } from '../../../core/models/warehouse.model';
import { ContentLoaderComponent } from '../../../shared/components/content-loader/content-loader.component';
import { SkeletonStatCardsComponent } from '../../../shared/components/skeleton-stat-cards/skeleton-stat-cards.component';
import { SkeletonChartComponent } from '../../../shared/components/skeleton-chart/skeleton-chart.component';
import { SkeletonTableComponent } from '../../../shared/components/skeleton-table/skeleton-table.component';
import { DataTableComponent } from '../../../shared/components/data-table/data-table.component';
import { DataTableColumn, DataTableAction, DataTableConfig } from '../../../shared/models/data-table.model';

@Component({
  selector: 'app-reorder-dashboard',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    ToastComponent,
    LoadingSpinnerComponent,
    UnifiedCardComponent,
    KpiStatCardComponent,
    StatCardComponent,
    IconComponent,
    ContentLoaderComponent,
    SkeletonStatCardsComponent,
    SkeletonChartComponent,
    SkeletonTableComponent,
    DataTableComponent
  ],
  templateUrl: './reorder-dashboard.component.html'
})
export class ReorderDashboardComponent implements OnInit {
  loading = true;
  suggestions: ReorderSuggestion[] = [];
  warehouses: Warehouse[] = [];
  selectedWarehouseId: string | null = null;
  showToast = false;
  toastMessage = '';
  toastType: 'success' | 'error' | 'info' = 'success';

  // Chart data
  itemsByPriority: StatBarData[] = [];
  daysRemainingData: StatBarData[] = [];

  get previewSuggestions(): ReorderSuggestion[] {
    return this.suggestions.slice(0, 10);
  }

  tableColumns: DataTableColumn<ReorderSuggestion>[] = [
    { key: 'productName', label: 'Product', sortable: false, width: '200px', render: (s) => `${s.productName} (${s.productSKU})` },
    { key: 'warehouseName', label: 'Warehouse', sortable: false, width: '120px' },
    { key: 'currentStock', label: 'Current', sortable: false, width: '80px', type: 'number' },
    { key: 'reorderPoint', label: 'Reorder at', sortable: false, width: '100px', format: (v) => v ?? '-' },
    { key: 'suggestedQuantity', label: 'Suggested qty', sortable: false, width: '100px', type: 'number' },
    { key: 'daysOfStockRemaining', label: 'Days left', sortable: false, width: '80px', type: 'number' },
    { key: 'priority', label: 'Priority', sortable: false, type: 'badge', width: '90px' }
  ];

  tableActions: DataTableAction<ReorderSuggestion>[] = [
    { label: 'Review', icon: 'eye', action: (row) => this.router.navigate(['/reorder/review'], { queryParams: { productId: row.productId, warehouseId: row.warehouseId } }) }
  ];

  tableConfig: DataTableConfig = {
    showSearch: false,
    showFilters: false,
    showColumnToggle: false,
    showPagination: false,
    showActions: true,
    showCheckbox: false,
    emptyMessage: 'No reorder suggestions. Stock levels are healthy.'
  };

  constructor(
    private reorderApi: ReorderApiService,
    private warehouseApi: WarehouseApiService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadWarehouses();
    this.loadSuggestions();
  }

  loadWarehouses(): void {
    this.warehouseApi.getWarehouses({ filterRequest: { pagination: { pageNo: 1, pageSize: 100 } } }).subscribe({
      next: (res: ApiResponse<any>) => {
        if (res.success && res.data?.items) this.warehouses = res.data.items;
      },
      error: () => {}
    });
  }

  loadSuggestions(): void {
    this.loading = true;
    const query: any = { includeInStock: false };
    if (this.selectedWarehouseId) query.warehouseId = this.selectedWarehouseId;
    this.reorderApi.getSuggestions(query).subscribe({
      next: (res: ApiResponse<ReorderSuggestion[]>) => {
        this.loading = false;
        if (res.success && res.data) {
          this.suggestions = res.data;
          this.buildChartData();
        } else {
          this.suggestions = [];
          this.showToastMsg(res.message || 'Failed to load suggestions', 'error');
        }
      },
      error: (err) => {
        this.loading = false;
        this.suggestions = [];
        this.showToastMsg(err.error?.message || 'Error loading suggestions', 'error');
      }
    });
  }

  buildChartData(): void {
    const priorityCounts: Record<string, number> = { Critical: 0, High: 0, Medium: 0, Low: 0 };
    this.suggestions.forEach(s => {
      priorityCounts[s.priority] = (priorityCounts[s.priority] || 0) + 1;
    });
    this.itemsByPriority = [
      { label: 'Critical', value: priorityCounts['Critical'] || 0 },
      { label: 'High', value: priorityCounts['High'] || 0 },
      { label: 'Medium', value: priorityCounts['Medium'] || 0 },
      { label: 'Low', value: priorityCounts['Low'] || 0 }
    ];
    this.daysRemainingData = this.suggestions
      .slice(0, 10)
      .map(s => ({ label: s.productName.length > 15 ? s.productName.substring(0, 15) + '…' : s.productName, value: s.daysOfStockRemaining }));
  }

  onWarehouseChange(): void {
    this.loadSuggestions();
  }

  showToastMsg(msg: string, type: 'success' | 'error' | 'info'): void {
    this.toastMessage = msg;
    this.toastType = type;
    this.showToast = true;
  }

  getPriorityClass(priority: string): string {
    switch (priority) {
      case 'Critical': return 'danger';
      case 'High': return 'warning';
      case 'Medium': return 'info';
      default: return 'secondary';
    }
  }

  get criticalCount(): number {
    return this.suggestions.filter(s => s.priority === 'Critical').length;
  }

  get highCount(): number {
    return this.suggestions.filter(s => s.priority === 'High').length;
  }
}
