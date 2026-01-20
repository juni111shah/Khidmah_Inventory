import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { InventoryApiService } from '../../../core/services/inventory-api.service';
import { ProductApiService } from '../../../core/services/product-api.service';
import { WarehouseApiService } from '../../../core/services/warehouse-api.service';
import { Product } from '../../../core/models/product.model';
import { Warehouse } from '../../../core/models/warehouse.model';
import { ToastComponent } from '../../../shared/components/toast/toast.component';
import { LoadingSpinnerComponent } from '../../../shared/components/loading-spinner/loading-spinner.component';
import { IconComponent } from '../../../shared/components/icon/icon.component';
import { HasPermissionDirective } from '../../../shared/directives/has-permission.directive';
import { PermissionService } from '../../../core/services/permission.service';
import { HeaderService } from '../../../core/services/header.service';
import { UnifiedButtonComponent } from '../../../shared/components/unified-button/unified-button.component';
import { UnifiedInputComponent } from '../../../shared/components/unified-input/unified-input.component';
import { UnifiedSelectComponent } from '../../../shared/components/unified-select/unified-select.component';
import { UnifiedTextareaComponent } from '../../../shared/components/unified-textarea/unified-textarea.component';
import { UnifiedDatePickerComponent } from '../../../shared/components/unified-date-picker/unified-date-picker.component';
import { UnifiedCardComponent } from '../../../shared/components/unified-card/unified-card.component';

@Component({
  selector: 'app-stock-transfer',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ToastComponent,
    LoadingSpinnerComponent,
    IconComponent,
    HasPermissionDirective,
    UnifiedButtonComponent,
    UnifiedInputComponent,
    UnifiedSelectComponent,
    UnifiedTextareaComponent,
    UnifiedDatePickerComponent,
    UnifiedCardComponent
  ],
  templateUrl: './stock-transfer.component.html'
})
export class StockTransferComponent implements OnInit {
  products: Product[] = [];
  warehouses: Warehouse[] = [];
  loading = false;
  transferring = false;

  formData = {
    productId: '',
    fromWarehouseId: '',
    toWarehouseId: '',
    quantity: 0,
    notes: '',
    batchNumber: '',
    expiryDate: ''
  };

  selectedProduct: Product | null = null;
  availableStock: number = 0;

  showToast = false;
  toastMessage = '';
  toastType: 'success' | 'error' | 'warning' | 'info' = 'success';

  constructor(
    private inventoryApiService: InventoryApiService,
    private productApiService: ProductApiService,
    private warehouseApiService: WarehouseApiService,
    public router: Router,
    public permissionService: PermissionService,
    private headerService: HeaderService
  ) {}

  // Computed properties for unified components
  get productOptions(): Array<{ value: string; label: string }> {
    return this.products.map(product => ({
      value: product.id,
      label: `${product.name} (${product.sku})`
    }));
  }

  get warehouseOptions(): Array<{ value: string; label: string }> {
    return this.warehouses.map(warehouse => ({
      value: warehouse.id,
      label: warehouse.name
    }));
  }

  getSelectedProduct(): Product | null {
    return this.products.find(p => p.id === this.formData.productId) || null;
  }

  getSelectedWarehouse(warehouseId: string): Warehouse | null {
    return this.warehouses.find(w => w.id === warehouseId) || null;
  }

  isFormValid(): boolean {
    return !!(
      this.formData.productId &&
      this.formData.fromWarehouseId &&
      this.formData.toWarehouseId &&
      this.formData.quantity > 0 &&
      this.formData.quantity >= 0.01 &&
      this.formData.fromWarehouseId !== this.formData.toWarehouseId &&
      (!this.availableStock || this.formData.quantity <= this.availableStock)
    );
  }

  ngOnInit(): void {
    this.headerService.setHeaderInfo({
      title: 'Stock Transfer',
      description: 'Move inventory between warehouses'
    });
    this.loadProducts();
    this.loadWarehouses();
  }

  loadProducts(): void {
    this.productApiService.getProducts({}).subscribe({
      next: (response) => {
        if (response.success && response.data) {
          this.products = response.data.items;
        }
      }
    });
  }

  loadWarehouses(): void {
    this.warehouseApiService.getWarehouses({}).subscribe({
      next: (response) => {
        if (response.success && response.data) {
          this.warehouses = response.data.items;
        }
      }
    });
  }

  onProductChange(): void {
    if (this.formData.productId && this.formData.fromWarehouseId) {
      this.checkAvailableStock();
    }
  }

  onFromWarehouseChange(): void {
    if (this.formData.productId && this.formData.fromWarehouseId) {
      this.checkAvailableStock();
    }
  }

  checkAvailableStock(): void {
    // In a real implementation, you'd call an API to get stock level
    // For now, we'll set a placeholder
    this.availableStock = 0;
  }

  transfer(): void {
    if (!this.formData.productId || !this.formData.fromWarehouseId || !this.formData.toWarehouseId) {
      this.showToastMessage('error', 'Please fill in all required fields');
      return;
    }

    if (this.formData.quantity <= 0 || this.formData.quantity < 0.01) {
      this.showToastMessage('error', 'Quantity must be at least 0.01');
      return;
    }

    if (this.formData.fromWarehouseId === this.formData.toWarehouseId) {
      this.showToastMessage('error', 'Source and destination warehouses must be different');
      return;
    }

    this.transferring = true;

    const request = {
      productId: this.formData.productId,
      fromWarehouseId: this.formData.fromWarehouseId,
      toWarehouseId: this.formData.toWarehouseId,
      quantity: this.formData.quantity,
      notes: this.formData.notes || undefined,
      batchNumber: this.formData.batchNumber || undefined,
      expiryDate: this.formData.expiryDate || undefined
    };

    this.inventoryApiService.transferStock(request).subscribe({
      next: (response) => {
        if (response.success) {
          this.showToastMessage('success', 'Stock transferred successfully');
          setTimeout(() => {
            this.router.navigate(['/inventory/stock-levels']);
          }, 1500);
        } else {
          this.showToastMessage('error', response.message || 'Failed to transfer stock');
        }
        this.transferring = false;
      },
      error: () => {
        this.showToastMessage('error', 'Error transferring stock');
        this.transferring = false;
      }
    });
  }

  showToastMessage(type: 'success' | 'error' | 'warning' | 'info', message: string): void {
    this.toastType = type;
    this.toastMessage = message;
    this.showToast = true;
    setTimeout(() => { this.showToast = false; }, 3000);
  }
}


