import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AnalyticsApiService } from '../../../core/services/analytics-api.service';
import { SignalRService } from '../../../core/services/signalr.service';
import { SalesAnalytics, TimeRangeType, AnalyticsRequest, AnalyticsType, ProductAnalytics, CustomerAnalytics } from '../../../core/models/analytics.model';
import { ChartComponent, ChartData, ChartDataset } from '../../../shared/components/chart/chart.component';
import { TimeRangeFilterComponent } from '../../../shared/components/time-range-filter/time-range-filter.component';
import { ToastComponent } from '../../../shared/components/toast/toast.component';
import { LoadingSpinnerComponent } from '../../../shared/components/loading-spinner/loading-spinner.component';
import { DataTableComponent } from '../../../shared/components/data-table/data-table.component';
import { DataTableColumn, DataTableConfig } from '../../../shared/models/data-table.model';
import { Subscription } from 'rxjs';
import { UnifiedCardComponent } from '../../../shared/components/unified-card/unified-card.component';
import { HeaderService } from '../../../core/services/header.service';

@Component({
  selector: 'app-sales-analytics',
  standalone: true,
  imports: [
    CommonModule,
    ChartComponent,
    TimeRangeFilterComponent,
    ToastComponent,
    LoadingSpinnerComponent,
    DataTableComponent,
    UnifiedCardComponent
  ],
  templateUrl: './sales-analytics.component.html'
})
export class SalesAnalyticsComponent implements OnInit, OnDestroy {
  analytics: SalesAnalytics | null = null;
  loading = false;
  selectedRange: TimeRangeType = TimeRangeType.Last30Days;
  customFromDate?: string;
  customToDate?: string;

  salesChartData: ChartData | null = null;
  categoryChartData: ChartData | null = null;

  // Top Products Table
  topProductsColumns: DataTableColumn<ProductAnalytics>[] = [
    { key: 'productName', label: 'Product', sortable: true, width: '200px' },
    { key: 'productSKU', label: 'SKU', sortable: true, width: '120px' },
    { 
      key: 'quantitySold', 
      label: 'Quantity Sold', 
      sortable: true, 
      type: 'number',
      width: '120px',
      format: (value) => this.formatNumber(value)
    },
    { 
      key: 'totalSales', 
      label: 'Total Sales', 
      sortable: true, 
      type: 'number',
      width: '120px',
      format: (value) => this.formatCurrency(value)
    },
    { 
      key: 'totalCost', 
      label: 'Total Cost', 
      sortable: true, 
      type: 'number',
      width: '120px',
      format: (value) => this.formatCurrency(value)
    },
    { 
      key: 'totalProfit', 
      label: 'Profit', 
      sortable: true, 
      type: 'number',
      width: '120px',
      format: (value) => this.formatCurrency(value),
      render: (row) => {
        const profit = this.formatCurrency(row.totalProfit);
        return profit;
      }
    }
  ];

  topProductsConfig: DataTableConfig = {
    showSearch: false,
    showFilters: false,
    showColumnToggle: false,
    showPagination: false,
    showActions: false,
    showCheckbox: false
  };

  // Top Customers Table
  topCustomersColumns: DataTableColumn<CustomerAnalytics>[] = [
    { key: 'customerName', label: 'Customer', sortable: true, width: '200px' },
    { 
      key: 'orderCount', 
      label: 'Orders', 
      sortable: true, 
      type: 'number',
      width: '100px',
      format: (value) => this.formatNumber(value)
    },
    { 
      key: 'totalSales', 
      label: 'Total Sales', 
      sortable: true, 
      type: 'number',
      width: '120px',
      format: (value) => this.formatCurrency(value)
    },
    { 
      key: 'averageOrderValue', 
      label: 'Avg Order Value', 
      sortable: true, 
      type: 'number',
      width: '140px',
      format: (value) => this.formatCurrency(value)
    },
    { 
      key: 'percentage', 
      label: 'Percentage', 
      sortable: true, 
      type: 'number',
      width: '100px',
      format: (value) => this.formatPercent(value)
    }
  ];

  topCustomersConfig: DataTableConfig = {
    showSearch: false,
    showFilters: false,
    showColumnToggle: false,
    showPagination: false,
    showActions: false,
    showCheckbox: false
  };

  private subscriptions = new Subscription();

  showToast = false;
  toastMessage = '';
  toastType: 'success' | 'error' | 'warning' | 'info' = 'success';

  constructor(
    private analyticsApiService: AnalyticsApiService,
    private signalRService: SignalRService,
    private headerService: HeaderService
  ) {}

  async ngOnInit(): Promise<void> {
    this.headerService.setHeaderInfo({
      title: 'Sales Analytics',
      description: 'Track performance metrics and sales trends'
    });
    await this.signalRService.startConnection();
    this.loadAnalytics();
    this.setupRealTimeUpdates();
  }

  ngOnDestroy(): void {
    this.subscriptions.unsubscribe();
    this.signalRService.offAnalyticsUpdate(this.onAnalyticsUpdate);
  }

  private setupRealTimeUpdates(): void {
    this.signalRService.subscribeToAnalytics('Sales');
    this.signalRService.onAnalyticsUpdate(this.onAnalyticsUpdate);
  }

  private onAnalyticsUpdate = (analyticsType: string, data: any): void => {
    if (analyticsType === 'Sales' && data) {
      this.analytics = data;
      this.updateCharts();
      this.showToastMessage('info', 'Analytics updated in real-time');
    }
  };

  onRangeChange(event: { range: TimeRangeType; fromDate?: string; toDate?: string }): void {
    this.selectedRange = event.range;
    this.customFromDate = event.fromDate;
    this.customToDate = event.toDate;
    this.loadAnalytics();
  }

  loadAnalytics(): void {
    this.loading = true;
    const request: AnalyticsRequest = {
      timeRange: this.selectedRange,
      customFromDate: this.customFromDate,
      customToDate: this.customToDate,
      analyticsType: AnalyticsType.Sales,
      groupBy: 'Day'
    };

    this.analyticsApiService.getSalesAnalytics(request).subscribe({
      next: (response) => {
        if (response.success && response.data) {
          this.analytics = response.data;
          this.updateCharts();
        } else {
          this.showToastMessage('error', response.message || 'Failed to load analytics');
        }
        this.loading = false;
      },
      error: () => {
        this.showToastMessage('error', 'Error loading analytics');
        this.loading = false;
      }
    });
  }

  private updateCharts(): void {
    if (!this.analytics) return;

    // Sales time series chart
    this.salesChartData = {
      labels: this.analytics.timeSeriesData.map(d => d.label),
      datasets: [
        {
          label: 'Sales',
          data: this.analytics.timeSeriesData.map(d => d.value),
          backgroundColor: 'rgba(54, 162, 235, 0.2)',
          borderColor: 'rgba(54, 162, 235, 1)',
          borderWidth: 2,
          fill: true,
          tension: 0.4
        }
      ]
    };

    // Category breakdown chart
    this.categoryChartData = {
      labels: this.analytics.categoryBreakdown.map(c => c.categoryName),
      datasets: [
        {
          label: 'Sales by Category',
          data: this.analytics.categoryBreakdown.map(c => c.totalSales),
          backgroundColor: [
            'rgba(255, 99, 132, 0.2)',
            'rgba(54, 162, 235, 0.2)',
            'rgba(255, 206, 86, 0.2)',
            'rgba(75, 192, 192, 0.2)',
            'rgba(153, 102, 255, 0.2)',
            'rgba(255, 159, 64, 0.2)'
          ],
          borderColor: [
            'rgba(255, 99, 132, 1)',
            'rgba(54, 162, 235, 1)',
            'rgba(255, 206, 86, 1)',
            'rgba(75, 192, 192, 1)',
            'rgba(153, 102, 255, 1)',
            'rgba(255, 159, 64, 1)'
          ],
          borderWidth: 2
        }
      ]
    };
  }

  formatCurrency(value: number): string {
    return new Intl.NumberFormat('en-US', { style: 'currency', currency: 'USD' }).format(value);
  }

  formatNumber(value: number): string {
    return new Intl.NumberFormat('en-US').format(value);
  }

  formatPercent(value: number): string {
    return `${value.toFixed(2)}%`;
  }

  showToastMessage(type: 'success' | 'error' | 'warning' | 'info', message: string): void {
    this.toastType = type;
    this.toastMessage = message;
    this.showToast = true;
    setTimeout(() => { this.showToast = false; }, 3000);
  }

  get chartOptions(): any {
    return {
      scales: {
        y: {
          beginAtZero: true,
          ticks: {
            callback: (value: any) => {
              return '$' + value.toLocaleString();
            }
          }
        }
      }
    };
  }
}


