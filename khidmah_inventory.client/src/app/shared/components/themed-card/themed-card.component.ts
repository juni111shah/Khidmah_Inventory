import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-themed-card',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './themed-card.component.html'
})
export class ThemedCardComponent {
  @Input() variant: 'flat' | 'elevated' | 'outlined' | 'auto' = 'auto';
  @Input() padding: string = 'var(--spacing)';
  @Input() clickable: boolean = false;

  getCardClasses(): string {
    const classes: string[] = ['card'];
    
    if (this.variant === 'auto') {
      // Use theme's card style from CSS variable
      if (typeof document !== 'undefined') {
        const cardStyle = getComputedStyle(document.documentElement)
          .getPropertyValue('--card-style')
          .trim() || 'elevated';
        classes.push(`card-${cardStyle}`);
      } else {
        classes.push('card-elevated');
      }
    } else {
      classes.push(`card-${this.variant}`);
    }
    
    return classes.join(' ');
  }
}

