import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { OptimizationSuggestion } from '../../../core/models/decision-support.model';
import { UnifiedCardComponent } from '../../../shared/components/unified-card/unified-card.component';

@Component({
  selector: 'app-optimization-list',
  standalone: true,
  imports: [CommonModule, RouterModule, UnifiedCardComponent],
  template: `
    <app-unified-card header="Optimization engine" customClass="border-0 shadow-sm">
      <p class="small text-muted mb-3">Suggested actions to improve operations.</p>
      <div class="list-group list-group-flush">
        <div *ngFor="let s of suggestions" class="list-group-item d-flex align-items-start">
          <span class="badge bg-{{ s.priority === 'high' ? 'danger' : s.priority === 'medium' ? 'warning' : 'secondary' }} me-2">{{ s.priority }}</span>
          <div class="flex-grow-1">
            <strong>{{ s.title }}</strong>
            <p class="mb-0 small text-muted">{{ s.description }}</p>
            <span *ngIf="s.impact" class="badge bg-light text-dark mt-1">{{ s.impact }}</span>
          </div>
          <a *ngIf="s.actionLink" [routerLink]="s.actionLink" class="btn btn-sm btn-outline-primary">{{ s.actionLabel || 'View' }}</a>
        </div>
      </div>
      <div *ngIf="!suggestions?.length" class="text-center text-muted py-3">No suggestions right now.</div>
    </app-unified-card>
  `
})
export class OptimizationListComponent {
  @Input() suggestions: OptimizationSuggestion[] = [];
}
