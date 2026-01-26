import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { UnifiedModalComponent } from '../../../shared/components/unified-modal/unified-modal.component';
import { UnifiedInputComponent } from '../../../shared/components/unified-input/unified-input.component';
import { UnifiedSelectComponent } from '../../../shared/components/unified-select/unified-select.component';
import { UnifiedButtonComponent } from '../../../shared/components/unified-button/unified-button.component';
import { UnifiedTextareaComponent } from '../../../shared/components/unified-textarea/unified-textarea.component';
import { InventoryApiService } from '../../../core/services/inventory-api.service';
import { ProductApiService } from '../../../core/services/product-api.service';
import { WarehouseApiService } from '../../../core/services/warehouse-api.service';
import { ToastComponent } from '../../../shared/components/toast/toast.component';

@Component({
  selector: 'app-create-batch',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    UnifiedModalComponent,
    UnifiedInputComponent,
    UnifiedSelectComponent,
    UnifiedButtonComponent,
    UnifiedTextareaComponent,
    ToastComponent
  ],
  templateUrl: './create-batch.component.html'
})
export class CreateBatchComponent implements OnInit {
  @Input() show: boolean = false;
  @Output() close = new EventEmitter<void>();
  @Output() saved = new EventEmitter<void>();

  batchForm: FormGroup;
  products: any[] = [];
  warehouses: any[] = [];
  loading: boolean = false;
  submitting: boolean = false;

  showToast = false;
  toastMessage = '';
  toastType: 'success' | 'error' = 'success';

  constructor(
    private fb: FormBuilder,
    private inventoryApi: InventoryApiService,
    private productApi: ProductApiService,
    private warehouseApi: WarehouseApiService
  ) {
    this.batchForm = this.fb.group({
      productId: ['', Validators.required],
      warehouseId: ['', Validators.required],
      batchNumber: ['', Validators.required],
      lotNumber: [''],
      manufactureDate: [''],
      expiryDate: [''],
      quantity: [0, [Validators.required, Validators.min(0.01)]],
      unitCost: [0],
      supplierName: [''],
      supplierBatchNumber: [''],
      notes: ['']
    });
  }

  ngOnInit(): void {
    this.loadInitialData();
  }

  loadInitialData() {
    this.loading = true;
    this.productApi.getProducts({}).subscribe(res => {
      if (res.success && res.data) this.products = res.data.items;
    });
    this.warehouseApi.getWarehouses({}).subscribe(res => {
      if (res.success && res.data) this.warehouses = res.data.items;
      this.loading = false;
    });
  }

  get productOptions() {
    return this.products.map(p => ({ label: p.name, value: p.id }));
  }

  get warehouseOptions() {
    return this.warehouses.map(w => ({ label: w.name, value: w.id }));
  }

  onSubmit() {
    if (this.batchForm.invalid) {
      this.batchForm.markAllAsTouched();
      return;
    }

    this.submitting = true;
    this.inventoryApi.createBatch(this.batchForm.value).subscribe({
      next: (res) => {
        this.submitting = false;
        if (res.success) {
          this.showToastMessage('success', 'Batch created successfully');
          this.saved.emit();
          this.onClose();
        } else {
          this.showToastMessage('error', res.message || 'Failed to create batch');
        }
      },
      error: (err) => {
        this.submitting = false;
        this.showToastMessage('error', 'An error occurred while creating batch');
      }
    });
  }

  onClose() {
    this.batchForm.reset({
      quantity: 0,
      unitCost: 0
    });
    this.close.emit();
  }

  showToastMessage(type: 'success' | 'error', message: string) {
    this.toastType = type;
    this.toastMessage = message;
    this.showToast = true;
    setTimeout(() => this.showToast = false, 3000);
  }
}
