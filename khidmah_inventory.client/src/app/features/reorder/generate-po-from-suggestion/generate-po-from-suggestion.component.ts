import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { FormBuilder, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { ReorderApiService } from '../../../core/services/reorder-api.service';
import { SupplierApiService } from '../../../core/services/supplier-api.service';
import { ReorderSuggestion, ReorderItemDto, GeneratePOFromSuggestionsRequest } from '../../../core/models/reorder.model';
import { ApiResponse } from '../../../core/models/api-response.model';
import { ToastComponent } from '../../../shared/components/toast/toast.component';
import { LoadingSpinnerComponent } from '../../../shared/components/loading-spinner/loading-spinner.component';
import { UnifiedCardComponent } from '../../../shared/components/unified-card/unified-card.component';
import { UnifiedButtonComponent } from '../../../shared/components/unified-button/unified-button.component';
import { Supplier } from '../../../core/models/supplier.model';
import { ContentLoaderComponent } from '../../../shared/components/content-loader/content-loader.component';
import { SkeletonDetailHeaderComponent } from '../../../shared/components/skeleton-detail-header/skeleton-detail-header.component';
import { SkeletonFormComponent } from '../../../shared/components/skeleton-form/skeleton-form.component';
import { SkeletonTableComponent } from '../../../shared/components/skeleton-table/skeleton-table.component';
import { DataTableComponent } from '../../../shared/components/data-table/data-table.component';
import { DataTableColumn, DataTableConfig } from '../../../shared/models/data-table.model';

@Component({
  selector: 'app-generate-po-from-suggestion',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    ReactiveFormsModule,
    ToastComponent,
    LoadingSpinnerComponent,
    UnifiedCardComponent,
    UnifiedButtonComponent,
    ContentLoaderComponent,
    SkeletonDetailHeaderComponent,
    SkeletonFormComponent,
    SkeletonTableComponent,
    DataTableComponent
  ],
  templateUrl: './generate-po-from-suggestion.component.html'
})
export class GeneratePoFromSuggestionComponent implements OnInit {
  loading = true;
  saving = false;
  suggestions: ReorderSuggestion[] = [];
  suppliers: Supplier[] = [];
  form: FormGroup;
  showToast = false;
  toastMessage = '';
  toastType: 'success' | 'error' | 'info' = 'success';

  lineColumns: DataTableColumn<ReorderSuggestion>[] = [
    { key: 'productName', label: 'Product', sortable: false, width: '200px', render: (s) => `${s.productName} (${s.productSKU})` },
    { key: 'warehouseName', label: 'Warehouse', sortable: false, width: '120px' },
    { key: 'suggestedQuantity', label: 'Quantity', sortable: false, width: '90px', type: 'number' },
    { key: 'priority', label: 'Priority', sortable: false, type: 'badge', width: '90px' }
  ];

  lineTableConfig: DataTableConfig = {
    showSearch: false,
    showFilters: false,
    showColumnToggle: false,
    showPagination: false,
    showActions: false,
    showCheckbox: false,
    emptyMessage: 'No lines.'
  };

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private fb: FormBuilder,
    private reorderApi: ReorderApiService,
    private supplierApi: SupplierApiService
  ) {
    this.form = this.fb.group({
      supplierId: [''],
      expectedDeliveryDate: [''],
      notes: ['']
    });
  }

  ngOnInit(): void {
    const idsParam = this.route.snapshot.queryParamMap.get('ids');
    this.supplierApi.getSuppliers({ filterRequest: { pagination: { pageNo: 1, pageSize: 200 } } }).subscribe({
      next: (res: ApiResponse<any>) => {
        if (res.success && res.data?.items) this.suppliers = res.data.items;
      },
      error: () => {}
    });
    if (idsParam) {
      const keys = idsParam.split(',').filter(Boolean);
      this.reorderApi.getSuggestions({}).subscribe({
        next: (res: ApiResponse<ReorderSuggestion[]>) => {
          this.loading = false;
          if (res.success && res.data) {
            this.suggestions = res.data.filter(s => keys.includes(`${s.productId}|${s.warehouseId}`));
            if (this.suggestions.length > 0 && !this.form.get('supplierId')?.value && this.suggestions[0].supplierSuggestions?.length) {
              this.form.patchValue({ supplierId: this.suggestions[0].supplierSuggestions[0].supplierId });
            }
          } else this.suggestions = [];
        },
        error: () => { this.loading = false; this.suggestions = []; }
      });
    } else {
      this.loading = false;
    }
  }

  getPreferredSupplierId(): string | null {
    const first = this.suggestions[0];
    return first?.supplierSuggestions?.length ? first.supplierSuggestions[0].supplierId : null;
  }

  generatePO(): void {
    const supplierId = this.form.get('supplierId')?.value;
    if (!supplierId) {
      this.showToastMsg('Select a supplier', 'error');
      return;
    }
    const items: ReorderItemDto[] = this.suggestions.map(s => ({
      productId: s.productId,
      warehouseId: s.warehouseId,
      quantity: s.suggestedQuantity,
      unitPrice: s.supplierSuggestions?.find(sup => sup.supplierId === supplierId)?.recommendedPrice
        ?? s.supplierSuggestions?.[0]?.lastPurchasePrice
    }));
    const request: GeneratePOFromSuggestionsRequest = {
      items,
      supplierId,
      expectedDeliveryDate: this.form.get('expectedDeliveryDate')?.value || undefined,
      notes: this.form.get('notes')?.value || undefined
    };
    this.saving = true;
    this.reorderApi.generatePurchaseOrder(request).subscribe({
      next: (res: ApiResponse<any>) => {
        this.saving = false;
        if (res.success && res.data) {
          this.showToastMsg('Purchase order created', 'success');
          this.router.navigate(['/purchase-orders', res.data.id]);
        } else this.showToastMsg(res.message || 'Failed to create PO', 'error');
      },
      error: (err) => {
        this.saving = false;
        this.showToastMsg(err.error?.message || 'Error creating PO', 'error');
      }
    });
  }

  showToastMsg(msg: string, type: 'success' | 'error' | 'info'): void {
    this.toastMessage = msg;
    this.toastType = type;
    this.showToast = true;
  }
}
