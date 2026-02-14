import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { ReorderApiService } from '../../../core/services/reorder-api.service';
import { ReorderSuggestion } from '../../../core/models/reorder.model';
import { ApiResponse } from '../../../core/models/api-response.model';
import { ToastComponent } from '../../../shared/components/toast/toast.component';
import { LoadingSpinnerComponent } from '../../../shared/components/loading-spinner/loading-spinner.component';
import { UnifiedCardComponent } from '../../../shared/components/unified-card/unified-card.component';
import { ContentLoaderComponent } from '../../../shared/components/content-loader/content-loader.component';
import { SkeletonDetailHeaderComponent } from '../../../shared/components/skeleton-detail-header/skeleton-detail-header.component';
import { SkeletonFormComponent } from '../../../shared/components/skeleton-form/skeleton-form.component';
import { SkeletonListComponent } from '../../../shared/components/skeleton-list/skeleton-list.component';

@Component({
  selector: 'app-reorder-review',
  standalone: true,
  imports: [CommonModule, RouterModule, ToastComponent, LoadingSpinnerComponent, UnifiedCardComponent, ContentLoaderComponent, SkeletonDetailHeaderComponent, SkeletonFormComponent, SkeletonListComponent],
  templateUrl: './reorder-review.component.html'
})
export class ReorderReviewComponent implements OnInit {
  loading = true;
  suggestion: ReorderSuggestion | null = null;
  showToast = false;
  toastMessage = '';
  toastType: 'success' | 'error' | 'info' = 'success';

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private reorderApi: ReorderApiService
  ) {}

  ngOnInit(): void {
    const productId = this.route.snapshot.queryParamMap.get('productId');
    const warehouseId = this.route.snapshot.queryParamMap.get('warehouseId');
    if (productId && warehouseId) this.loadSuggestion(productId, warehouseId);
    else this.loading = false;
  }

  loadSuggestion(productId: string, warehouseId: string): void {
    this.reorderApi.getSuggestions({}).subscribe({
      next: (res: ApiResponse<ReorderSuggestion[]>) => {
        this.loading = false;
        if (res.success && res.data) {
          this.suggestion = res.data.find(s => s.productId === productId && s.warehouseId === warehouseId) || null;
          if (!this.suggestion) this.showToastMsg('Suggestion not found', 'error');
        }
      },
      error: () => {
        this.loading = false;
        this.showToastMsg('Error loading suggestion', 'error');
      }
    });
  }

  goToGeneratePO(): void {
    if (!this.suggestion) return;
    this.router.navigate(['/reorder/generate-po'], {
      queryParams: { ids: `${this.suggestion.productId}|${this.suggestion.warehouseId}` }
    });
  }

  showToastMsg(msg: string, type: 'success' | 'error' | 'info'): void {
    this.toastMessage = msg;
    this.toastType = type;
    this.showToast = true;
  }

  getPriorityClass(priority: string): string {
    switch (priority) {
      case 'Critical': return 'danger';
      case 'High': return 'warning';
      case 'Medium': return 'info';
      default: return 'secondary';
    }
  }
}
