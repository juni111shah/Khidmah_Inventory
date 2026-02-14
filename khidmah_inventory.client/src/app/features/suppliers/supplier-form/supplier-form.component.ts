import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { SupplierApiService } from '../../../core/services/supplier-api.service';
import { Supplier, CreateSupplierRequest } from '../../../core/models/supplier.model';
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
  selector: 'app-supplier-form',
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
  templateUrl: './supplier-form.component.html'
})
export class SupplierFormComponent implements OnInit {
  supplier: Supplier | null = null;
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
    private supplierApiService: SupplierApiService,
    private route: ActivatedRoute,
    public router: Router,
    private headerService: HeaderService,
    private exportService: ExportService
  ) {}

  ngOnInit(): void {
    const supplierId = this.route.snapshot.paramMap.get('id');
    const url = this.router.url;

    // If we have an ID but URL doesn't end with /edit, we are in View Mode
    this.isEditMode = !!supplierId && url.includes('/edit');
    this.isViewMode = !!supplierId && !url.includes('/edit');

    this.headerService.setHeaderInfo({
      title: this.isViewMode ? 'Supplier Details' : (this.isEditMode ? 'Edit Supplier' : 'New Supplier'),
      description: this.isViewMode ? 'View supplier information' : (this.isEditMode ? 'Update supplier information' : 'Register a new supplier')
    });

    if (supplierId) {
      this.loadSupplier(supplierId);
    }
  }

  loadSupplier(id: string): void {
    this.loading = true;
    this.supplierApiService.getSupplier(id).subscribe({
      next: (response) => {
        if (response.success && response.data) {
          this.supplier = response.data;
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
          this.showToastMessage('error', response.message || 'Failed to load supplier');
        }
        this.loading = false;
      },
      error: () => {
        this.showToastMessage('error', 'Error loading supplier');
        this.loading = false;
      }
    });
  }

  save(): void {
    if (!this.formData.name.trim()) {
      this.showToastMessage('error', 'Supplier name is required');
      return;
    }

    this.saving = true;

    if (this.isEditMode && this.supplier) {
      const updateRequest: any = { // Using any for now to bypass strict typing if UpdateSupplierRequest isn't perfectly aligned
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

      this.supplierApiService.updateSupplier(this.supplier.id, updateRequest).subscribe({
        next: (response) => {
          if (response.success) {
            this.showToastMessage('success', 'Supplier updated successfully');
            setTimeout(() => {
              this.router.navigate(['/suppliers']);
            }, 1500);
          } else {
            this.showToastMessage('error', response.message || 'Failed to update supplier');
          }
          this.saving = false;
        },
        error: () => {
          this.showToastMessage('error', 'Error updating supplier');
          this.saving = false;
        }
      });
    } else {
      const createRequest: CreateSupplierRequest = {
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

      this.supplierApiService.createSupplier(createRequest).subscribe({
        next: (response) => {
          if (response.success) {
            this.showToastMessage('success', 'Supplier created successfully');
            setTimeout(() => {
              this.router.navigate(['/suppliers']);
            }, 1500);
          } else {
            this.showToastMessage('error', response.message || 'Failed to create supplier');
          }
          this.saving = false;
        },
        error: () => {
          this.showToastMessage('error', 'Error creating supplier');
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
    if (!this.supplier) return;

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
        `Supplier Details: ${this.formData.name}`,
        `supplier_details_${this.formData.code || this.supplier.id}`
      );
      this.showToastMessage('success', 'PDF exported successfully');
    } catch (error) {
      console.error(error);
      this.showToastMessage('error', 'Failed to export PDF');
    }
  }
}
