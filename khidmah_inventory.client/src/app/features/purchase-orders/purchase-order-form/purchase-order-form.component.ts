import { Component, HostListener, OnDestroy, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { PurchaseOrderApiService } from '../../../core/services/purchase-order-api.service';
import { SupplierApiService } from '../../../core/services/supplier-api.service';
import { PurchaseOrder, CreatePurchaseOrderRequest, UpdatePurchaseOrderRequest } from '../../../core/models/purchase-order.model';
import { Supplier } from '../../../core/models/supplier.model';
import { ToastComponent } from '../../../shared/components/toast/toast.component';
import { LoadingSpinnerComponent } from '../../../shared/components/loading-spinner/loading-spinner.component';
import { IconComponent } from '../../../shared/components/icon/icon.component';
import { HasPermissionDirective } from '../../../shared/directives/has-permission.directive';
import { FormFieldComponent } from '../../../shared/components/form-field/form-field.component';
import { UnifiedButtonComponent } from '../../../shared/components/unified-button/unified-button.component';
import { UnifiedCardComponent } from '../../../shared/components/unified-card/unified-card.component';
import { ActivityFeedPanelComponent } from '../../../shared/components/activity-feed-panel/activity-feed-panel.component';
import { HeaderService } from '../../../core/services/header.service';
import { ContentLoaderComponent } from '../../../shared/components/content-loader/content-loader.component';
import { SkeletonDetailHeaderComponent } from '../../../shared/components/skeleton-detail-header/skeleton-detail-header.component';
import { SkeletonFormComponent } from '../../../shared/components/skeleton-form/skeleton-form.component';
import { SkeletonTableComponent } from '../../../shared/components/skeleton-table/skeleton-table.component';
import { ProductApiService } from '../../../core/services/product-api.service';
import { InventoryApiService } from '../../../core/services/inventory-api.service';
import { Product } from '../../../core/models/product.model';
import { SearchMode } from '../../../core/models/api-response.model';
import { Subject, Subscription, debounceTime } from 'rxjs';
import { AuthService } from '../../../core/services/auth.service';
import { DocumentApiService } from '../../../core/services/document-api.service';
import { RealtimeSyncService } from '../../../core/services/realtime-sync.service';

interface PurchaseOrderLineVm {
  uid: string;
  productId: string;
  productName: string;
  sku: string;
  unit: string;
  barcode: string;
  availableStock: number | null;
  quantity: number;
  unitPrice: number;
  discountPercent: number;
  taxPercent: number;
  notes: string;
  subTotal: number;
  discountAmount: number;
  taxAmount: number;
  lineTotal: number;
}

interface TimelineEventVm {
  at: string;
  by: string;
  action: string;
  detail: string;
}

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
    UnifiedCardComponent,
    ActivityFeedPanelComponent,
    ContentLoaderComponent,
    SkeletonDetailHeaderComponent,
    SkeletonFormComponent,
    SkeletonTableComponent
  ],
  templateUrl: './purchase-order-form.component.html',
  styleUrls: ['./purchase-order-form.component.scss']
})
export class PurchaseOrderFormComponent implements OnInit, OnDestroy {
  purchaseOrder: PurchaseOrder | null = null;
  suppliers: Supplier[] = [];
  products: Product[] = [];
  productById = new Map<string, Product>();
  lines: PurchaseOrderLineVm[] = [];
  timeline: TimelineEventVm[] = [];
  loading = false;
  saving = false;
  isEditMode = false;
  bulkPasteText = '';
  barcodeInput = '';
  defaultTaxPercent = 15;
  abnormalDiscountThreshold = 35;
  currentOrderId: string | null = null;
  private autoSave$ = new Subject<void>();
  private subs = new Subscription();
  workflowStates = ['Draft', 'Submitted', 'Approved', 'Rejected', 'Posted'];
  workflowStateOptions = this.workflowStates.map(state => ({ value: state, label: state }));

  formData = {
    supplierId: '',
    orderDate: new Date().toISOString().split('T')[0],
    expectedDeliveryDate: '',
    notes: '',
    termsAndConditions: '',
    workflowStatus: 'Draft'
  };

  showToast = false;
  toastMessage = '';
  toastType: 'success' | 'error' | 'warning' | 'info' = 'success';
  activityPanelOpen = false;

  constructor(
    private purchaseOrderApiService: PurchaseOrderApiService,
    private supplierApiService: SupplierApiService,
    private productApiService: ProductApiService,
    private inventoryApiService: InventoryApiService,
    private documentApiService: DocumentApiService,
    private realtimeSync: RealtimeSyncService,
    private authService: AuthService,
    private route: ActivatedRoute,
    public router: Router,
    private headerService: HeaderService
  ) {}

  ngOnInit(): void {
    const orderId = this.route.snapshot.paramMap.get('id');
    this.currentOrderId = orderId;
    this.isEditMode = !!orderId;

    this.headerService.setHeaderInfo({
      title: this.isEditMode ? 'Edit Purchase Order' : 'New Purchase Order',
      description: this.isEditMode ? 'Modify purchase order details' : 'Create a new purchase order'
    });

    this.lines = [this.createEmptyLine()];
    this.loadSuppliers();
    this.loadProducts();
    this.setupAutosave();

    if (this.isEditMode && orderId) {
      this.loadPurchaseOrder(orderId);
      this.subs.add(
        this.realtimeSync.watchEntity('PurchaseOrder', orderId).subscribe(() => {
          if (!this.saving && !this.loading) {
            this.showToastMessage('warning', 'This purchase order was updated by another user. Data has been refreshed.');
            this.loadPurchaseOrder(orderId);
          }
        })
      );
    } else {
      this.restoreDraft();
    }
  }

  ngOnDestroy(): void {
    this.subs.unsubscribe();
  }

  @HostListener('window:beforeunload')
  onBeforeUnload(): void {
    this.persistDraft();
  }

  @HostListener('document:keydown', ['$event'])
  onHotkey(event: KeyboardEvent): void {
    if (event.ctrlKey && event.key.toLowerCase() === 's') {
      event.preventDefault();
      this.save('Draft');
      return;
    }

    if (event.ctrlKey && event.key === 'Enter') {
      event.preventDefault();
      this.save('Submitted');
      return;
    }

    if (event.altKey && event.key.toLowerCase() === 'n') {
      event.preventDefault();
      this.addLine();
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

  loadProducts(): void {
    this.productApiService.getProducts({
      filterRequest: { pagination: { pageNo: 1, pageSize: 500 } }
    }).subscribe({
      next: (response) => {
        if (response.success && response.data) {
          this.products = response.data.items;
          this.productById = new Map(this.products.map(p => [p.id, p]));
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
            termsAndConditions: response.data.termsAndConditions || '',
            workflowStatus: response.data.status || 'Draft'
          };

          this.lines = response.data.items?.length
            ? response.data.items.map(item => this.recalculateLine({
                uid: this.generateLineUid(),
                productId: item.productId,
                productName: item.productName,
                sku: item.productSKU,
                unit: '',
                barcode: '',
                availableStock: null,
                quantity: item.quantity,
                unitPrice: item.unitPrice,
                discountPercent: item.discountPercent || 0,
                taxPercent: item.taxPercent || this.defaultTaxPercent,
                notes: item.notes || '',
                subTotal: 0,
                discountAmount: 0,
                taxAmount: 0,
                lineTotal: 0
              }))
            : [this.createEmptyLine()];

          this.timeline = [
            { at: response.data.createdAt, by: 'System', action: 'Created', detail: `Order ${response.data.orderNumber} created` }
          ];
          if (response.data.updatedAt) {
            this.timeline.push({ at: response.data.updatedAt, by: 'System', action: 'Edited', detail: 'Order details updated' });
          }
          this.timeline.push({ at: new Date().toISOString(), by: 'System', action: 'Status', detail: `Current state: ${this.formData.workflowStatus}` });
          this.loadStocksForLines();
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

  save(nextStatus?: string): void {
    if (nextStatus) {
      this.formData.workflowStatus = nextStatus;
    }
    if (this.isEditLocked) {
      this.showToastMessage('warning', 'Approved or posted orders are locked from editing.');
      return;
    }
    if (!this.formData.supplierId) {
      this.showToastMessage('error', 'Supplier is required');
      return;
    }
    if (!this.formData.orderDate) {
      this.showToastMessage('error', 'Order date is required');
      return;
    }
    if (!this.lines.some(line => line.productId && line.quantity > 0)) {
      this.showToastMessage('error', 'At least one line item is required');
      return;
    }
    if (!this.validateBusinessRules()) {
      return;
    }

    this.saving = true;

    const request: CreatePurchaseOrderRequest = {
      supplierId: this.formData.supplierId,
      orderDate: new Date(this.formData.orderDate).toISOString(),
      expectedDeliveryDate: this.formData.expectedDeliveryDate ? new Date(this.formData.expectedDeliveryDate).toISOString() : undefined,
      notes: this.formData.notes || undefined,
      termsAndConditions: this.formData.termsAndConditions || undefined,
      status: this.formData.workflowStatus,
      items: this.lines
        .filter(line => line.productId && line.quantity > 0)
        .map(line => ({
          productId: line.productId,
          quantity: line.quantity,
          unitPrice: line.unitPrice,
          discountPercent: line.discountPercent || 0,
          taxPercent: line.taxPercent || 0,
          notes: line.notes || undefined
        }))
    };

    if (this.isEditMode && this.purchaseOrder) {
      this.purchaseOrderApiService.updatePurchaseOrder(this.purchaseOrder.id, request as UpdatePurchaseOrderRequest).subscribe({
        next: (response) => {
          if (response.success) {
            this.recordTimeline('Updated', `Order updated with state ${this.formData.workflowStatus}`);
            this.persistDraft();
            this.showToastMessage('success', 'Purchase order updated successfully');
          } else {
            this.showToastMessage('error', response.message || 'Failed to update purchase order');
          }
          this.saving = false;
        },
        error: (error) => {
          this.showToastMessage('error', this.extractServerError(error, 'Error updating purchase order'));
          this.saving = false;
        }
      });
    } else {
      this.purchaseOrderApiService.createPurchaseOrder(request).subscribe({
        next: (response) => {
          if (response.success) {
            this.rememberRecentSelections();
            this.clearDraft();
            this.showToastMessage('success', 'Purchase Order created successfully');
            setTimeout(() => {
              this.router.navigate(['/purchase-orders']);
            }, 1500);
          } else {
            this.showToastMessage('error', response.message || 'Failed to create purchase order');
          }
          this.saving = false;
        },
        error: (error) => {
          this.showToastMessage('error', this.extractServerError(error, 'Error creating purchase order'));
          this.saving = false;
        }
      });
    }
  }

  onLineValueChange(line: PurchaseOrderLineVm): void {
    this.recalculateLine(line);
    this.queueAutosave();
  }

  addLine(copyFrom?: PurchaseOrderLineVm): void {
    if (this.isEditLocked) return;
    const line = copyFrom
      ? this.recalculateLine({
          ...copyFrom,
          uid: this.generateLineUid()
        })
      : this.createEmptyLine();
    this.lines = [...this.lines, line];
    this.queueAutosave();
  }

  duplicateLine(index: number): void {
    const source = this.lines[index];
    if (!source) return;
    this.addLine(source);
  }

  removeLine(index: number): void {
    if (this.isEditLocked) return;
    this.lines = this.lines.filter((_, idx) => idx !== index);
    if (!this.lines.length) {
      this.lines = [this.createEmptyLine()];
    }
    this.queueAutosave();
  }

  onProductSelected(lineIndex: number, productId: string): void {
    const line = this.lines[lineIndex];
    if (!line) return;
    const product = this.productById.get(productId);
    if (!product) return;

    const rememberedPrice = this.getRememberedPrice(product.id) ?? product.purchasePrice ?? 0;
    line.productId = product.id;
    line.productName = product.name;
    line.sku = product.sku;
    line.unit = product.unitOfMeasureCode || product.unitOfMeasureName || '';
    line.barcode = product.barcode || '';
    line.unitPrice = rememberedPrice;
    line.taxPercent = line.taxPercent || this.defaultTaxPercent;

    this.recalculateLine(line);
    this.loadStockForProduct(product.id);
    this.focusQuantity(line.uid);
    this.queueAutosave();
  }

  onBarcodeEnter(): void {
    const value = this.barcodeInput.trim();
    if (!value) return;

    const localMatch = this.products.find(
      p => p.barcode?.toLowerCase() === value.toLowerCase() || p.sku.toLowerCase() === value.toLowerCase()
    );

    if (localMatch) {
      const lastLine = this.lines[this.lines.length - 1];
      if (lastLine && !lastLine.productId) {
        this.onProductSelected(this.lines.length - 1, localMatch.id);
      } else {
        const newLine = this.createEmptyLine();
        this.lines = [...this.lines, newLine];
        this.onProductSelected(this.lines.length - 1, localMatch.id);
      }
      this.barcodeInput = '';
      return;
    }

    this.productApiService.getProducts({
      filterRequest: {
        pagination: { pageNo: 1, pageSize: 1 },
        search: {
          term: value,
          searchFields: ['barcode', 'sku'],
          mode: SearchMode.ExactMatch,
          isCaseSensitive: false
        }
      }
    }).subscribe({
      next: (res) => {
        if (res.success && res.data?.items?.length) {
          const product = res.data.items[0];
          const newLine = this.createEmptyLine();
          this.lines = [...this.lines, newLine];
          this.onProductSelected(this.lines.length - 1, product.id);
          this.barcodeInput = '';
        } else {
          this.showToastMessage('warning', 'No product found for scanned barcode');
        }
      }
    });
  }

  applyBulkPaste(): void {
    const raw = this.bulkPasteText.trim();
    if (!raw) return;

    const rows = raw.split('\n');
    const parsed: PurchaseOrderLineVm[] = [];
    const failures: string[] = [];

    for (const row of rows) {
      const cols = row.split(',').map(v => v.trim());
      const [skuOrBarcode, qtyRaw, priceRaw, discountRaw, taxRaw] = cols;
      const product = this.products.find(p =>
        p.sku.toLowerCase() === (skuOrBarcode || '').toLowerCase() ||
        (p.barcode || '').toLowerCase() === (skuOrBarcode || '').toLowerCase()
      );

      if (!product) {
        failures.push(skuOrBarcode || '(empty)');
        continue;
      }

      parsed.push(this.recalculateLine({
        uid: this.generateLineUid(),
        productId: product.id,
        productName: product.name,
        sku: product.sku,
        unit: product.unitOfMeasureCode || product.unitOfMeasureName || '',
        barcode: product.barcode || '',
        availableStock: null,
        quantity: Number(qtyRaw || 1) || 1,
        unitPrice: Number(priceRaw || product.purchasePrice || 0) || 0,
        discountPercent: Number(discountRaw || 0) || 0,
        taxPercent: Number(taxRaw || this.defaultTaxPercent) || this.defaultTaxPercent,
        notes: '',
        subTotal: 0,
        discountAmount: 0,
        taxAmount: 0,
        lineTotal: 0
      }));
    }

    if (parsed.length) {
      this.lines = [...this.lines.filter(l => l.productId), ...parsed];
      this.queueAutosave();
      this.loadStocksForLines();
    }

    this.bulkPasteText = '';

    if (failures.length) {
      this.showToastMessage('warning', `Some items were skipped (not found): ${failures.slice(0, 5).join(', ')}`);
    } else {
      this.showToastMessage('success', `${parsed.length} line(s) imported`);
    }
  }

  clearLines(): void {
    this.lines = [this.createEmptyLine()];
    this.queueAutosave();
  }

  get totals(): { subTotal: number; discount: number; tax: number; grandTotal: number } {
    return this.lines.reduce((acc, line) => {
      acc.subTotal += line.subTotal;
      acc.discount += line.discountAmount;
      acc.tax += line.taxAmount;
      acc.grandTotal += line.lineTotal;
      return acc;
    }, { subTotal: 0, discount: 0, tax: 0, grandTotal: 0 });
  }

  get isEditLocked(): boolean {
    return this.isEditMode && (this.formData.workflowStatus === 'Approved' || this.formData.workflowStatus === 'Posted');
  }

  transitionTo(status: string): void {
    if (!this.workflowStates.includes(status)) return;
    this.save(status);
  }

  cloneAsNew(): void {
    const payload = {
      formData: { ...this.formData, workflowStatus: 'Draft' },
      lines: this.lines.map(l => ({ ...l, uid: this.generateLineUid() }))
    };
    localStorage.setItem('purchase_order_clone_payload', JSON.stringify(payload));
    this.router.navigate(['/purchase-orders/new']);
  }

  convertToGrn(): void {
    if (!this.purchaseOrder?.id) return;
    this.router.navigate(['/inventory/transfer'], {
      queryParams: { source: 'purchase-order', sourceId: this.purchaseOrder.id }
    });
    this.showToastMessage('info', 'Purchase order context sent to stock transfer for GRN processing.');
  }

  downloadPurchaseOrder(): void {
    if (!this.purchaseOrder?.id) return;
    this.documentApiService.generatePurchaseOrderPdf(this.purchaseOrder.id).subscribe({
      next: (blob) => {
        const url = window.URL.createObjectURL(blob);
        window.open(url, '_blank');
        setTimeout(() => window.URL.revokeObjectURL(url), 5000);
      },
      error: () => {
        this.showToastMessage('error', 'Failed to generate purchase order document');
      }
    });
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

  private setupAutosave(): void {
    this.subs.add(
      this.autoSave$.pipe(debounceTime(700)).subscribe(() => {
        this.persistDraft();
      })
    );

    const clonePayload = localStorage.getItem('purchase_order_clone_payload');
    if (!this.isEditMode && clonePayload) {
      try {
        const parsed = JSON.parse(clonePayload);
        this.formData = { ...this.formData, ...parsed.formData, workflowStatus: 'Draft' };
        this.lines = (parsed.lines || []).map((line: PurchaseOrderLineVm) => this.recalculateLine({ ...line, uid: this.generateLineUid() }));
      } catch {
        // ignore malformed clone payload
      }
      localStorage.removeItem('purchase_order_clone_payload');
    }
  }

  queueAutosave(): void {
    if (!this.loading && !this.saving) {
      this.autoSave$.next();
    }
  }

  private validateBusinessRules(): boolean {
    const hasAbnormalDiscount = this.lines.some(line => line.discountPercent > this.abnormalDiscountThreshold);
    if (hasAbnormalDiscount) {
      const proceed = window.confirm(`Some lines have discount above ${this.abnormalDiscountThreshold}%. Continue?`);
      if (!proceed) return false;
    }

    const hasHighCost = this.lines.some(line => {
      const p = this.productById.get(line.productId);
      if (!p) return false;
      return p.salePrice > 0 && line.unitPrice > p.salePrice;
    });
    if (hasHighCost) {
      const proceed = window.confirm('Some line prices exceed current sale price and may reduce margin. Continue?');
      if (!proceed) return false;
    }

    return true;
  }

  private createEmptyLine(): PurchaseOrderLineVm {
    return {
      uid: this.generateLineUid(),
      productId: '',
      productName: '',
      sku: '',
      unit: '',
      barcode: '',
      availableStock: null,
      quantity: 1,
      unitPrice: 0,
      discountPercent: 0,
      taxPercent: this.defaultTaxPercent,
      notes: '',
      subTotal: 0,
      discountAmount: 0,
      taxAmount: 0,
      lineTotal: 0
    };
  }

  private recalculateLine(line: PurchaseOrderLineVm): PurchaseOrderLineVm {
    line.quantity = Number(line.quantity || 0);
    line.unitPrice = Number(line.unitPrice || 0);
    line.discountPercent = Number(line.discountPercent || 0);
    line.taxPercent = Number(line.taxPercent || 0);

    const subTotal = line.quantity * line.unitPrice;
    const discountAmount = subTotal * (line.discountPercent / 100);
    const taxable = subTotal - discountAmount;
    const taxAmount = taxable * (line.taxPercent / 100);
    const lineTotal = taxable + taxAmount;

    line.subTotal = subTotal;
    line.discountAmount = discountAmount;
    line.taxAmount = taxAmount;
    line.lineTotal = lineTotal;
    return line;
  }

  private loadStocksForLines(): void {
    const productIds = [...new Set(this.lines.map(line => line.productId).filter(Boolean))];
    productIds.forEach(productId => this.loadStockForProduct(productId));
  }

  private loadStockForProduct(productId: string): void {
    this.inventoryApiService.getStockLevels({
      productId,
      filterRequest: { pagination: { pageNo: 1, pageSize: 50 } }
    }).subscribe({
      next: (res) => {
        const totalAvailable = (res.data?.items || []).reduce((sum, item) => sum + (item.availableQuantity || 0), 0);
        this.lines.forEach(line => {
          if (line.productId === productId) {
            line.availableStock = totalAvailable;
          }
        });
      }
    });
  }

  private persistDraft(): void {
    const payload = {
      formData: this.formData,
      lines: this.lines,
      timeline: this.timeline,
      at: new Date().toISOString()
    };
    localStorage.setItem(this.draftKey, JSON.stringify(payload));
  }

  private restoreDraft(): void {
    const raw = localStorage.getItem(this.draftKey);
    if (!raw) return;
    try {
      const draft = JSON.parse(raw);
      this.formData = { ...this.formData, ...(draft.formData || {}) };
      const draftLines = (draft.lines || []) as PurchaseOrderLineVm[];
      if (draftLines.length) {
        this.lines = draftLines.map(line => this.recalculateLine({ ...line, uid: this.generateLineUid() }));
      }
      this.timeline = draft.timeline || [];
      this.showToastMessage('info', 'Draft restored');
      this.loadStocksForLines();
    } catch {
      // ignore malformed draft
    }
  }

  private clearDraft(): void {
    localStorage.removeItem(this.draftKey);
  }

  private rememberRecentSelections(): void {
    const priceMap: Record<string, number> = {};
    this.lines.forEach(line => {
      if (line.productId) {
        priceMap[line.productId] = line.unitPrice;
      }
    });
    localStorage.setItem('po_recent_prices', JSON.stringify(priceMap));
  }

  private getRememberedPrice(productId: string): number | null {
    try {
      const raw = localStorage.getItem('po_recent_prices');
      if (!raw) return null;
      const map = JSON.parse(raw) as Record<string, number>;
      return map[productId] ?? null;
    } catch {
      return null;
    }
  }

  private recordTimeline(action: string, detail: string): void {
    const user = this.authService.getCurrentUser();
    const by = user ? `${user.firstName} ${user.lastName}`.trim() : 'Current user';
    this.timeline = [
      { at: new Date().toISOString(), by: by || 'Current user', action, detail },
      ...this.timeline
    ];
  }

  private focusQuantity(uid: string): void {
    setTimeout(() => {
      const el = document.querySelector<HTMLInputElement>(`#qty-${uid}`);
      el?.focus();
      el?.select();
    }, 0);
  }

  private extractServerError(error: any, fallback: string): string {
    const err = error?.error;
    if (!err) return fallback;
    if (Array.isArray(err.errors) && err.errors.length) {
      return err.errors.join(', ');
    }
    return err.message || fallback;
  }

  private generateLineUid(): string {
    return `line-${Math.random().toString(36).slice(2, 10)}`;
  }

  private get draftKey(): string {
    return this.isEditMode && this.currentOrderId
      ? `purchase_order_draft_${this.currentOrderId}`
      : 'purchase_order_draft_new';
  }
}
