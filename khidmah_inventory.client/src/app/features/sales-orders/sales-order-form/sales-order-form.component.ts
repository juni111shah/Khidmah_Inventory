import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { SalesOrderApiService } from '../../../core/services/sales-order-api.service';
import { CustomerApiService } from '../../../core/services/customer-api.service';
import { SalesOrder, CreateSalesOrderRequest } from '../../../core/models/sales-order.model';
import { Customer } from '../../../core/models/customer.model';
import { ToastComponent } from '../../../shared/components/toast/toast.component';
import { LoadingSpinnerComponent } from '../../../shared/components/loading-spinner/loading-spinner.component';
import { IconComponent } from '../../../shared/components/icon/icon.component';
import { HasPermissionDirective } from '../../../shared/directives/has-permission.directive';
import { FormFieldComponent } from '../../../shared/components/form-field/form-field.component';
import { UnifiedButtonComponent } from '../../../shared/components/unified-button/unified-button.component';
import { UnifiedCardComponent } from '../../../shared/components/unified-card/unified-card.component';
import { HeaderService } from '../../../core/services/header.service';

@Component({
  selector: 'app-sales-order-form',
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
  templateUrl: './sales-order-form.component.html'
})
export class SalesOrderFormComponent implements OnInit {
  salesOrder: SalesOrder | null = null;
  customers: Customer[] = [];
  loading = false;
  saving = false;
  isEditMode = false;

  formData = {
    customerId: '',
    orderDate: new Date().toISOString().split('T')[0],
    expectedDeliveryDate: '',
    notes: '',
    termsAndConditions: ''
  };

  showToast = false;
  toastMessage = '';
  toastType: 'success' | 'error' | 'warning' | 'info' = 'success';

  constructor(
    private salesOrderApiService: SalesOrderApiService,
    private customerApiService: CustomerApiService,
    private route: ActivatedRoute,
    public router: Router,
    private headerService: HeaderService
  ) {}

  ngOnInit(): void {
    const orderId = this.route.snapshot.paramMap.get('id');
    this.isEditMode = !!orderId;

    this.headerService.setHeaderInfo({
      title: this.isEditMode ? 'Edit Sales Order' : 'New Sales Order',
      description: this.isEditMode ? 'Modify sales order details' : 'Create a new sales order'
    });

    this.loadCustomers();

    if (this.isEditMode && orderId) {
      this.loadSalesOrder(orderId);
    }
  }

  loadCustomers(): void {
    this.customerApiService.getCustomers({ isActive: true }).subscribe({
      next: (response) => {
        if (response.success && response.data) {
          this.customers = response.data.items;
        }
      }
    });
  }

  loadSalesOrder(id: string): void {
    this.loading = true;
    this.salesOrderApiService.getSalesOrder(id).subscribe({
      next: (response) => {
        if (response.success && response.data) {
          this.salesOrder = response.data;
          this.formData = {
            customerId: response.data.customerId,
            orderDate: new Date(response.data.orderDate).toISOString().split('T')[0],
            expectedDeliveryDate: response.data.expectedDeliveryDate ? new Date(response.data.expectedDeliveryDate).toISOString().split('T')[0] : '',
            notes: response.data.notes || '',
            termsAndConditions: response.data.termsAndConditions || ''
          };
        } else {
          this.showToastMessage('error', response.message || 'Failed to load sales order');
        }
        this.loading = false;
      },
      error: () => {
        this.showToastMessage('error', 'Error loading sales order');
        this.loading = false;
      }
    });
  }

  save(): void {
    if (!this.formData.customerId) {
      this.showToastMessage('error', 'Customer is required');
      return;
    }
    if (!this.formData.orderDate) {
      this.showToastMessage('error', 'Order date is required');
      return;
    }

    this.saving = true;

    // Note: This is a simplified implementation. Real-world would handle items.
    const request: any = {
      customerId: this.formData.customerId,
      orderDate: new Date(this.formData.orderDate).toISOString(),
      expectedDeliveryDate: this.formData.expectedDeliveryDate ? new Date(this.formData.expectedDeliveryDate).toISOString() : undefined,
      notes: this.formData.notes || undefined,
      termsAndConditions: this.formData.termsAndConditions || undefined,
      items: [] // Placeholder: items need to be implemented
    };

    if (this.isEditMode && this.salesOrder) {
      // Update logic would go here
       this.showToastMessage('info', 'Update functionality pending item implementation');
       this.saving = false;
    } else {
      this.salesOrderApiService.createSalesOrder(request as CreateSalesOrderRequest).subscribe({
        next: (response) => {
          if (response.success) {
            this.showToastMessage('success', 'Sales Order created successfully');
            setTimeout(() => {
              this.router.navigate(['/sales-orders']);
            }, 1500);
          } else {
            this.showToastMessage('error', response.message || 'Failed to create sales order');
          }
          this.saving = false;
        },
        error: () => {
          // this.showToastMessage('error', 'Error creating sales order');
           console.error('Error creating SO');
           this.showToastMessage('error', 'Backend requires items. Items UI implementation needed.');
           this.saving = false;
        }
      });
    }
  }

  get customerOptions(): { value: any; label: string }[] {
    return [
      { value: '', label: 'Select Customer' },
      ...this.customers.map(c => ({ value: c.id, label: c.name }))
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
