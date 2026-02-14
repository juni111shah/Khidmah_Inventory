import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { BranchPerformanceApiService } from '../../../core/services/branch-performance-api.service';
import { BranchComparisonData } from '../../../core/models/branch-performance.model';
import { ApiResponse } from '../../../core/models/api-response.model';
import { ToastComponent } from '../../../shared/components/toast/toast.component';
import { LoadingSpinnerComponent } from '../../../shared/components/loading-spinner/loading-spinner.component';
import { UnifiedCardComponent } from '../../../shared/components/unified-card/unified-card.component';
import { ContentLoaderComponent } from '../../../shared/components/content-loader/content-loader.component';
import { SkeletonTableComponent } from '../../../shared/components/skeleton-table/skeleton-table.component';

@Component({
  selector: 'app-branch-performance',
  standalone: true,
  imports: [CommonModule, ToastComponent, LoadingSpinnerComponent, UnifiedCardComponent, ContentLoaderComponent, SkeletonTableComponent],
  templateUrl: './branch-performance.component.html'
})
export class BranchPerformanceComponent implements OnInit {
  data: BranchComparisonData | null = null;
  loading = true;
  formatCurrency = (n: number) => new Intl.NumberFormat('en-US', { style: 'currency', currency: 'USD', minimumFractionDigits: 0 }).format(n);

  constructor(private branchApi: BranchPerformanceApiService) {}

  ngOnInit(): void {
    this.branchApi.getBranchComparison().subscribe({
      next: (res: ApiResponse<BranchComparisonData>) => {
        this.loading = false;
        if (res.success && res.data) this.data = res.data;
      },
      error: () => {
        this.loading = false;
        this.branchApi.getBranchComparisonMock().subscribe(m => {
          if (m.success && m.data) this.data = m.data;
        });
      }
    });
  }

  healthClass(score: number): string {
    if (score >= 80) return 'success';
    if (score >= 60) return 'warning';
    return 'danger';
  }
}
