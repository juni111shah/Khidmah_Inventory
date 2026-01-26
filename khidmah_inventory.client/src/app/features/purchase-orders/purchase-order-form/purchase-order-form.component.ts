import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { PurchaseOrderApiService } from '../../../core/services/purchase-order-api.service';
import { SupplierApiService } from '../../../core/services/supplier-api.service';
import { PurchaseOrder, CreatePurchaseOrderRequest, PurchaseOrderItem } from '../../../core/models/purchase-order.model';
import { Supplier } from '../../../core/models/supplier.model';
import { ToastComponent } from '../../../shared/components/toast/toast.component';
import { LoadingSpinnerComponent } from '../../../shared/components/loading-spinner/loading-spinner.component';
import { IconComponent } from '../../../shared/components/icon/icon.component';
import { HasPermissionDirective } from '../../../shared/directives/has-permission.directive';
import { FormFieldComponent } from '../../../shared/components/form-field/form-field.component';
import { UnifiedButtonComponent } from '../../../shared/components/unified-button/unified-button.component';
import { UnifiedCardComponent } from '../../../shared/components/unified-card/unified-card.component';
import { HeaderService } from '../../../core/services/header.service';

@Component({
  selector: 'app-purchase-order-form',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ToastComponent,
    LoadingSpinnerComponent,
    IconComponent,
    HasPermissionDirective,
    FormFieldComponent,
    UnifiedButtonComponent,
    UnifiedCardComponent
  ],
  templateUrl: './purchase-order-form.component.html'
})
export class PurchaseOrderFormComponent implements OnInit {
  purchaseOrder: PurchaseOrder | null = null;
  suppliers: Supplier[] = [];
  loading = false;
  saving = false;
  isEditMode = false;

  formData = {
    supplierId: '',
    orderDate: new Date().toISOString().split('T')[0],
    expectedDeliveryDate: '',
    notes: '',
    termsAndConditions: ''
  };

  showToast = false;
  toastMessage = '';
  toastType: 'success' | 'error' | 'warning' | 'info' = 'success';

  constructor(
    private purchaseOrderApiService: PurchaseOrderApiService,
    private supplierApiService: SupplierApiService,
    private route: ActivatedRoute,
    public router: Router,
    private headerService: HeaderService
  ) {}

  ngOnInit(): void {
    const orderId = this.route.snapshot.paramMap.get('id');
    this.isEditMode = !!orderId;

    this.headerService.setHeaderInfo({
      title: this.isEditMode ? 'Edit Purchase Order' : 'New Purchase Order',
      description: this.isEditMode ? 'Modify purchase order details' : 'Create a new purchase order'
    });

    this.loadSuppliers();

    if (this.isEditMode && orderId) {
      this.loadPurchaseOrder(orderId);
    }
  }

  loadSuppliers(): void {
    this.supplierApiService.getSuppliers({ isActive: true }).subscribe({
      next: (response) => {
        if (response.success && response.data) {
          this.suppliers = response.data.items;
        }
      }
    });
  }

  loadPurchaseOrder(id: string): void {
    this.loading = true;
    this.purchaseOrderApiService.getPurchaseOrder(id).subscribe({
      next: (response) => {
        if (response.success && response.data) {
          this.purchaseOrder = response.data;
          this.formData = {
            supplierId: response.data.supplierId,
            orderDate: new Date(response.data.orderDate).toISOString().split('T')[0],
            expectedDeliveryDate: response.data.expectedDeliveryDate ? new Date(response.data.expectedDeliveryDate).toISOString().split('T')[0] : '',
            notes: response.data.notes || '',
            termsAndConditions: response.data.termsAndConditions || ''
          };
        } else {
          this.showToastMessage('error', response.message || 'Failed to load purchase order');
        }
        this.loading = false;
      },
      error: () => {
        this.showToastMessage('error', 'Error loading purchase order');
        this.loading = false;
      }
    });
  }

  save(): void {
    if (!this.formData.supplierId) {
      this.showToastMessage('error', 'Supplier is required');
      return;
    }
    if (!this.formData.orderDate) {
      this.showToastMessage('error', 'Order date is required');
      return;
    }

    this.saving = true;

    // Note: This is a simplified implementation. Real-world would handle items.
    const request: any = {
      supplierId: this.formData.supplierId,
      orderDate: new Date(this.formData.orderDate).toISOString(),
      expectedDeliveryDate: this.formData.expectedDeliveryDate ? new Date(this.formData.expectedDeliveryDate).toISOString() : undefined,
      notes: this.formData.notes || undefined,
      termsAndConditions: this.formData.termsAndConditions || undefined,
      items: [] // Placeholder: items need to be implemented
    };

    if (this.isEditMode && this.purchaseOrder) {
      // Update logic would go here
       this.showToastMessage('info', 'Update functionality pending item implementation');
       this.saving = false;
    } else {
      this.purchaseOrderApiService.createPurchaseOrder(request as CreatePurchaseOrderRequest).subscribe({
        next: (response) => {
          if (response.success) {
            this.showToastMessage('success', 'Purchase Order created successfully');
            setTimeout(() => {
              this.router.navigate(['/purchase-orders']);
            }, 1500);
          } else {
            this.showToastMessage('error', response.message || 'Failed to create purchase order');
          }
          this.saving = false;
        },
        error: () => {
          // this.showToastMessage('error', 'Error creating purchase order');
           // Since items are empty it might fail validation on backend, treating as simulated success for navigation check
           console.error('Error creating PO');
           this.showToastMessage('error', 'Backend requires items. Items UI implementation needed.');
           this.saving = false;
        }
      });
    }
  }

  get supplierOptions(): { value: any; label: string }[] {
    return [
      { value: '', label: 'Select Supplier' },
      ...this.suppliers.map(s => ({ value: s.id, label: s.name }))
    ];
  }

  showToastMessage(type: 'success' | 'error' | 'warning' | 'info', message: string): void {
    this.toastType = type;
    this.toastMessage = message;
    this.showToast = true;
    setTimeout(() => {
      this.showToast = false;
    }, 3000);
  }
}
