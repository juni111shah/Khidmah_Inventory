import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { WarehouseApiService } from '../../../core/services/warehouse-api.service';
import { Warehouse, CreateWarehouseRequest, UpdateWarehouseRequest } from '../../../core/models/warehouse.model';
import { ToastComponent } from '../../../shared/components/toast/toast.component';
import { LoadingSpinnerComponent } from '../../../shared/components/loading-spinner/loading-spinner.component';
import { IconComponent } from '../../../shared/components/icon/icon.component';
import { PermissionService } from '../../../core/services/permission.service';
import { HasPermissionDirective } from '../../../shared/directives/has-permission.directive';
import { FormFieldComponent } from '../../../shared/components/form-field/form-field.component';
import { UnifiedButtonComponent } from '../../../shared/components/unified-button/unified-button.component';
import { HeaderService } from '../../../core/services/header.service';
import { UnifiedCardComponent } from '../../../shared/components/unified-card/unified-card.component';
import { UnifiedCheckboxComponent } from '../../../shared/components/unified-checkbox/unified-checkbox.component';
import { ContentLoaderComponent } from '../../../shared/components/content-loader/content-loader.component';
import { SkeletonDetailHeaderComponent } from '../../../shared/components/skeleton-detail-header/skeleton-detail-header.component';
import { SkeletonFormComponent } from '../../../shared/components/skeleton-form/skeleton-form.component';
import { SkeletonSidePanelComponent } from '../../../shared/components/skeleton-side-panel/skeleton-side-panel.component';

@Component({
  selector: 'app-warehouse-form',
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
    UnifiedCheckboxComponent,
    ContentLoaderComponent,
    SkeletonDetailHeaderComponent,
    SkeletonFormComponent,
    SkeletonSidePanelComponent
  ],
  templateUrl: './warehouse-form.component.html'
})
export class WarehouseFormComponent implements OnInit {
  warehouse: Warehouse | null = null;
  loading = false;
  saving = false;
  isEditMode = false;

  formData = {
    name: '',
    code: '',
    description: '',
    address: '',
    city: '',
    state: '',
    country: '',
    postalCode: '',
    phoneNumber: '',
    email: '',
    isDefault: false
  };

  showToast = false;
  toastMessage = '';
  toastType: 'success' | 'error' | 'warning' | 'info' = 'success';

  constructor(
    private warehouseApiService: WarehouseApiService,
    private route: ActivatedRoute,
    public router: Router,
    public permissionService: PermissionService,
    private headerService: HeaderService
  ) {}

  ngOnInit(): void {
    const warehouseId = this.route.snapshot.paramMap.get('id');
    const isEditPath = this.route.snapshot.url.some(segment => segment.path === 'edit');
    this.isEditMode = !!warehouseId && isEditPath;

    this.headerService.setHeaderInfo({
      title: this.isEditMode ? 'Edit Warehouse' : 'Create Warehouse',
      description: this.isEditMode ? 'Modify existing warehouse details' : 'Add a new warehouse location'
    });

    if (warehouseId) {
      this.loadWarehouse(warehouseId);
    }
  }

  loadWarehouse(id: string): void {
    this.loading = true;
    this.warehouseApiService.getWarehouse(id).subscribe({
      next: (response) => {
        if (response.success && response.data) {
          this.warehouse = response.data;
          this.formData = {
            name: response.data.name,
            code: response.data.code || '',
            description: response.data.description || '',
            address: response.data.address || '',
            city: response.data.city || '',
            state: response.data.state || '',
            country: response.data.country || '',
            postalCode: response.data.postalCode || '',
            phoneNumber: response.data.phoneNumber || '',
            email: response.data.email || '',
            isDefault: response.data.isDefault
          };
        } else {
          this.showToastMessage('error', response.message || 'Failed to load warehouse');
        }
        this.loading = false;
      },
      error: () => {
        this.showToastMessage('error', 'Error loading warehouse');
        this.loading = false;
      }
    });
  }

  save(): void {
    if (!this.formData.name.trim()) {
      this.showToastMessage('error', 'Warehouse name is required');
      return;
    }

    this.saving = true;

    if (this.isEditMode && this.warehouse) {
      const updateRequest: UpdateWarehouseRequest = {
        name: this.formData.name,
        code: this.formData.code || undefined,
        description: this.formData.description || undefined,
        address: this.formData.address || undefined,
        city: this.formData.city || undefined,
        state: this.formData.state || undefined,
        country: this.formData.country || undefined,
        postalCode: this.formData.postalCode || undefined,
        phoneNumber: this.formData.phoneNumber || undefined,
        email: this.formData.email || undefined,
        isDefault: this.formData.isDefault
      };

      this.warehouseApiService.updateWarehouse(this.warehouse.id, updateRequest).subscribe({
        next: (response) => {
          if (response.success) {
            this.showToastMessage('success', 'Warehouse updated successfully');
            setTimeout(() => {
              this.router.navigate(['/warehouses']);
            }, 1500);
          } else {
            this.showToastMessage('error', response.message || 'Failed to update warehouse');
          }
          this.saving = false;
        },
        error: () => {
          this.showToastMessage('error', 'Error updating warehouse');
          this.saving = false;
        }
      });
    } else {
      const createRequest: CreateWarehouseRequest = {
        name: this.formData.name,
        code: this.formData.code || undefined,
        description: this.formData.description || undefined,
        address: this.formData.address || undefined,
        city: this.formData.city || undefined,
        state: this.formData.state || undefined,
        country: this.formData.country || undefined,
        postalCode: this.formData.postalCode || undefined,
        phoneNumber: this.formData.phoneNumber || undefined,
        email: this.formData.email || undefined,
        isDefault: this.formData.isDefault
      };

      this.warehouseApiService.createWarehouse(createRequest).subscribe({
        next: (response) => {
          if (response.success) {
            this.showToastMessage('success', 'Warehouse created successfully');
            setTimeout(() => {
              this.router.navigate(['/warehouses']);
            }, 1500);
          } else {
            this.showToastMessage('error', response.message || 'Failed to create warehouse');
          }
          this.saving = false;
        },
        error: () => {
          this.showToastMessage('error', 'Error creating warehouse');
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
}
