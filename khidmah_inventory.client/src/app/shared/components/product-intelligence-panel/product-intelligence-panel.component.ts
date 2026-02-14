import { Component, Input, OnChanges, SimpleChanges, ChangeDetectionStrategy, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { IntelligenceApiService } from '../../../core/services/intelligence-api.service';
import { ProductIntelligence } from '../../../core/models/intelligence.model';
import { UnifiedCardComponent } from '../unified-card/unified-card.component';
import { LoadingSpinnerComponent } from '../loading-spinner/loading-spinner.component';
import { SeverityBadgeComponent } from '../severity-badge/severity-badge.component';
import { TrendArrowComponent } from '../trend-arrow/trend-arrow.component';
import { UnifiedButtonComponent } from '../unified-button/unified-button.component';

@Component({
  selector: 'app-product-intelligence-panel',
  standalone: true,
  imports: [
    CommonModule,
    UnifiedCardComponent,
    LoadingSpinnerComponent,
    SeverityBadgeComponent,
    TrendArrowComponent,
    UnifiedButtonComponent
  ],
  templateUrl: './product-intelligence-panel.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ProductIntelligencePanelComponent implements OnChanges {
  @Input() productId: string | null = null;

  loading = false;
  error: string | null = null;
  data: ProductIntelligence | null = null;
  /** Cached for template to avoid method call every CD */
  marginTrend: 'Up' | 'Down' | 'Stable' = 'Stable';

  constructor(
    private intelligenceApi: IntelligenceApiService,
    private router: Router,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['productId']) {
      this.load();
    }
  }

  load(): void {
    if (!this.productId) {
      this.data = null;
      this.error = null;
      this.marginTrend = 'Stable';
      this.cdr.markForCheck();
      return;
    }
    this.loading = true;
    this.error = null;
    this.data = null;
    this.marginTrend = 'Stable';
    this.cdr.markForCheck();
    this.intelligenceApi.getProductIntelligence(this.productId, 30).subscribe({
      next: (res) => {
        this.loading = false;
        if (res.success && res.data) {
          this.data = res.data;
          this.marginTrend = this.computeMarginTrend(this.data.marginTrend);
        } else {
          this.error = res.message || 'Failed to load intelligence data';
        }
        this.cdr.markForCheck();
      },
      error: (err) => {
        this.loading = false;
        this.error = err?.message || 'Error loading intelligence data';
        this.cdr.markForCheck();
      }
    });
  }

  goReorder(): void {
    if (this.productId) {
      this.router.navigate(['/inventory', 'reorder'], { queryParams: { productId: this.productId } });
    }
  }

  goInventory(): void {
    if (this.productId) {
      this.router.navigate(['/inventory', 'stock-levels'], { queryParams: { productId: this.productId } });
    }
  }

  private computeMarginTrend(marginTrend: string | undefined): 'Up' | 'Down' | 'Stable' {
    if (!marginTrend) return 'Stable';
    const t = String(marginTrend).toLowerCase();
    if (t === 'up') return 'Up';
    if (t === 'down') return 'Down';
    return 'Stable';
  }
}
