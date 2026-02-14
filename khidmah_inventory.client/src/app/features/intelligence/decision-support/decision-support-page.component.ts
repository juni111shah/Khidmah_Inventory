import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DecisionSupportApiService } from '../../../core/services/decision-support-api.service';
import { HeaderService } from '../../../core/services/header.service';
import { ApiResponse } from '../../../core/models/api-response.model';
import { DecisionSupportSummary, ExplainableInsight } from '../../../core/models/decision-support.model';
import { LoadingSpinnerComponent } from '../../../shared/components/loading-spinner/loading-spinner.component';
import { ExplainableAiPanelComponent } from './explainable-ai-panel.component';
import { WhatIfSimulatorComponent } from './what-if-simulator.component';
import { BusinessHealthGaugeComponent } from './business-health-gauge.component';
import { OptimizationListComponent } from './optimization-list.component';
import { OpportunityFinderComponent } from './opportunity-finder.component';
import { ManagementStoryComponent } from './management-story.component';
import { AnomalyListComponent } from './anomaly-list.component';
import { UnifiedCardComponent } from '../../../shared/components/unified-card/unified-card.component';
import { SignalRService } from '../../../core/services/signalr.service';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { ContentLoaderComponent } from '../../../shared/components/content-loader/content-loader.component';
import { SkeletonChartComponent } from '../../../shared/components/skeleton-chart/skeleton-chart.component';
import { SkeletonListComponent } from '../../../shared/components/skeleton-list/skeleton-list.component';
import { SkeletonSidePanelComponent } from '../../../shared/components/skeleton-side-panel/skeleton-side-panel.component';
import { SkeletonTableComponent } from '../../../shared/components/skeleton-table/skeleton-table.component';

@Component({
  selector: 'app-decision-support-page',
  standalone: true,
  imports: [
    CommonModule,
    LoadingSpinnerComponent,
    UnifiedCardComponent,
    ExplainableAiPanelComponent,
    WhatIfSimulatorComponent,
    BusinessHealthGaugeComponent,
    OptimizationListComponent,
    OpportunityFinderComponent,
    ManagementStoryComponent,
    AnomalyListComponent,
    ContentLoaderComponent,
    SkeletonChartComponent,
    SkeletonListComponent,
    SkeletonSidePanelComponent,
    SkeletonTableComponent
  ],
  templateUrl: './decision-support-page.component.html'
})
export class DecisionSupportPageComponent implements OnInit, OnDestroy {
  summary: DecisionSupportSummary | null = null;
  loading = true;
  selectedInsight: ExplainableInsight | null = null;
  private destroy$ = new Subject<void>();

  constructor(
    private api: DecisionSupportApiService,
    private headerService: HeaderService,
    private signalR: SignalRService
  ) {}

  ngOnInit(): void {
    this.headerService.setHeaderInfo({
      title: 'Decision support',
      description: 'Actionable intelligence & optimization'
    });
    this.load();
    this.signalR.getStockChanged().pipe(takeUntil(this.destroy$)).subscribe(() => this.load());
    this.signalR.getSaleCompleted().pipe(takeUntil(this.destroy$)).subscribe(() => this.load());
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  load(): void {
    this.api.getSummary().subscribe({
      next: (res: ApiResponse<DecisionSupportSummary>) => {
        this.loading = false;
        if (res.success && res.data) this.summary = res.data;
      },
      error: () => { this.loading = false; }
    });
  }

  selectInsight(insight: ExplainableInsight): void {
    this.selectedInsight = insight;
  }
}
