import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { CustomerApiService } from '../../../core/services/customer-api.service';
import { Customer, CreateCustomerRequest } from '../../../core/models/customer.model';
import { ToastComponent } from '../../../shared/components/toast/toast.component';
import { LoadingSpinnerComponent } from '../../../shared/components/loading-spinner/loading-spinner.component';
import { IconComponent } from '../../../shared/components/icon/icon.component';
import { HasPermissionDirective } from '../../../shared/directives/has-permission.directive';
import { FormFieldComponent } from '../../../shared/components/form-field/form-field.component';
import { UnifiedButtonComponent } from '../../../shared/components/unified-button/unified-button.component';
import { UnifiedCardComponent } from '../../../shared/components/unified-card/unified-card.component';
import { HeaderService } from '../../../core/services/header.service';
import { ExportService } from '../../../core/services/export.service';
import { ContentLoaderComponent } from '../../../shared/components/content-loader/content-loader.component';
import { SkeletonDetailHeaderComponent } from '../../../shared/components/skeleton-detail-header/skeleton-detail-header.component';
import { SkeletonFormComponent } from '../../../shared/components/skeleton-form/skeleton-form.component';

@Component({
  selector: 'app-customer-form',
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
    UnifiedCardComponent,
    ContentLoaderComponent,
    SkeletonDetailHeaderComponent,
    SkeletonFormComponent
  ],
  templateUrl: './customer-form.component.html'
})
export class CustomerFormComponent implements OnInit {
  customer: Customer | null = null;
  loading = false;
  saving = false;
  isEditMode = false;
  isViewMode = false;

  formData = {
    name: '',
    code: '',
    contactPerson: '',
    email: '',
    phoneNumber: '',
    address: '',
    city: '',
    state: '',
    country: '',
    postalCode: '',
    taxId: '',
    paymentTerms: '',
    creditLimit: 0,
    isActive: true
  };

  showToast = false;
  toastMessage = '';
  toastType: 'success' | 'error' | 'warning' | 'info' = 'success';

  constructor(
    private customerApiService: CustomerApiService,
    private route: ActivatedRoute,
    public router: Router,
    private headerService: HeaderService,
    private exportService: ExportService
  ) {}

  ngOnInit(): void {
    const customerId = this.route.snapshot.paramMap.get('id');
    const url = this.router.url;

    // If we have an ID but URL doesn't end with /edit, we are in View Mode
    this.isEditMode = !!customerId && url.includes('/edit');
    this.isViewMode = !!customerId && !url.includes('/edit');

    this.headerService.setHeaderInfo({
      title: this.isViewMode ? 'Customer Details' : (this.isEditMode ? 'Edit Customer' : 'New Customer'),
      description: this.isViewMode ? 'View customer information' : (this.isEditMode ? 'Update customer information' : 'Register a new customer')
    });

    if (customerId) {
      this.loadCustomer(customerId);
    }
  }

  loadCustomer(id: string): void {
    this.loading = true;
    this.customerApiService.getCustomer(id).subscribe({
      next: (response) => {
        if (response.success && response.data) {
          this.customer = response.data;
          this.formData = {
            name: response.data.name,
            code: response.data.code || '',
            contactPerson: response.data.contactPerson || '',
            email: response.data.email || '',
            phoneNumber: response.data.phoneNumber || '',
            address: response.data.address || '',
            city: response.data.city || '',
            state: response.data.state || '',
            country: response.data.country || '',
            postalCode: response.data.postalCode || '',
            taxId: response.data.taxId || '',
            paymentTerms: response.data.paymentTerms || '',
            creditLimit: response.data.creditLimit || 0,
            isActive: response.data.isActive
          };
        } else {
          this.showToastMessage('error', response.message || 'Failed to load customer');
        }
        this.loading = false;
      },
      error: () => {
        this.showToastMessage('error', 'Error loading customer');
        this.loading = false;
      }
    });
  }

  save(): void {
    if (!this.formData.name.trim()) {
      this.showToastMessage('error', 'Customer name is required');
      return;
    }

    this.saving = true;

    if (this.isEditMode && this.customer) {
      const updateRequest: any = {
        name: this.formData.name,
        code: this.formData.code || undefined,
        contactPerson: this.formData.contactPerson || undefined,
        email: this.formData.email || undefined,
        phoneNumber: this.formData.phoneNumber || undefined,
        address: this.formData.address || undefined,
        city: this.formData.city || undefined,
        state: this.formData.state || undefined,
        country: this.formData.country || undefined,
        postalCode: this.formData.postalCode || undefined,
        taxId: this.formData.taxId || undefined,
        paymentTerms: this.formData.paymentTerms || undefined,
        creditLimit: this.formData.creditLimit,
        isActive: this.formData.isActive
      };

      this.customerApiService.updateCustomer(this.customer.id, updateRequest).subscribe({
        next: (response) => {
          if (response.success) {
            this.showToastMessage('success', 'Customer updated successfully');
            setTimeout(() => {
              this.router.navigate(['/customers']);
            }, 1500);
          } else {
            this.showToastMessage('error', response.message || 'Failed to update customer');
          }
          this.saving = false;
        },
        error: () => {
          this.showToastMessage('error', 'Error updating customer');
          this.saving = false;
        }
      });
    } else {
      const createRequest: CreateCustomerRequest = {
        name: this.formData.name,
        code: this.formData.code || undefined,
        contactPerson: this.formData.contactPerson || undefined,
        email: this.formData.email || undefined,
        phoneNumber: this.formData.phoneNumber || undefined,
        address: this.formData.address || undefined,
        city: this.formData.city || undefined,
        state: this.formData.state || undefined,
        country: this.formData.country || undefined,
        postalCode: this.formData.postalCode || undefined,
        taxId: this.formData.taxId || undefined,
        paymentTerms: this.formData.paymentTerms || undefined,
        creditLimit: this.formData.creditLimit
      };

      this.customerApiService.createCustomer(createRequest).subscribe({
        next: (response) => {
          if (response.success) {
            this.showToastMessage('success', 'Customer created successfully');
            setTimeout(() => {
              this.router.navigate(['/customers']);
            }, 1500);
          } else {
            this.showToastMessage('error', response.message || 'Failed to create customer');
          }
          this.saving = false;
        },
        error: () => {
          this.showToastMessage('error', 'Error creating customer');
          this.saving = false;
        }
      });
    }
  }

  showToastMessage(type: 'success' | 'error' | 'warning' | 'info', message: string): void {
    this.toastType = type;
    this.toastMessage = message;
    this.showToast = true;
    setTimeout(() => {
      this.showToast = false;
    }, 3000);
  }

  async exportToPdf(): Promise<void> {
    if (!this.customer) return;

    const details = [
      { label: 'Name', value: this.formData.name },
      { label: 'Code', value: this.formData.code },
      { label: 'Contact Person', value: this.formData.contactPerson },
      { label: 'Email', value: this.formData.email },
      { label: 'Phone', value: this.formData.phoneNumber },
      { label: 'Address', value: this.formData.address },
      { label: 'City', value: this.formData.city },
      { label: 'State', value: this.formData.state },
      { label: 'Postal Code', value: this.formData.postalCode },
      { label: 'Country', value: this.formData.country },
      { label: 'Tax ID', value: this.formData.taxId },
      { label: 'Payment Terms', value: this.formData.paymentTerms },
      { label: 'Credit Limit', value: this.formData.creditLimit },
      { label: 'Status', value: this.formData.isActive ? 'Active' : 'Inactive' }
    ];

    try {
      await this.exportService.exportEntityDetails(
        details,
        `Customer Details: ${this.formData.name}`,
        `customer_details_${this.formData.code || this.customer.id}`
      );
      this.showToastMessage('success', 'PDF exported successfully');
    } catch (error) {
      console.error(error);
      this.showToastMessage('error', 'Failed to export PDF');
    }
  }
}
