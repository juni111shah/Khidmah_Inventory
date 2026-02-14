import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, ActivatedRoute, RouterModule } from '@angular/router';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { DailyBriefingApiService } from '../../core/services/daily-briefing-api.service';
import { AuthService } from '../../core/services/auth.service';
import { SignalRService } from '../../core/services/signalr.service';
import { ApiResponse } from '../../core/models/api-response.model';
import { DailyBriefing, BriefingStatus } from '../../core/models/daily-briefing.model';
import { User } from '../../core/models/user.model';
import { ToastComponent } from '../../shared/components/toast/toast.component';
import { LoadingSpinnerComponent } from '../../shared/components/loading-spinner/loading-spinner.component';
import { UnifiedCardComponent } from '../../shared/components/unified-card/unified-card.component';
import { KpiStatCardComponent } from '../../shared/components/kpi-stat-card/kpi-stat-card.component';
import { IconComponent } from '../../shared/components/icon/icon.component';
import { UnifiedButtonComponent } from '../../shared/components/unified-button/unified-button.component';
import { HubConnectionState } from '@microsoft/signalr';
import { ContentLoaderComponent } from '../../shared/components/content-loader/content-loader.component';
import { SkeletonStatCardsComponent } from '../../shared/components/skeleton-stat-cards/skeleton-stat-cards.component';
import { SkeletonListComponent } from '../../shared/components/skeleton-list/skeleton-list.component';
import { SkeletonLoaderComponent } from '../../shared/components/skeleton-loader/skeleton-loader.component';

@Component({
  selector: 'app-daily-briefing',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    ToastComponent,
    LoadingSpinnerComponent,
    UnifiedCardComponent,
    KpiStatCardComponent,
    IconComponent,
    UnifiedButtonComponent,
    ContentLoaderComponent,
    SkeletonStatCardsComponent,
    SkeletonListComponent,
    SkeletonLoaderComponent
  ],
  templateUrl: './daily-briefing.component.html',
  styles: [`
    .briefing-trend { font-size: 0.8rem; font-weight: 600; }
    .stat-badge { font-size: 0.7rem; font-weight: 700; padding: 0.2rem 0.5rem; border-radius: 0.25rem; }
    .stat-badge.CRITICAL { background: var(--error-color); color: #fff; }
    .stat-badge.WATCH { background: var(--warning-color); color: #000; }
    .stat-badge.GOOD { background: var(--success-color); color: #fff; }
    .trend-up { color: var(--success-color); }
    .trend-down { color: var(--error-color); }
    .trend-flat { color: var(--text-secondary-color); }
    .section-title { font-size: 1rem; font-weight: 600; margin-bottom: 0.75rem; }
    .quick-action-btn { min-width: 120px; }
  `]
})
export class DailyBriefingComponent implements OnInit, OnDestroy {
  data: DailyBriefing | null = null;
  loading = true;
  showToast = false;
  toastMessage = '';
  toastType: 'success' | 'error' | 'info' = 'info';
  user: User | null = null;
  returnUrl = '/dashboard';
  signalRLive = false;
  skeletonQuickActionCount = [1, 2, 3, 4, 5, 6];
  private destroy$ = new Subject<void>();

  constructor(
    private briefingApi: DailyBriefingApiService,
    private authService: AuthService,
    private signalR: SignalRService,
    private router: Router,
    private route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    this.user = this.authService.getCurrentUser();
    this.returnUrl = this.route.snapshot.queryParams['returnUrl'] || '/dashboard';
    this.load();
    this.signalR.getConnectionState().pipe(takeUntil(this.destroy$)).subscribe(state => {
      this.signalRLive = state === HubConnectionState.Connected;
    });
    this.signalR.getStockChanged().pipe(takeUntil(this.destroy$)).subscribe(() => this.load());
    this.signalR.getOrderApproved().pipe(takeUntil(this.destroy$)).subscribe(() => this.load());
    this.signalR.getSaleCompleted().pipe(takeUntil(this.destroy$)).subscribe(() => this.load());
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  load(): void {
    this.briefingApi.getBriefing().subscribe({
      next: (res: ApiResponse<DailyBriefing>) => {
        this.loading = false;
        if (res.success && res.data) this.data = res.data;
      },
      error: () => { this.loading = false; this.showToastMsg('Failed to load briefing', 'error'); }
    });
  }

  continue(): void {
    this.router.navigateByUrl(this.returnUrl);
  }

  formatCurrency(n: number): string {
    return new Intl.NumberFormat('en-US', { style: 'currency', currency: 'USD', minimumFractionDigits: 0, maximumFractionDigits: 0 }).format(n);
  }

  greeting(): string {
    const hour = new Date().getHours();
    if (hour < 12) return 'Good morning';
    if (hour < 17) return 'Good afternoon';
    return 'Good evening';
  }

  displayDate(): string {
    return new Date().toLocaleDateString('en-US', { weekday: 'long', year: 'numeric', month: 'long', day: 'numeric' });
  }

  trendIcon(dir: 'up' | 'down' | 'flat'): string {
    return dir === 'up' ? 'bi-arrow-up' : dir === 'down' ? 'bi-arrow-down' : 'bi-dash';
  }

  trendClass(dir: 'up' | 'down' | 'flat'): string {
    return dir === 'up' ? 'trend-up' : dir === 'down' ? 'trend-down' : 'trend-flat';
  }

  statusBadgeClass(s: BriefingStatus): string {
    return `stat-badge ${s}`;
  }

  showToastMsg(msg: string, type: 'success' | 'error' | 'info'): void {
    this.toastMessage = msg;
    this.toastType = type;
    this.showToast = true;
  }
}
