import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-severity-badge',
  standalone: true,
  imports: [CommonModule],
  template: `<span class="badge" [ngClass]="badgeClass">{{ label || severity }}</span>`
})
export class SeverityBadgeComponent {
  @Input() severity: 'Low' | 'Medium' | 'High' | 'Critical' = 'Low';
  @Input() label = '';

  get badgeClass(): string {
    const map: Record<string, string> = {
      Low: 'bg-secondary',
      Medium: 'bg-warning text-dark',
      High: 'bg-danger',
      Critical: 'bg-danger'
    };
    return map[this.severity] || 'bg-secondary';
  }
}
