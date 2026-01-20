import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { IconComponent } from '../icon/icon.component';

export type ButtonVariant = 'primary' | 'secondary' | 'success' | 'danger' | 'warning' | 'info' | 'light' | 'dark' | 'accent' | 'link';
export type ButtonSize = 'xs' | 'sm' | 'md' | 'lg' | 'xl';
export type ButtonType = 'button' | 'submit' | 'reset';

@Component({
  selector: 'app-unified-button',
  standalone: true,
  imports: [CommonModule, IconComponent],
  templateUrl: './unified-button.component.html'
})
export class UnifiedButtonComponent {
  @Input() variant: ButtonVariant = 'primary';
  @Input() size: ButtonSize = 'md';
  @Input() type: ButtonType = 'button';
  @Input() disabled: boolean = false;
  @Input() loading: boolean = false;
  @Input() icon: string = '';
  @Input() iconPosition: 'left' | 'right' = 'left';
  @Input() iconLibrary: 'fa' | 'material' | 'bi' = 'bi';
  @Input() fullWidth: boolean = false;
  @Input() outlined: boolean = false;
  @Input() raised: boolean = false; // Bootstrap buttons are generally not 'raised' in the same way, but we can map it to shadow
  @Input() customClass: string = '';
  @Input() tooltip: string = '';
  
  @Output() clicked = new EventEmitter<MouseEvent>();

  onClick(event: MouseEvent): void {
    if (!this.disabled && !this.loading) {
      this.clicked.emit(event);
    }
  }

  get buttonClasses(): string {
    const classes: string[] = ['btn'];
    
    // Variant mapping
    const variantName = this.variant === 'accent' ? 'primary' : this.variant;
    if (this.outlined) {
      classes.push(`btn-outline-${variantName}`);
    } else {
      classes.push(`btn-${variantName}`);
    }
    
    // Size mapping
    if (this.size === 'sm' || this.size === 'xs') {
      classes.push('btn-sm');
    } else if (this.size === 'lg' || this.size === 'xl') {
      classes.push('btn-lg');
    }
    
    if (this.fullWidth) {
      classes.push('w-100');
    }
    
    if (this.raised) {
      classes.push('shadow-sm');
    }
    
    if (this.customClass) {
      classes.push(this.customClass);
    }
    
    return classes.join(' ');
  }

  get iconClass(): string {
    if (this.iconLibrary === 'fa') {
      return `fa ${this.icon}`;
    } else if (this.iconLibrary === 'bi') {
      return `bi ${this.icon}`;
    }
    return '';
  }
}

