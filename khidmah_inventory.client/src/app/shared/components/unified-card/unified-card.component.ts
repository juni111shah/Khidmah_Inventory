import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';

export type CardVariant = 'default' | 'primary' | 'secondary' | 'success' | 'danger' | 'warning' | 'info';
export type CardElevation = 0 | 1 | 2 | 3 | 4 | 5;

@Component({
  selector: 'app-unified-card',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './unified-card.component.html'
})
export class UnifiedCardComponent {
  @Input() variant: CardVariant = 'default';
  @Input() elevation: CardElevation = 1;
  @Input() outlined: boolean = false;
  @Input() header: string = '';
  @Input() footer: string = '';
  @Input() customClass: string = '';
  @Input() hoverable: boolean = false;
  @Input() clickable: boolean = false;

  get cardClasses(): string {
    const classes: string[] = ['card'];
    
    if (this.variant !== 'default') {
      classes.push(`text-white bg-${this.variant}`);
    }
    
    if (this.elevation > 0) {
      classes.push('shadow-sm');
    }
    
    if (this.hoverable) {
      classes.push('card-hoverable');
    }
    
    if (this.clickable) {
      classes.push('cursor-pointer');
    }
    
    if (this.customClass) {
      classes.push(this.customClass);
    }
    
    return classes.join(' ');
  }
}

