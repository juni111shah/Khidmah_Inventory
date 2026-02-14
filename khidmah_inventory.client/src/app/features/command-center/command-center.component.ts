import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { CommandCenterApiService } from '../../core/services/command-center-api.service';
import { SignalRService } from '../../core/services/signalr.service';
import { CommandCenterData } from '../../core/models/command-center.model';
import { ApiResponse } from '../../core/models/api-response.model';
import { ToastComponent } from '../../shared/components/toast/toast.component';
import { LoadingSpinnerComponent } from '../../shared/components/loading-spinner/loading-spinner.component';
import { UnifiedCardComponent } from '../../shared/components/unified-card/unified-card.component';
import { KpiStatCardComponent } from '../../shared/components/kpi-stat-card/kpi-stat-card.component';
import { IconComponent } from '../../shared/components/icon/icon.component';
import { HubConnectionState } from '@microsoft/signalr';
import { ContentLoaderComponent } from '../../shared/components/content-loader/content-loader.component';
import { SkeletonStatCardsComponent } from '../../shared/components/skeleton-stat-cards/skeleton-stat-cards.component';
import { SkeletonListComponent } from '../../shared/components/skeleton-list/skeleton-list.component';

@Component({
  selector: 'app-command-center',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    ToastComponent,
    LoadingSpinnerComponent,
    UnifiedCardComponent,
    KpiStatCardComponent,
    IconComponent,
    ContentLoaderComponent,
    SkeletonStatCardsComponent,
    SkeletonListComponent
  ],
  templateUrl: './command-center.component.html'
})
export class CommandCenterComponent implements OnInit, OnDestroy {
  data: CommandCenterData | null = null;
  loading = true;
  showToast = false;
  toastMessage = '';
  toastType: 'success' | 'error' | 'info' = 'success';
  signalRConnected = false;
  private destroy$ = new Subject<void>();
  private refreshInterval: any;

  constructor(
    private commandCenterApi: CommandCenterApiService,
    private signalR: SignalRService
  ) {}

  ngOnInit(): void {
    this.load();
    this.signalR.getConnectionState().pipe(takeUntil(this.destroy$)).subscribe(state => {
      this.signalRConnected = state === HubConnectionState.Connected;
      if (this.signalRConnected) this.signalR.startConnection();
    });
    this.signalR.startConnection();
    this.refreshInterval = setInterval(() => this.load(), 60000);
    // Real-time refresh on operations events
    this.signalR.getStockChanged().pipe(takeUntil(this.destroy$)).subscribe(() => this.load());
    this.signalR.getSaleCompleted().pipe(takeUntil(this.destroy$)).subscribe(() => this.load());
    this.signalR.getOrderCreated().pipe(takeUntil(this.destroy$)).subscribe(() => this.load());
    this.signalR.getOrderApproved().pipe(takeUntil(this.destroy$)).subscribe(() => this.load());
    this.signalR.getPurchaseCreated().pipe(takeUntil(this.destroy$)).subscribe(() => this.load());
    this.signalR.getProductUpdated().pipe(takeUntil(this.destroy$)).subscribe(() => this.load());
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
    if (this.refreshInterval) clearInterval(this.refreshInterval);
  }

  load(): void {
    this.commandCenterApi.getCommandCenterData().subscribe({
      next: (res: ApiResponse<CommandCenterData>) => {
        this.loading = false;
        if (res.success && res.data) this.data = res.data;
      },
      error: () => { this.loading = false; }
    });
  }

  formatCurrency(n: number): string {
    return new Intl.NumberFormat('en-US', { style: 'currency', currency: 'USD', minimumFractionDigits: 0 }).format(n);
  }
}
