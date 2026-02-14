import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { Anomaly } from '../../../core/models/decision-support.model';
import { UnifiedCardComponent } from '../../../shared/components/unified-card/unified-card.component';

@Component({
  selector: 'app-anomaly-list',
  standalone: true,
  imports: [CommonModule, RouterModule, UnifiedCardComponent],
  template: `
    <app-unified-card header="Anomaly detection" customClass="border-0 shadow-sm">
      <div class="list-group list-group-flush">
        <div *ngFor="let a of anomalies" class="list-group-item d-flex align-items-start">
          <span class="badge bg-{{ a.severity === 'high' ? 'danger' : a.severity === 'medium' ? 'warning' : 'secondary' }} me-2">{{ a.severity }}</span>
          <div class="flex-grow-1">
            <strong>{{ a.title }}</strong>
            <p class="mb-0 small text-muted">{{ a.description }}</p>
            <span *ngIf="a.expectedRange" class="small">Expected: {{ a.expectedRange }}</span>
          </div>
          <a *ngIf="a.link" [routerLink]="a.link" class="btn btn-sm btn-outline-primary">View</a>
        </div>
      </div>
      <div *ngIf="!anomalies?.length" class="text-center text-muted py-3">No anomalies detected.</div>
    </app-unified-card>
  `
})
export class AnomalyListComponent {
  @Input() anomalies: Anomaly[] = [];
}
