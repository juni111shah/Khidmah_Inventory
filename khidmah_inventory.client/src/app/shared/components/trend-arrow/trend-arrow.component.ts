import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-trend-arrow',
  standalone: true,
  imports: [CommonModule],
  template: `
    <span [class]="containerClass" [title]="tooltip">
      <i *ngIf="trend === 'Up'" class="bi bi-arrow-up-short"></i>
      <i *ngIf="trend === 'Down'" class="bi bi-arrow-down-short"></i>
      <i *ngIf="trend === 'Stable'" class="bi bi-dash"></i>
      <ng-content></ng-content>
    </span>
  `,
  styles: [`
    .trend-up { color: var(--bs-success, #198754); }
    .trend-down { color: var(--bs-danger, #dc3545); }
    .trend-stable { color: var(--bs-secondary, #6c757d); }
  `]
})
export class TrendArrowComponent {
  @Input() trend: 'Up' | 'Down' | 'Stable' = 'Stable';
  @Input() tooltip = '';

  get containerClass(): string {
    const t = this.trend.toLowerCase();
    return `trend-${t}`;
  }
}
