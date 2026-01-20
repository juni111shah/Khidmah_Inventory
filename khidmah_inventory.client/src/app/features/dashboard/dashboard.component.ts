import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { DashboardApiService } from '../../core/services/dashboard-api.service';
import { SignalRService } from '../../core/services/signalr.service';
import { Dashboard } from '../../core/models/dashboard.model';
import { ToastComponent } from '../../shared/components/toast/toast.component';
import { LoadingSpinnerComponent } from '../../shared/components/loading-spinner/loading-spinner.component';
import { IconComponent } from '../../shared/components/icon/icon.component';
import { HasPermissionDirective } from '../../shared/directives/has-permission.directive';
import { ChartComponent, ChartData } from '../../shared/components/chart/chart.component';
import { StatCardComponent, StatBarData, TimeFrame } from '../../shared/components/stat-card/stat-card.component';
import { SparklineChartComponent } from '../../shared/components/sparkline-chart/sparkline-chart.component';
import { Subscription } from 'rxjs';
import { UnifiedCardComponent } from '../../shared/components/unified-card/unified-card.component';
import { HeaderService } from '../../core/services/header.service';
import { NgApexchartsModule } from 'ng-apexcharts';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [
    CommonModule,
    ToastComponent,
    LoadingSpinnerComponent,
    IconComponent,
    HasPermissionDirective,
    ChartComponent,
    StatCardComponent,
    SparklineChartComponent,
    UnifiedCardComponent,
    NgApexchartsModule
  ],
  templateUrl: './dashboard.component.html',
  styles: [`
    /* Override timeframe navigation for dashboard charts */
    .timeframe-nav {
      align-self: flex-start;
    }

    /* Stat card header styles for dashboard cards */
    .stat-card-header {
      display: flex;
      justify-content: space-between;
      align-items: flex-start;
      margin-bottom: 1.5rem;
    }

    .stat-title {
      font-size: 1.1rem;
      font-weight: 700;
      margin-bottom: 0.25rem;
    }

    .stat-source {
      font-size: 0.8rem;
      color: var(--text-secondary-color);
      margin-bottom: 0;
    }

    .title-icon {
      margin-left: 0.5rem;
      font-size: 1.1rem;
    }

    .header-left {
      flex: 1;
    }

    .header-right {
      /* For future menu buttons */
    }
  `]
})
export class DashboardComponent implements OnInit, OnDestroy {
  dashboard: Dashboard | null = null;
  loading = false;
  showToast = false;
  toastMessage = '';
  toastType: 'success' | 'error' | 'warning' | 'info' = 'success';
  salesChartData: ChartData | null = null;
  inventoryChartData: ChartData | null = null;
  warehouseChartData: ChartData | null = null;
  unifiedStockChartData: ChartData | null = null;
  selectedStockTab: 'warehouse' | 'category' = 'warehouse';
  selectedSalesPeriod: 7 | 15 | 30 = 30;
  statCardData: StatBarData[] = [];
  selectedTimeFrame: TimeFrame = 'month';
  
  // Sparkline data for summary cards
  totalProductsTrend: number[] = [];
  warehousesTrend: number[] = [];
  stockValueTrend: number[] = [];
  lowStockTrend: number[] = [];
  todaySalesTrend: number[] = [];
  todayPurchasesTrend: number[] = [];
  pendingPOTrend: number[] = [];
  pendingSOTrend: number[] = [];
  
  private subscriptions = new Subscription();

  constructor(
    private dashboardApiService: DashboardApiService,
    private signalRService: SignalRService,
    public router: Router,
    private headerService: HeaderService
  ) {}

  ngOnInit(): void {
    this.headerService.setHeaderInfo({
      title: 'Dashboard',
      description: 'System overview and performance metrics'
    });
    // Start SignalR connection in background (non-blocking)
    this.signalRService.startConnection().catch(error => {
      console.warn('SignalR connection failed, continuing without real-time updates:', error);
    });
    // Initialize with sample data
    this.updateStatCardData();
    this.generateTrendData();
    this.updateCharts();
    this.updateInventoryChart();
    this.updateWarehouseChart();
    this.updateUnifiedStockChart();
    this.loadDashboard();
    this.setupRealTimeUpdates();
  }

  ngOnDestroy(): void {
    this.subscriptions.unsubscribe();
    this.signalRService.offDashboardUpdate(this.onDashboardUpdate);
  }

  private setupRealTimeUpdates(): void {
    this.signalRService.onDashboardUpdate(this.onDashboardUpdate);
  }

  private onDashboardUpdate = (data: Dashboard): void => {
    this.dashboard = data;
    this.updateCharts();
    this.updateInventoryChart();
          this.updateWarehouseChart();
          this.updateUnifiedStockChart();
          this.showToastMessage('info', 'Dashboard updated in real-time');
  };

  loadDashboard(): void {
    this.loading = true;
    this.dashboardApiService.getDashboardData().subscribe({
      next: (response) => {
        if (response.success && response.data) {
          this.dashboard = response.data;
          this.updateCharts();
          this.updateInventoryChart();
          this.updateWarehouseChart();
          this.updateUnifiedStockChart();
          this.updateStatCardData();
          this.generateTrendData();
        } else {
          this.showToastMessage('error', response.message || 'Failed to load dashboard data');
        }
        this.loading = false;
      },
      error: (error) => {
        this.showToastMessage('error', 'Error loading dashboard data');
        this.loading = false;
      }
    });
  }

  private updateCharts(): void {
    console.log('updateCharts called', {
      hasDashboard: !!this.dashboard,
      hasSalesData: !!(this.dashboard?.salesChartData),
      salesDataLength: this.dashboard?.salesChartData?.length || 0
    });

    // Always try to create sales data - either from API or sample data
    if (!this.dashboard || !this.dashboard.salesChartData || this.dashboard.salesChartData.length === 0) {
      // Create sample sales data when no real data is available
      this.createSampleSalesData();
      return;
    }

    // Process real sales data from dashboard
    try {
      const formatDate = (dateStr: string): string => {
        if (!dateStr) return '';
        try {
          const date = new Date(dateStr);
          return date.toLocaleDateString('en-US', { month: 'short', day: 'numeric' });
        } catch {
          return dateStr;
        }
      };

      // Filter data based on selected time period
      const cutoffDate = new Date();
      cutoffDate.setDate(cutoffDate.getDate() - this.selectedSalesPeriod);

      const filteredData = this.dashboard!.salesChartData.filter(item => {
        if (!item.date) return false;
        const itemDate = new Date(item.date);
        return itemDate >= cutoffDate;
      });

      this.salesChartData = {
        labels: filteredData.map(d => formatDate(d.date || '')),
        datasets: [
          {
            label: 'Sales',
            data: filteredData.map(d => Number(d.sales) || 0),
            backgroundColor: 'rgba(54, 162, 235, 0.2)',
            borderColor: 'rgba(54, 162, 235, 1)',
            borderWidth: 2,
            fill: true,
            tension: 0.4
          },
          {
            label: 'Purchases',
            data: filteredData.map(d => Number(d.purchases) || 0),
            backgroundColor: 'rgba(255, 99, 132, 0.2)',
            borderColor: 'rgba(255, 99, 132, 1)',
            borderWidth: 2,
            fill: true,
            tension: 0.4
          }
        ]
      };
      console.log(`Created sales chart data from API (${this.selectedSalesPeriod} days)`, this.salesChartData);
    } catch (error) {
      console.error('Error creating sales chart:', error);
      this.createSampleSalesData();
    }
  }

  private createSampleSalesData(): void {
    // Create sample sales data for the selected time period
    const labels: string[] = [];
    const salesData: number[] = [];
    const purchasesData: number[] = [];

    const today = new Date();
    for (let i = this.selectedSalesPeriod - 1; i >= 0; i--) {
      const date = new Date(today);
      date.setDate(today.getDate() - i);

      labels.push(date.toLocaleDateString('en-US', { month: 'short', day: 'numeric' }));

      // Generate sample data with some variation
      const baseSales = 8000 + Math.random() * 4000;
      const basePurchases = 6000 + Math.random() * 3000;

      salesData.push(Math.round(baseSales + (Math.random() - 0.5) * 2000));
      purchasesData.push(Math.round(basePurchases + (Math.random() - 0.5) * 1500));
    }

    this.salesChartData = {
      labels: labels,
      datasets: [
        {
          label: 'Sales',
          data: salesData,
          backgroundColor: 'rgba(54, 162, 235, 0.2)',
          borderColor: 'rgba(54, 162, 235, 1)',
          borderWidth: 2,
          fill: true,
          tension: 0.4
        },
        {
          label: 'Purchases',
          data: purchasesData,
          backgroundColor: 'rgba(255, 99, 132, 0.2)',
          borderColor: 'rgba(255, 99, 132, 1)',
          borderWidth: 2,
          fill: true,
          tension: 0.4
        }
      ]
    };
    console.log('Created sample sales chart data', this.salesChartData);

    // Create inventory chart (pie chart showing stock value by category)
    this.updateInventoryChart();

    // Create warehouse donut chart
    this.updateWarehouseChart();
  }

  private updateInventoryChart(): void {
    console.log('updateInventoryChart called', {
      hasDashboard: !!this.dashboard,
      hasInventoryData: !!(this.dashboard?.inventoryChartData),
      inventoryDataLength: this.dashboard?.inventoryChartData?.length || 0
    });

    // Always create sample inventory data for testing
    this.inventoryChartData = {
      labels: ['Electronics', 'Clothing', 'Home & Garden'],
      datasets: [
        {
          label: 'Stock Value by Category',
          data: [25000, 18000, 15000],
          backgroundColor: ['#FF6384', '#36A2EB', '#FFCE56'],
          borderColor: ['#FF6384', '#36A2EB', '#FFCE56'],
          borderWidth: 2,
          fill: false
        }
      ]
    };
    console.log('Created sample inventory chart data', this.inventoryChartData);
    return;

    if (!this.dashboard!.inventoryChartData || this.dashboard!.inventoryChartData.length === 0) {
      // Create sample data based on dashboard summary
      const categories = ['Electronics', 'Clothing', 'Home & Garden', 'Sports', 'Books'];
      const totalValue = this.dashboard!.summary.totalStockValue || 100000;
      const data = categories.map((_, index) =>
        Math.round(totalValue * (0.3 - index * 0.05)) // Decreasing values
      );

      this.inventoryChartData = {
        labels: categories,
        datasets: [
          {
            label: 'Stock Value by Category',
            data: data,
            backgroundColor: [
              '#FF6384', '#36A2EB', '#FFCE56', '#4BC0C0',
              '#9966FF', '#FF9F40', '#8AC249', '#EA80FC'
            ].slice(0, categories.length),
            borderColor: [
              '#FF6384', '#36A2EB', '#FFCE56', '#4BC0C0',
              '#9966FF', '#FF9F40', '#8AC249', '#EA80FC'
            ].slice(0, categories.length),
            borderWidth: 2,
            fill: false
          }
        ]
      };
      console.log('Created sample inventory chart data from dashboard summary');
      return;
    }

    try {
      // Filter out categories with zero stock value and limit to top 8 categories
      const filteredData = this.dashboard!.inventoryChartData
        .filter(item => item.stockValue > 0)
        .sort((a, b) => b.stockValue - a.stockValue)
        .slice(0, 8);

      if (filteredData.length === 0) {
        this.inventoryChartData = null;
        console.log('No valid inventory data after filtering');
        return;
      }

      this.inventoryChartData = {
        labels: filteredData.map(d => d.categoryName),
        datasets: [
          {
            label: 'Stock Value by Category',
            data: filteredData.map(d => Number(d.stockValue) || 0),
            backgroundColor: [
              '#FF6384', '#36A2EB', '#FFCE56', '#4BC0C0',
              '#9966FF', '#FF9F40', '#8AC249', '#EA80FC'
            ],
            borderColor: [
              '#FF6384', '#36A2EB', '#FFCE56', '#4BC0C0',
              '#9966FF', '#FF9F40', '#8AC249', '#EA80FC'
            ],
            borderWidth: 2,
            fill: false
          }
        ]
      };
      console.log('Created inventory chart data from API', this.inventoryChartData);
    } catch (error) {
      console.error('Error creating inventory chart:', error);
      this.inventoryChartData = null;
    }
  }


  private updateWarehouseChart(): void {
    console.log('updateWarehouseChart called', {
      hasDashboard: !!this.dashboard,
      totalWarehouses: this.dashboard?.summary?.totalWarehouses || 0
    });

    // Always create sample warehouse distribution data for testing
    this.warehouseChartData = {
      labels: ['Main Warehouse', 'Secondary Warehouse', 'Distribution Center'],
      datasets: [
        {
          label: 'Stock Distribution',
          data: [45, 25, 20],
          backgroundColor: ['#FF6384', '#36A2EB', '#FFCE56'],
          borderColor: ['#FF6384', '#36A2EB', '#FFCE56'],
          borderWidth: 2,
          fill: false
        }
      ]
    };
    console.log('Created sample warehouse chart data', this.warehouseChartData);
    return;

    try {
      // Create sample distribution data
      const warehouseCount = Math.max(this.dashboard!.summary.totalWarehouses || 3, 1);
      const sampleLabels = ['Main Warehouse', 'Secondary Warehouse', 'Distribution Center', 'Retail Store', 'Cold Storage'];
      const labels = sampleLabels.slice(0, Math.min(warehouseCount, sampleLabels.length));

      // Distribute the total stock value across warehouses
      const totalValue = this.dashboard!.summary.totalStockValue || 10000;
      const baseValues = [0.4, 0.25, 0.2, 0.1, 0.05]; // Distribution percentages
      const data = baseValues.slice(0, labels.length).map(percent => Math.round(totalValue * percent));

      this.warehouseChartData = {
        labels: labels,
        datasets: [
          {
            label: 'Stock Value Distribution',
            data: data,
            backgroundColor: [
              '#FF6384', '#36A2EB', '#FFCE56', '#4BC0C0', '#9966FF'
            ].slice(0, labels.length),
            borderColor: [
              '#FF6384', '#36A2EB', '#FFCE56', '#4BC0C0', '#9966FF'
            ].slice(0, labels.length),
            borderWidth: 2,
            fill: false
          }
        ]
      };
      console.log('Created warehouse chart data', this.warehouseChartData);
    } catch (error) {
      console.error('Error creating warehouse chart:', error);
      // Fallback to basic sample data
      this.warehouseChartData = {
        labels: ['Warehouse A', 'Warehouse B', 'Warehouse C'],
        datasets: [
          {
            label: 'Stock Distribution',
            data: [40, 35, 25],
            backgroundColor: ['#FF6384', '#36A2EB', '#FFCE56'],
            borderColor: ['#FF6384', '#36A2EB', '#FFCE56'],
            borderWidth: 2,
            fill: false
          }
        ]
      };
      console.log('Created fallback warehouse chart data');
    }
  }

  private updateUnifiedStockChart(): void {
    if (this.selectedStockTab === 'warehouse') {
      // Use warehouse data for bar chart
      if (!this.dashboard) {
        this.unifiedStockChartData = {
          labels: ['Main Warehouse', 'Secondary Warehouse', 'Distribution Center', 'Retail Store'],
          datasets: [{
            label: 'Stock Value ($)',
            data: [45000, 25000, 20000, 10000],
            backgroundColor: '#0d6efd',
            borderColor: '#0d6efd',
            borderWidth: 0
          }]
        };
      } else {
        const warehouseCount = Math.max(this.dashboard!.summary.totalWarehouses || 3, 1);
        const sampleLabels = ['Main Warehouse', 'Secondary Warehouse', 'Distribution Center', 'Retail Store', 'Cold Storage'];
        const labels = sampleLabels.slice(0, Math.min(warehouseCount, sampleLabels.length));

        const totalValue = this.dashboard!.summary.totalStockValue || 10000;
        const baseValues = [0.4, 0.25, 0.2, 0.1, 0.05];
        const data = baseValues.slice(0, labels.length).map(percent => Math.round(totalValue * percent));

        this.unifiedStockChartData = {
          labels: labels,
          datasets: [{
            label: 'Stock Value ($)',
            data: data,
            backgroundColor: '#0d6efd',
            borderColor: '#0d6efd',
            borderWidth: 0
          }]
        };
      }
    } else if (this.selectedStockTab === 'category') {
      // Use category data for bar chart
      if (!this.dashboard || !this.dashboard.inventoryChartData || this.dashboard.inventoryChartData.length === 0) {
        // Sample category data
        const categories = ['Electronics', 'Clothing', 'Home & Garden', 'Sports', 'Books'];
        const totalValue = 100000;
        const data = categories.map((_, index) =>
          Math.round(totalValue * (0.3 - index * 0.05))
        );

        this.unifiedStockChartData = {
          labels: categories,
          datasets: [{
            label: 'Stock Value ($)',
            data: data,
            backgroundColor: '#0d6efd',
            borderColor: '#0d6efd',
            borderWidth: 0
          }]
        };
      } else {
        // Real category data
        const filteredData = this.dashboard!.inventoryChartData
          .filter(item => item.stockValue > 0)
          .sort((a, b) => b.stockValue - a.stockValue)
          .slice(0, 8);

        this.unifiedStockChartData = {
          labels: filteredData.map(d => d.categoryName),
          datasets: [{
            label: 'Stock Value ($)',
            data: filteredData.map(d => Number(d.stockValue) || 0),
            backgroundColor: '#0d6efd',
            borderColor: '#0d6efd',
            borderWidth: 0
          }]
        };
      }
    }
    console.log('Updated unified stock chart for tab:', this.selectedStockTab, this.unifiedStockChartData);
  }

  onStockTabChange(tab: 'warehouse' | 'category'): void {
    this.selectedStockTab = tab;
    this.updateUnifiedStockChart();
  }

  onSalesPeriodChange(period: 7 | 15 | 30): void {
    this.selectedSalesPeriod = period;
    this.updateCharts();
  }

  get salesChartTitle(): string {
    return `Sales & Purchases (Last ${this.selectedSalesPeriod} Days)`;
  }

  showToastMessage(type: 'success' | 'error' | 'warning' | 'info', message: string): void {
    this.toastType = type;
    this.toastMessage = message;
    this.showToast = true;
    setTimeout(() => { this.showToast = false; }, 3000);
  }

  formatCurrency(value: number): string {
    return new Intl.NumberFormat('en-US', { style: 'currency', currency: 'USD' }).format(value);
  }

  formatNumber(value: number): string {
    return new Intl.NumberFormat('en-US').format(value);
  }

  chartOptions: any = {
    scales: {
      y: {
        beginAtZero: true,
        ticks: {
          callback: (value: any) => {
            return '$' + value.toLocaleString();
          }
        }
      }
    },
    plugins: {
      legend: {
        display: true,
        position: 'top'
      },
      tooltip: {
        mode: 'index',
        intersect: false,
        callbacks: {
          label: (context: any) => {
            let label = context.dataset.label || '';
            if (label) {
              label += ': ';
            }
            if (context.parsed.y !== null) {
              label += '$' + context.parsed.y.toLocaleString();
            }
            return label;
          }
        }
      }
    },
    animation: {
      duration: 1000,
      easing: 'easeInOutQuart'
    },
    responsive: true,
    maintainAspectRatio: false
  };

  pieChartOptions: any = {
    plugins: {
      legend: {
        position: 'bottom',
        labels: {
          usePointStyle: true,
          padding: 20
        }
      },
      tooltip: {
        callbacks: {
          label: (context: any) => {
            const label = context.label || '';
            const value = context.parsed || 0;
            const total = context.dataset.data.reduce((a: number, b: number) => a + b, 0);
            const percentage = total > 0 ? ((value / total) * 100).toFixed(1) : '0';
            return `${label}: $${value.toLocaleString()} (${percentage}%)`;
          }
        }
      }
    },
    animation: {
      animateScale: true,
      animateRotate: true,
      duration: 1500,
      easing: 'easeInOutQuart'
    },
    responsive: true,
    maintainAspectRatio: false
  };

  barChartOptions: any = {
    scales: {
      y: {
        beginAtZero: true,
        ticks: {
          callback: (value: any) => {
            return '$' + value.toLocaleString();
          }
        }
      }
    },
    plugins: {
      legend: {
        display: true,
        position: 'top'
      },
      tooltip: {
        mode: 'index',
        intersect: false,
        callbacks: {
          label: (context: any) => {
            let label = context.dataset.label || '';
            if (label) {
              label += ': ';
            }
            if (context.parsed.y !== null) {
              label += '$' + context.parsed.y.toLocaleString();
            }
            return label;
          }
        }
      }
    },
    animation: {
      duration: 1200,
      easing: 'easeInOutQuart',
      delay: (context: any) => context.dataIndex * 100
    },
    responsive: true,
    maintainAspectRatio: false
  };

  navigateToOrder(order: any): void {
    if (order.type === 'Purchase') {
      this.router.navigate(['/purchase-orders', order.id]);
    } else {
      this.router.navigate(['/sales-orders', order.id]);
    }
  }

  private updateStatCardData(): void {
    if (!this.dashboard || !this.dashboard.salesChartData || this.dashboard.salesChartData.length === 0) {
      this.statCardData = [
        { label: '1-7', value: 75, average: 91 },
        { label: '8-14', value: 95 },
        { label: '15-21', value: 70 },
        { label: '22-28', value: 100 },
        { label: '29-30', value: 60, average: 91 }
      ];
      return;
    }

    const now = new Date();
    const currentMonth = now.getMonth();
    const currentYear = now.getFullYear();
    
    const monthlyData = this.dashboard.salesChartData.filter(d => {
      if (!d.date) return false;
      const date = new Date(d.date);
      return date.getMonth() === currentMonth && date.getFullYear() === currentYear;
    });

    if (monthlyData.length === 0) {
      this.statCardData = [
        { label: '1-7', value: 75, average: 91 },
        { label: '8-14', value: 95 },
        { label: '15-21', value: 70 },
        { label: '22-28', value: 100 },
        { label: '29-30', value: 60, average: 91 }
      ];
      return;
    }

    const weeks: { [key: string]: number[] } = {};
    monthlyData.forEach(d => {
      if (!d.date) return;
      const date = new Date(d.date);
      const day = date.getDate();
      let weekKey = '';
      
      if (day <= 7) weekKey = '1-7';
      else if (day <= 14) weekKey = '8-14';
      else if (day <= 21) weekKey = '15-21';
      else if (day <= 28) weekKey = '22-28';
      else weekKey = '29-30';

      if (!weeks[weekKey]) weeks[weekKey] = [];
      weeks[weekKey].push(Number(d.sales) || 0);
    });

    this.statCardData = Object.keys(weeks).map((key, index, arr) => {
      const values = weeks[key];
      const avg = Math.round(values.reduce((a, b) => a + b, 0) / values.length);
      return {
        label: key,
        value: avg,
        average: index === arr.length - 1 ? Math.round(
          Object.values(weeks).flat().reduce((a, b) => a + b, 0) / 
          Object.values(weeks).flat().length
        ) : undefined
      };
    });
  }

  onTimeFrameChange(frame: TimeFrame): void {
    this.selectedTimeFrame = frame;
    this.updateStatCardData();
  }

  onFullStatsClick(): void {
    console.log('Full stats clicked');
  }

  onMenuClick(): void {
    console.log('Menu clicked');
  }

  getStatCardRange(): string {
    if (!this.statCardData || this.statCardData.length === 0) return '0-0';
    const values = this.statCardData.map(d => d.value);
    const min = Math.min(...values);
    const max = Math.max(...values);
    return `${min}-${max}`;
  }

  getStatCardPeriod(): string {
    const now = new Date();
    return now.toLocaleDateString('en-US', { month: 'long', year: 'numeric' });
  }

  private generateTrendData(): void {
    if (!this.dashboard) {
      this.totalProductsTrend = this.generateSampleTrend(7, 5, 10);
      this.warehousesTrend = this.generateSampleTrend(2, 1, 3);
      this.stockValueTrend = this.generateSampleTrend(18000, 15000, 20000);
      this.lowStockTrend = this.generateSampleTrend(1, 0, 3);
      this.todaySalesTrend = this.generateSampleTrend(0, 0, 1000);
      this.todayPurchasesTrend = this.generateSampleTrend(0, 0, 500);
      this.pendingPOTrend = this.generateSampleTrend(2, 0, 5);
      this.pendingSOTrend = this.generateSampleTrend(1, 0, 4);
      return;
    }

    const baseProducts = this.dashboard.summary.totalProducts || 7;
    const baseWarehouses = this.dashboard.summary.totalWarehouses || 2;
    const baseStockValue = this.dashboard.summary.totalStockValue || 18000;
    const baseLowStock = this.dashboard.summary.lowStockItems || 1;
    const baseSales = this.dashboard.summary.todaySales || 0;
    const basePurchases = this.dashboard.summary.todayPurchases || 0;
    const basePO = this.dashboard.summary.pendingPurchaseOrders || 2;
    const baseSO = this.dashboard.summary.pendingSalesOrders || 1;

    this.totalProductsTrend = this.generateTrendFromBase(baseProducts, 0.1);
    this.warehousesTrend = this.generateTrendFromBase(baseWarehouses, 0.2);
    this.stockValueTrend = this.generateTrendFromBase(baseStockValue, 0.15);
    this.lowStockTrend = this.generateTrendFromBase(baseLowStock, 0.3);
    this.todaySalesTrend = this.generateTrendFromBase(baseSales, 0.4);
    this.todayPurchasesTrend = this.generateTrendFromBase(basePurchases, 0.4);
    this.pendingPOTrend = this.generateTrendFromBase(basePO, 0.25);
    this.pendingSOTrend = this.generateTrendFromBase(baseSO, 0.25);
  }

  private generateSampleTrend(base: number, min: number, max: number): number[] {
    const data: number[] = [];
    for (let i = 0; i < 7; i++) {
      const variation = (Math.random() - 0.5) * (max - min);
      data.push(Math.max(min, Math.min(max, base + variation)));
    }
    return data;
  }

  private generateTrendFromBase(base: number, variation: number): number[] {
    const data: number[] = [];
    for (let i = 0; i < 7; i++) {
      const change = (Math.random() - 0.5) * variation * base;
      data.push(Math.max(0, base + change));
    }
    return data;
  }

  getChangePercentage(trend: number[]): number {
    if (!trend || trend.length < 2) return 0;
    const current = trend[trend.length - 1];
    const previous = trend[trend.length - 2];
    if (previous === 0) return current > 0 ? 100 : 0;
    return ((current - previous) / previous) * 100;
  }

  Math = Math;
}

