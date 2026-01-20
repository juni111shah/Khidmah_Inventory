import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-badge',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './badge.component.html'
})
export class BadgeComponent {
  @Input() value: string | number = '';
  @Input() color: 'primary' | 'secondary' | 'accent' | 'success' | 'warning' | 'error' | string = 'primary';
  @Input() size: 'small' | 'medium' | 'large' = 'medium';
  @Input() dot: boolean = false;
}

