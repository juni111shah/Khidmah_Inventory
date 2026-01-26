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
  selector: 'app-assign-serial-numbers',
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
  templateUrl: './assign-serial-numbers.component.html'
})
export class AssignSerialNumbersComponent implements OnInit {
  @Input() show: boolean = false;
  @Output() close = new EventEmitter<void>();
  @Output() saved = new EventEmitter<void>();

  serialForm: FormGroup;
  products: any[] = [];
  warehouses: any[] = [];
  batches: any[] = [];
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
    this.serialForm = this.fb.group({
      productId: ['', Validators.required],
      warehouseId: ['', Validators.required],
      serialNumberValue: ['', Validators.required],
      batchId: [null],
      manufactureDate: [''],
      expiryDate: [''],
      warrantyExpiryDate: [''],
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
    });
    this.inventoryApi.getBatches({}).subscribe(res => {
      if (res.success && res.data) this.batches = res.data.items;
      this.loading = false;
    });
  }

  get productOptions() {
    return this.products.map(p => ({ label: p.name, value: p.id }));
  }

  get warehouseOptions() {
    return this.warehouses.map(w => ({ label: w.name, value: w.id }));
  }

  get batchOptions() {
    return this.batches.map(b => ({ label: b.batchNumber, value: b.id }));
  }

  onSubmit() {
    if (this.serialForm.invalid) {
      this.serialForm.markAllAsTouched();
      return;
    }

    this.submitting = true;
    this.inventoryApi.createSerialNumber(this.serialForm.value).subscribe({
      next: (res) => {
        this.submitting = false;
        if (res.success) {
          this.showToastMessage('success', 'Serial number assigned successfully');
          this.saved.emit();
          this.onClose();
        } else {
          this.showToastMessage('error', res.message || 'Failed to assign serial number');
        }
      },
      error: (err) => {
        this.submitting = false;
        this.showToastMessage('error', 'An error occurred while assigning serial number');
      }
    });
  }

  onClose() {
    this.serialForm.reset();
    this.close.emit();
  }

  showToastMessage(type: 'success' | 'error', message: string) {
    this.toastType = type;
    this.toastMessage = message;
    this.showToast = true;
    setTimeout(() => this.showToast = false, 3000);
  }
}
