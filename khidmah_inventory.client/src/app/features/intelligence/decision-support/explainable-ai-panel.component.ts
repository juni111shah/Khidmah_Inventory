import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { ExplainableInsight } from '../../../core/models/decision-support.model';
import { UnifiedCardComponent } from '../../../shared/components/unified-card/unified-card.component';
import { IconComponent } from '../../../shared/components/icon/icon.component';

@Component({
  selector: 'app-explainable-ai-panel',
  standalone: true,
  imports: [CommonModule, RouterModule, UnifiedCardComponent, IconComponent],
  template: `
    <app-unified-card header="Why this recommendation" customClass="border-0 shadow-sm">
      <ng-container *ngIf="insight">
        <div class="d-flex justify-content-between align-items-start mb-3">
          <h6 class="mb-0">{{ insight.title }}</h6>
          <span class="badge bg-{{ severityClass(insight.severity) }}">{{ insight.severity }}</span>
        </div>
        <ul class="list-unstyled mb-0">
          <li *ngFor="let r of insight.reasons" class="d-flex justify-content-between py-1 border-bottom border-light">
            <span class="text-muted">{{ r.label }}</span>
            <span>{{ r.value }}{{ r.unit ? ' ' + r.unit : '' }} <i *ngIf="r.trend" class="bi bi-arrow-{{ r.trend === 'up' ? 'up' : r.trend === 'down' ? 'down' : 'minus' }} text-{{ r.trend === 'up' ? 'success' : r.trend === 'down' ? 'danger' : 'secondary' }}"></i></span>
          </li>
        </ul>
        <p *ngIf="insight.suggestedAction" class="mt-2 mb-0 small"><strong>Suggested:</strong> {{ insight.suggestedAction }}</p>
        <a *ngIf="insight.link" [routerLink]="insight.link" class="btn btn-sm btn-primary mt-2">Take action</a>
      </ng-container>
      <div *ngIf="!insight" class="text-muted small">Select a recommendation to see why.</div>
    </app-unified-card>
  `
})
export class ExplainableAiPanelComponent {
  @Input() insight: ExplainableInsight | null = null;

  severityClass(s: string): string {
    return s === 'critical' ? 'danger' : s === 'warning' ? 'warning' : 'info';
  }
}
