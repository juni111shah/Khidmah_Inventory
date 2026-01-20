import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { IconComponent } from '../../shared/components/icon/icon.component';
import { ReportApiService } from '../../core/services/report-api.service';
import { SalesReport, InventoryReport, PurchaseReport, SalesReportItem, InventoryReportItem, PurchaseReportItem } from '../../core/models/report.model';
import { ToastComponent } from '../../shared/components/toast/toast.component';
import { LoadingSpinnerComponent } from '../../shared/components/loading-spinner/loading-spinner.component';
import { HasPermissionDirective } from '../../shared/directives/has-permission.directive';
import { DataTableComponent } from '../../shared/components/data-table/data-table.component';
import { DataTableColumn, DataTableConfig } from '../../shared/models/data-table.model';
import { PermissionService } from '../../core/services/permission.service';
import { UnifiedCardComponent } from '../../shared/components/unified-card/unified-card.component';
import { UnifiedButtonComponent } from '../../shared/components/unified-button/unified-button.component';
import { HeaderService } from '../../core/services/header.service';

@Component({
  selector: 'app-reports',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ToastComponent,
    LoadingSpinnerComponent,
    HasPermissionDirective,
    DataTableComponent,
    IconComponent,
    UnifiedCardComponent,
    UnifiedButtonComponent
  ],
  templateUrl: './reports.component.html'
})
export class ReportsComponent implements OnInit {
  activeTab: 'sales' | 'inventory' | 'purchase' = 'sales';
  loading = false;

  // Sales Report
  salesReport: SalesReport | null = null;
  salesFromDate: Date = new Date(new Date().setDate(new Date().getDate() - 30));
  salesToDate: Date = new Date();

  // Inventory Report
  inventoryReport: InventoryReport | null = null;
  inventoryWarehouseId: string | null = null;
  inventoryCategoryId: string | null = null;
  lowStockOnly = false;

  // Purchase Report
  purchaseReport: PurchaseReport | null = null;
  purchaseFromDate: Date = new Date(new Date().setDate(new Date().getDate() - 30));
  purchaseToDate: Date = new Date();

  // Sales Report Table Columns
  salesReportColumns: DataTableColumn<SalesReportItem>[] = [
    { 
      key: 'date', 
      label: 'Date', 
      sortable: true, 
      type: 'date',
      width: '150px',
      format: (value) => new Date(value).toLocaleString()
    },
    { key: 'orderNumber', label: 'Order #', sortable: true, width: '120px' },
    { key: 'customerName', label: 'Customer', sortable: true, width: '200px' },
    { 
      key: 'amount', 
      label: 'Amount', 
      sortable: true, 
      type: 'number',
      width: '120px',
      format: (value) => this.formatCurrency(value)
    },
    { 
      key: 'cost', 
      label: 'Cost', 
      sortable: true, 
      type: 'number',
      width: '120px',
      format: (value) => this.formatCurrency(value)
    },
    { 
      key: 'profit', 
      label: 'Profit', 
      sortable: true, 
      type: 'number',
      width: '120px',
      format: (value) => this.formatCurrency(value),
      render: (row) => {
        const profit = this.formatCurrency(row.profit);
        return profit;
      }
    }
  ];

  // Inventory Report Table Columns
  inventoryReportColumns: DataTableColumn<InventoryReportItem>[] = [
    { key: 'productName', label: 'Product', sortable: true, width: '200px' },
    { key: 'productSKU', label: 'SKU', sortable: true, width: '120px' },
    { key: 'categoryName', label: 'Category', sortable: true, width: '150px' },
    { key: 'warehouseName', label: 'Warehouse', sortable: true, width: '150px' },
    { 
      key: 'quantity', 
      label: 'Quantity', 
      sortable: true, 
      type: 'number',
      width: '100px',
      format: (value) => this.formatNumber(value)
    },
    { 
      key: 'averageCost', 
      label: 'Avg Cost', 
      sortable: true, 
      type: 'number',
      width: '120px',
      format: (value) => this.formatCurrency(value)
    },
    { 
      key: 'stockValue', 
      label: 'Stock Value', 
      sortable: true, 
      type: 'number',
      width: '120px',
      format: (value) => this.formatCurrency(value)
    },
    { 
      key: 'status', 
      label: 'Status', 
      sortable: true, 
      type: 'badge',
      width: '100px'
    }
  ];

  // Purchase Report Table Columns
  purchaseReportColumns: DataTableColumn<PurchaseReportItem>[] = [
    { 
      key: 'date', 
      label: 'Date', 
      sortable: true, 
      type: 'date',
      width: '150px',
      format: (value) => new Date(value).toLocaleString()
    },
    { key: 'orderNumber', label: 'Order #', sortable: true, width: '120px' },
    { key: 'supplierName', label: 'Supplier', sortable: true, width: '200px' },
    { 
      key: 'amount', 
      label: 'Amount', 
      sortable: true, 
      type: 'number',
      width: '120px',
      format: (value) => this.formatCurrency(value)
    },
    { 
      key: 'status', 
      label: 'Status', 
      sortable: true, 
      type: 'badge',
      width: '100px'
    }
  ];

  tableConfig: DataTableConfig = {
    showSearch: false,
    showFilters: false,
    showColumnToggle: false,
    showPagination: false,
    showActions: false,
    showCheckbox: false
  };

  showToast = false;
  toastMessage = '';
  toastType: 'success' | 'error' | 'warning' | 'info' = 'success';

  constructor(
    private reportApiService: ReportApiService,
    public permissionService: PermissionService,
    private headerService: HeaderService
  ) {}

  ngOnInit(): void {
    this.headerService.setHeaderInfo({
      title: 'Reports',
      description: 'System reports and data analysis'
    });
    this.loadSalesReport();
  }

  onTabChange(tab: 'sales' | 'inventory' | 'purchase'): void {
    this.activeTab = tab;
    if (tab === 'sales') {
      this.loadSalesReport();
    } else if (tab === 'inventory') {
      this.loadInventoryReport();
    } else {
      this.loadPurchaseReport();
    }
  }

  loadSalesReport(): void {
    this.loading = true;
    this.reportApiService.getSalesReport(this.salesFromDate, this.salesToDate).subscribe({
      next: (response) => {
        if (response.success && response.data) {
          this.salesReport = response.data;
        } else {
          this.showToastMessage('error', response.message || 'Failed to load sales report');
        }
        this.loading = false;
      },
      error: () => {
        this.showToastMessage('error', 'Error loading sales report');
        this.loading = false;
      }
    });
  }

  loadInventoryReport(): void {
    this.loading = true;
    this.reportApiService.getInventoryReport(
      this.inventoryWarehouseId || undefined,
      this.inventoryCategoryId || undefined,
      this.lowStockOnly
    ).subscribe({
      next: (response) => {
        if (response.success && response.data) {
          this.inventoryReport = response.data;
        } else {
          this.showToastMessage('error', response.message || 'Failed to load inventory report');
        }
        this.loading = false;
      },
      error: () => {
        this.showToastMessage('error', 'Error loading inventory report');
        this.loading = false;
      }
    });
  }

  loadPurchaseReport(): void {
    this.loading = true;
    this.reportApiService.getPurchaseReport(this.purchaseFromDate, this.purchaseToDate).subscribe({
      next: (response) => {
        if (response.success && response.data) {
          this.purchaseReport = response.data;
        } else {
          this.showToastMessage('error', response.message || 'Failed to load purchase report');
        }
        this.loading = false;
      },
      error: () => {
        this.showToastMessage('error', 'Error loading purchase report');
        this.loading = false;
      }
    });
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
}


