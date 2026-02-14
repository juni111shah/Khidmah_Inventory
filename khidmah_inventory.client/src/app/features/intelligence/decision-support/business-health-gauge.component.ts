import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { BusinessHealthScore } from '../../../core/models/decision-support.model';
import { UnifiedCardComponent } from '../../../shared/components/unified-card/unified-card.component';

@Component({
  selector: 'app-business-health-gauge',
  standalone: true,
  imports: [CommonModule, RouterModule, UnifiedCardComponent],
  template: `
    <app-unified-card [header]="header" customClass="border-0 shadow-sm text-center">
      <ng-container *ngIf="health">
        <div class="position-relative d-inline-block mb-3">
          <svg viewBox="0 0 36 36" class="circular-chart" [style.width.px]="size" [style.height.px]="size">
            <path class="circle-bg" d="M18 2.0845 a 15.9155 15.9155 0 0 1 0 31.831 a 15.9155 15.9155 0 0 1 0 -31.831" />
            <path class="circle" [attr.stroke-dasharray]="health.score + ', 100'" [class]="'circle-' + health.grade" d="M18 2.0845 a 15.9155 15.9155 0 0 1 0 31.831 a 15.9155 15.9155 0 0 1 0 -31.831" />
          </svg>
          <div class="position-absolute top-50 start-50 translate-middle">
            <span class="display-6 fw-bold">{{ health.score }}</span>
            <span class="d-block small text-muted">{{ health.grade }}</span>
          </div>
        </div>
        <p class="mb-2 small text-muted">Business health score</p>
        <ul class="list-unstyled text-start small" *ngIf="showFactors">
          <li *ngFor="let f of health.factors" class="d-flex justify-content-between py-1">
            <span>{{ f.name }}</span>
            <span class="badge bg-{{ f.status === 'good' ? 'success' : f.status === 'warning' ? 'warning' : 'danger' }}">{{ f.value | number:'1.0-0' }}</span>
          </li>
        </ul>
      </ng-container>
      <div *ngIf="!health" class="text-muted small">Loading...</div>
    </app-unified-card>
  `,
  styles: [`
    .circular-chart { display: block; margin: 0 auto; }
    .circle-bg { fill: none; stroke: #eee; stroke-width: 2.8; }
    .circle { fill: none; stroke-width: 2.8; stroke-linecap: round; transition: stroke-dasharray 0.5s; }
    .circle-A { stroke: #198754; }
    .circle-B { stroke: #20c997; }
    .circle-C { stroke: #ffc107; }
    .circle-D { stroke: #fd7e14; }
    .circle-F { stroke: #dc3545; }
  `]
})
export class BusinessHealthGaugeComponent {
  @Input() health: BusinessHealthScore | null = null;
  @Input() header = 'Business health';
  @Input() size = 140;
  @Input() showFactors = true;
}
