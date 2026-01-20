import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-app-footer',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './app-footer.component.html'
})
export class AppFooterComponent {
  @Input() copyright: string = '';
  @Input() showCopyright: boolean = true;
  
  get currentYear(): number {
    return new Date().getFullYear();
  }
  
  get defaultCopyright(): string {
    return `© ${this.currentYear} All rights reserved.`;
  }
}

