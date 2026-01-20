import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-themed-button',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './themed-button.component.html'
})
export class ThemedButtonComponent {
  @Input() type: 'button' | 'submit' | 'reset' = 'button';
  @Input() variant: 'flat' | 'raised' | 'outlined' | 'gradient' | 'auto' = 'auto';
  @Input() disabled: boolean = false;
  @Input() size: 'small' | 'medium' | 'large' = 'medium';
  @Input() fullWidth: boolean = false;

  getButtonClasses(): string {
    const classes: string[] = ['btn'];
    
    if (this.variant === 'auto') {
      // Use theme's button style from CSS variable
      if (typeof document !== 'undefined') {
        const buttonStyle = getComputedStyle(document.documentElement)
          .getPropertyValue('--button-style')
          .trim() || 'raised';
        classes.push(`btn-${buttonStyle}`);
      } else {
        classes.push('btn-raised');
      }
    } else {
      classes.push(`btn-${this.variant}`);
    }
    
    classes.push(`btn-${this.size}`);
    
    if (this.fullWidth) {
      classes.push('full-width');
    }
    
    return classes.join(' ');
  }
}

