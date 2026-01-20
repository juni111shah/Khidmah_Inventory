import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';

export type IconLibrary = 'fa' | 'bi' | 'material';
export type IconSize = 'xs' | 'sm' | 'md' | 'lg' | 'xl' | 'xxl';

@Component({
  selector: 'app-icon',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './icon.component.html'
})
export class IconComponent {
  @Input() name: string = '';
  @Input() library: IconLibrary = 'bi'; // Default to bootstrap icons
  @Input() size: IconSize = 'md';
  @Input() color: string = '';
  @Input() customClass: string = '';
  @Input() spin: boolean = false;
  @Input() pulse: boolean = false;
  @Input() rotate: number = 0;
  @Input() flip: 'horizontal' | 'vertical' | 'both' | '' = '';

  get iconClasses(): string {
    const classes: string[] = [];
    
    if (this.library === 'fa') {
      const parts = this.name.split(' ');
      if (parts.length > 1 && ['fas', 'far', 'fab', 'fal', 'fad'].includes(parts[0])) {
        classes.push(...parts);
      } else if (this.name.startsWith('fa-')) {
        classes.push('fa', 'fas', this.name);
      } else {
        classes.push('fa', 'fas', `fa-${this.name}`);
      }
      
      if (this.spin) classes.push('fa-spin');
      if (this.pulse) classes.push('fa-pulse');
      if (this.rotate) classes.push(`fa-rotate-${this.rotate}`);
      if (this.flip === 'horizontal') classes.push('fa-flip-horizontal');
      if (this.flip === 'vertical') classes.push('fa-flip-vertical');
      if (this.flip === 'both') classes.push('fa-flip-horizontal', 'fa-flip-vertical');
    } else if (this.library === 'bi') {
      classes.push('bi');
      if (this.name.startsWith('bi-')) {
        classes.push(this.name);
      } else {
        classes.push(`bi-${this.name}`);
      }
      if (this.spin) classes.push('bi-spin'); // Custom CSS might be needed for BI spin
    }
    
    classes.push(`icon-${this.size}`);
    
    if (this.customClass) {
      classes.push(this.customClass);
    }
    
    return classes.join(' ');
  }

  get styleObject(): { [key: string]: string } {
    const styles: { [key: string]: string } = {};
    if (this.color) {
      styles['color'] = this.color;
    }
    if (this.rotate && this.library !== 'fa') {
      styles['transform'] = `rotate(${this.rotate}deg)`;
    }
    return styles;
  }
}

