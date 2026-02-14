import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ProductApiService } from '../../../core/services/product-api.service';
import { CategoryApiService } from '../../../core/services/category-api.service';
import { Product, CreateProductRequest, UpdateProductRequest } from '../../../core/models/product.model';
import { Category } from '../../../core/models/category.model';
import { ToastComponent } from '../../../shared/components/toast/toast.component';
import { LoadingSpinnerComponent } from '../../../shared/components/loading-spinner/loading-spinner.component';
import { IconComponent } from '../../../shared/components/icon/icon.component';
import { PermissionService } from '../../../core/services/permission.service';
import { HasPermissionDirective } from '../../../shared/directives/has-permission.directive';
import { FormFieldComponent } from '../../../shared/components/form-field/form-field.component';
import { UnifiedButtonComponent } from '../../../shared/components/unified-button/unified-button.component';
import { HeaderService } from '../../../core/services/header.service';
import { ExportService } from '../../../core/services/export.service';
import { UnifiedCardComponent } from '../../../shared/components/unified-card/unified-card.component';
import { UnifiedCheckboxComponent } from '../../../shared/components/unified-checkbox/unified-checkbox.component';
import { TabsComponent, TabComponent } from '../../../shared/components/tabs/tabs.component';
import { AiApiService } from '../../../core/services/ai-api.service';
import { PricingApiService } from '../../../core/services/pricing-api.service';
import { ChartComponent } from '../../../shared/components/chart/chart.component';
import { CHART_PRIMARY } from '../../../shared/constants/chart-colors';
import { ActivityFeedPanelComponent } from '../../../shared/components/activity-feed-panel/activity-feed-panel.component';
import { ForecastConfidenceComponent } from '../../intelligence/decision-support/forecast-confidence.component';
import { WhatIfSimulatorComponent } from '../../intelligence/decision-support/what-if-simulator.component';
import { ProductIntelligencePanelComponent } from '../../../shared/components/product-intelligence-panel/product-intelligence-panel.component';
import { PriceOptimization } from '../../../core/models/pricing.model';
import { SignalRService } from '../../../core/services/signalr.service';
import { Subscription } from 'rxjs';
import { ContentLoaderComponent } from '../../../shared/components/content-loader/content-loader.component';
import { SkeletonDetailHeaderComponent } from '../../../shared/components/skeleton-detail-header/skeleton-detail-header.component';
import { SkeletonFormComponent } from '../../../shared/components/skeleton-form/skeleton-form.component';

@Component({
  selector: 'app-product-form',
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
    TabsComponent,
    TabComponent,
    ChartComponent,
    ActivityFeedPanelComponent,
    ForecastConfidenceComponent,
    WhatIfSimulatorComponent,
    ProductIntelligencePanelComponent,
    ContentLoaderComponent,
    SkeletonDetailHeaderComponent,
    SkeletonFormComponent
  ],
  templateUrl: './product-form.component.html'
})
export class ProductFormComponent implements OnInit, OnDestroy {
  product: Product | null = null;
  private signalRSubs = new Subscription();
  categories: Category[] = [];
  loading = false;
  saving = false;
  isEditMode = false;
  isViewMode = false;
  activeTab = 0;
  forecastData: any = null;
  forecastChartData: any = null;
  priceSuggestion: PriceOptimization | null = null;
  loadingPrice = false;
  activityPanelOpen = false;


  formData = {
    name: '',
    description: '',
    sku: '',
    barcode: '',
    categoryId: '',
    brandId: '',
    unitOfMeasureId: '',
    purchasePrice: 0,
    salePrice: 0,
    costPrice: 0,
    minStockLevel: 0,
    maxStockLevel: 0,
    reorderPoint: 0,
    trackQuantity: true,
    trackBatch: false,
    trackExpiry: false,
    weight: 0,
    weightUnit: 'kg',
    length: 0,
    width: 0,
    height: 0,
    dimensionsUnit: 'cm'
  };

  // Mock data - in real app, these would come from API
  brands: { id: string; name: string }[] = [];
  unitOfMeasures: { id: string; name: string; code: string }[] = [
    { id: '1', name: 'Piece', code: 'PCS' },
    { id: '2', name: 'Kilogram', code: 'KG' },
    { id: '3', name: 'Liter', code: 'L' },
    { id: '4', name: 'Meter', code: 'M' }
  ];

  /** Cached for template to avoid new array every change detection */
  categoryOptions: { value: any; label: string }[] = [];
  brandOptions: { value: any; label: string }[] = [];
  unitOfMeasureOptions: { value: any; label: string }[] = [];

  weightUnitOptions = [
    { value: 'kg', label: 'Kilogram (kg)' },
    { value: 'g', label: 'Gram (g)' },
    { value: 'lb', label: 'Pound (lb)' },
    { value: 'oz', label: 'Ounce (oz)' }
  ];

  dimensionsUnitOptions = [
    { value: 'cm', label: 'Centimeter (cm)' },
    { value: 'm', label: 'Meter (m)' },
    { value: 'in', label: 'Inch (in)' },
    { value: 'ft', label: 'Foot (ft)' }
  ];

  showToast = false;
  toastMessage = '';
  toastType: 'success' | 'error' | 'warning' | 'info' = 'success';

  constructor(
    private productApiService: ProductApiService,
    private categoryApiService: CategoryApiService,
    private route: ActivatedRoute,
    public router: Router,
    public permissionService: PermissionService,
    private headerService: HeaderService,
    private exportService: ExportService,
    private aiApiService: AiApiService,
    private pricingApiService: PricingApiService,
    private signalR: SignalRService
  ) {}

  ngOnInit(): void {
    this.updateSelectOptions();
    const productId = this.route.snapshot.paramMap.get('id');
    const url = this.router.url;

    // If we have an ID but URL doesn't end with /edit, we are in View Mode
    this.isEditMode = !!productId && url.includes('/edit');
    this.isViewMode = !!productId && !url.includes('/edit');

    this.headerService.setHeaderInfo({
      title: this.isViewMode ? 'Product Details' : (this.isEditMode ? 'Edit Product' : 'Create Product'),
      description: this.isViewMode ? 'View product details' : (this.isEditMode ? 'Modify existing product details' : 'Add a new product to inventory')
    });

    this.loadCategories();

    if (productId) {
      this.loadProduct(productId);
      this.setupRealTimeProductUpdates(productId);
    }
  }

  private setupRealTimeProductUpdates(productId: string): void {
    this.signalRSubs.add(
      this.signalR.getProductUpdated().subscribe(p => {
        if (p.entityId === productId) {
          this.loadProduct(productId);
          this.showToastMessage('info', 'Product updated in real-time');
        }
      })
    );
    this.signalRSubs.add(
      this.signalR.getStockChanged().subscribe(p => {
        if (p.entityId === productId || (p.payload && (p.payload as any).productId === productId)) {
          this.loadProduct(productId);
        }
      })
    );
    this.signalRSubs.add(
      this.signalR.getCommentAdded().subscribe(p => {
        if (p.entityType === 'Product' && p.entityId === productId) {
          this.loadProduct(productId);
        }
      })
    );
  }

  ngOnDestroy(): void {
    this.signalRSubs.unsubscribe();
  }

  loadProduct(id: string): void {
    this.loading = true;
    this.productApiService.getProduct(id).subscribe({
      next: (response) => {
        if (response.success && response.data) {
          this.product = response.data;
          this.formData = {
            name: response.data.name,
            description: response.data.description || '',
            sku: response.data.sku,
            barcode: response.data.barcode || '',
            categoryId: response.data.categoryId || '',
            brandId: response.data.brandId || '',
            unitOfMeasureId: response.data.unitOfMeasureId,
            purchasePrice: response.data.purchasePrice,
            salePrice: response.data.salePrice,
            costPrice: response.data.costPrice || 0,
            minStockLevel: response.data.minStockLevel || 0,
            maxStockLevel: response.data.maxStockLevel || 0,
            reorderPoint: response.data.reorderPoint || 0,
            trackQuantity: response.data.trackQuantity,
            trackBatch: response.data.trackBatch,
            trackExpiry: response.data.trackExpiry,
            weight: response.data.weight,
            weightUnit: response.data.weightUnit || 'kg',
            length: response.data.length || 0,
            width: response.data.width || 0,
            height: response.data.height || 0,
            dimensionsUnit: response.data.dimensionsUnit || 'cm'
          };
        } else {
          this.showToastMessage('error', response.message || 'Failed to load product');
        }
        this.loading = false;

        if (this.isViewMode) {
          this.loadForecast();
          this.loadPriceSuggestion();
        }
        if (this.isEditMode && this.product?.id) this.loadPriceSuggestion();
      },
      error: () => {
        this.showToastMessage('error', 'Error loading product');
        this.loading = false;
      }
    });
  }

  loadForecast() {
    if (!this.product) return;
    this.aiApiService.getDemandForecast(this.product.id).subscribe(res => {
      if (res.success) {
        this.forecastData = res.data;
        this.prepareForecastChart();
      }
    });
  }

  loadPriceSuggestion(): void {
    const id = this.product?.id || this.route.snapshot.paramMap.get('id');
    if (!id) return;
    this.loadingPrice = true;
    this.pricingApiService.getSuggestions([id]).subscribe({
      next: (res) => {
        this.loadingPrice = false;
        if (res.success && res.data?.length) this.priceSuggestion = res.data[0];
        else this.priceSuggestion = null;
      },
      error: () => { this.loadingPrice = false; this.priceSuggestion = null; }
    });
  }

  applySuggestedPrice(): void {
    if (!this.priceSuggestion?.recommendedPrice || !this.product?.id) return;
    this.formData.salePrice = this.priceSuggestion.recommendedPrice;
    this.showToastMessage('success', 'Sale price updated. Click Save to persist.');
  }

  prepareForecastChart() {
    if (!this.forecastData) return;

    this.forecastChartData = {
      labels: this.forecastData.forecastDates.map((d: string) => new Date(d).toLocaleDateString()),
      datasets: [
        {
          label: 'Predicted Demand',
          data: this.forecastData.forecastQuantities,
          borderColor: CHART_PRIMARY,
          backgroundColor: CHART_PRIMARY,
          fill: true,
          tension: 0.4
        }
      ]
    };
  }

  loadCategories(): void {
    this.categoryApiService.getCategories({}).subscribe({
      next: (response) => {
        if (response.success && response.data) {
          this.categories = response.data.items;
          this.updateSelectOptions();
        }
      }
    });
  }

  private updateSelectOptions(): void {
    this.categoryOptions = [
      { value: '', label: 'None' },
      ...this.categories.map(c => ({ value: c.id, label: c.name }))
    ];
    this.brandOptions = [
      { value: '', label: 'None' },
      ...this.brands.map(b => ({ value: b.id, label: b.name }))
    ];
    this.unitOfMeasureOptions = [
      { value: '', label: 'Select Unit' },
      ...this.unitOfMeasures.map(u => ({ value: u.id, label: `${u.name} (${u.code})` }))
    ];
  }

  save(): void {
    if (!this.formData.name.trim() || !this.formData.sku.trim() || !this.formData.unitOfMeasureId) {
      this.showToastMessage('error', 'Name, SKU, and Unit of Measure are required');
      return;
    }

    this.saving = true;

    if (this.isEditMode && this.product) {
      const updateRequest: UpdateProductRequest = {
        name: this.formData.name,
        description: this.formData.description || undefined,
        sku: this.formData.sku,
        barcode: this.formData.barcode || undefined,
        categoryId: this.formData.categoryId || undefined,
        brandId: this.formData.brandId || undefined,
        unitOfMeasureId: this.formData.unitOfMeasureId,
        purchasePrice: this.formData.purchasePrice,
        salePrice: this.formData.salePrice,
        costPrice: this.formData.costPrice || undefined,
        minStockLevel: this.formData.minStockLevel || undefined,
        maxStockLevel: this.formData.maxStockLevel || undefined,
        reorderPoint: this.formData.reorderPoint || undefined,
        trackQuantity: this.formData.trackQuantity,
        trackBatch: this.formData.trackBatch,
        trackExpiry: this.formData.trackExpiry,
        weight: this.formData.weight,
        weightUnit: this.formData.weightUnit,
        length: this.formData.length || undefined,
        width: this.formData.width || undefined,
        height: this.formData.height || undefined,
        dimensionsUnit: this.formData.dimensionsUnit
      };

      this.productApiService.updateProduct(this.product.id, updateRequest).subscribe({
        next: (response) => {
          if (response.success) {
            this.showToastMessage('success', 'Product updated successfully');
            setTimeout(() => {
              this.router.navigate(['/products']);
            }, 1500);
          } else {
            this.showToastMessage('error', response.message || 'Failed to update product');
          }
          this.saving = false;
        },
        error: () => {
          this.showToastMessage('error', 'Error updating product');
          this.saving = false;
        }
      });
    } else {
      const createRequest: CreateProductRequest = {
        name: this.formData.name,
        description: this.formData.description || undefined,
        sku: this.formData.sku,
        barcode: this.formData.barcode || undefined,
        categoryId: this.formData.categoryId || undefined,
        brandId: this.formData.brandId || undefined,
        unitOfMeasureId: this.formData.unitOfMeasureId,
        purchasePrice: this.formData.purchasePrice,
        salePrice: this.formData.salePrice,
        costPrice: this.formData.costPrice || undefined,
        minStockLevel: this.formData.minStockLevel || undefined,
        maxStockLevel: this.formData.maxStockLevel || undefined,
        reorderPoint: this.formData.reorderPoint || undefined,
        trackQuantity: this.formData.trackQuantity,
        trackBatch: this.formData.trackBatch,
        trackExpiry: this.formData.trackExpiry,
        weight: this.formData.weight,
        weightUnit: this.formData.weightUnit,
        length: this.formData.length || undefined,
        width: this.formData.width || undefined,
        height: this.formData.height || undefined,
        dimensionsUnit: this.formData.dimensionsUnit
      };

      this.productApiService.createProduct(createRequest).subscribe({
        next: (response) => {
          if (response.success) {
            this.showToastMessage('success', 'Product created successfully');
            setTimeout(() => {
              this.router.navigate(['/products']);
            }, 1500);
          } else {
            this.showToastMessage('error', response.message || 'Failed to create product');
          }
          this.saving = false;
        },
        error: () => {
          this.showToastMessage('error', 'Error creating product');
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
    if (!this.product) return;

    const details = [
      { label: 'Name', value: this.formData.name },
      { label: 'SKU', value: this.formData.sku },
      { label: 'Barcode', value: this.formData.barcode },
      { label: 'Category', value: this.categories.find(c => c.id === this.formData.categoryId)?.name },
      { label: 'Brand', value: this.brands.find(b => b.id === this.formData.brandId)?.name }, // Simplified
      { label: 'Unit Of Measure', value: this.unitOfMeasures.find(u => u.id === this.formData.unitOfMeasureId)?.name },
      { label: 'Purchase Price', value: this.formData.purchasePrice },
      { label: 'Sale Price', value: this.formData.salePrice },
      { label: 'Cost Price', value: this.formData.costPrice },
      { label: 'Min Stock', value: this.formData.minStockLevel },
      { label: 'Max Stock', value: this.formData.maxStockLevel },
      { label: 'Reorder Point', value: this.formData.reorderPoint },
      { label: 'Weight', value: `${this.formData.weight} ${this.formData.weightUnit}` },
      { label: 'Dimensions', value: `${this.formData.length} x ${this.formData.width} x ${this.formData.height} ${this.formData.dimensionsUnit}` }
    ];

    try {
      await this.exportService.exportEntityDetails(
        details,
        `Product Details: ${this.formData.name}`,
        `product_details_${this.formData.sku}`
      );
      this.showToastMessage('success', 'PDF exported successfully');
    } catch (error) {
      console.error(error);
      this.showToastMessage('error', 'Failed to export PDF');
    }
  }

}
