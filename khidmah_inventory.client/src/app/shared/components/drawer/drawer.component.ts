import { Component, Input, Output, EventEmitter, HostListener } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-drawer',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './drawer.component.html'
})
export class DrawerComponent {
  @Input() isOpen: boolean = false;
  @Input() position: 'left' | 'right' | 'top' | 'bottom' = 'right';
  @Input() title: string = '';
  @Input() width: string = '400px';
  @Input() height: string = '100%';
  @Input() closable: boolean = true;
  @Input() closeOnBackdrop: boolean = true;

  @Output() isOpenChange = new EventEmitter<boolean>();
  @Output() close = new EventEmitter<void>();

  @HostListener('document:keydown.escape', ['$event'])
  onEscapeKey(event: KeyboardEvent): void {
    if (this.isOpen && this.closable) {
      this.closeDrawer();
    }
  }

  closeDrawer(): void {
    this.isOpen = false;
    this.isOpenChange.emit(false);
    this.close.emit();
  }

  onBackdropClick(): void {
    if (this.closeOnBackdrop && this.closable) {
      this.closeDrawer();
    }
  }

  onContentClick(event: Event): void {
    event.stopPropagation();
  }
}


