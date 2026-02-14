import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { Opportunity } from '../../../core/models/decision-support.model';
import { UnifiedCardComponent } from '../../../shared/components/unified-card/unified-card.component';

@Component({
  selector: 'app-opportunity-finder',
  standalone: true,
  imports: [CommonModule, RouterModule, UnifiedCardComponent],
  template: `
    <app-unified-card header="Opportunity finder" customClass="border-0 shadow-sm">
      <div class="row g-2">
        <div *ngFor="let o of opportunities" class="col-12 col-md-6">
          <div class="p-2 rounded border h-100">
            <span class="badge bg-info me-1">{{ typeLabel(o.type) }}</span>
            <strong class="d-block">{{ o.title }}</strong>
            <p class="mb-1 small text-muted">{{ o.description }}</p>
            <span *ngIf="o.metric != null" class="small fw-bold">{{ o.metric }}{{ o.metricLabel ? ' ' + o.metricLabel : '' }}</span>
            <a *ngIf="o.link" [routerLink]="o.link" class="btn btn-sm btn-link p-0 ms-1">Go</a>
          </div>
        </div>
      </div>
      <div *ngIf="!opportunities?.length" class="text-center text-muted py-3">No opportunities identified.</div>
    </app-unified-card>
  `
})
export class OpportunityFinderComponent {
  @Input() opportunities: Opportunity[] = [];

  typeLabel(t: string): string {
    return t.replace(/_/g, ' ');
  }
}
