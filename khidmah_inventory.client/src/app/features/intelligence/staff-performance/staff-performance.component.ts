import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { StaffPerformanceApiService } from '../../../core/services/staff-performance-api.service';
import { StaffPerformanceData } from '../../../core/models/staff-performance.model';
import { ApiResponse } from '../../../core/models/api-response.model';
import { ToastComponent } from '../../../shared/components/toast/toast.component';
import { LoadingSpinnerComponent } from '../../../shared/components/loading-spinner/loading-spinner.component';
import { UnifiedCardComponent } from '../../../shared/components/unified-card/unified-card.component';
import { ContentLoaderComponent } from '../../../shared/components/content-loader/content-loader.component';
import { SkeletonTableComponent } from '../../../shared/components/skeleton-table/skeleton-table.component';

@Component({
  selector: 'app-staff-performance',
  standalone: true,
  imports: [CommonModule, ToastComponent, LoadingSpinnerComponent, UnifiedCardComponent, ContentLoaderComponent, SkeletonTableComponent],
  templateUrl: './staff-performance.component.html'
})
export class StaffPerformanceComponent implements OnInit {
  data: StaffPerformanceData | null = null;
  loading = true;
  formatCurrency = (n: number) => new Intl.NumberFormat('en-US', { style: 'currency', currency: 'USD', minimumFractionDigits: 0 }).format(n);

  constructor(private staffApi: StaffPerformanceApiService) {}

  ngOnInit(): void {
    this.staffApi.getStaffPerformance().subscribe({
      next: (res: ApiResponse<StaffPerformanceData>) => {
        this.loading = false;
        if (res.success && res.data) this.data = res.data;
      },
      error: () => {
        this.loading = false;
        this.staffApi.getStaffPerformanceMock().subscribe(m => {
          if (m.success && m.data) this.data = m.data;
        });
      }
    });
  }
}
