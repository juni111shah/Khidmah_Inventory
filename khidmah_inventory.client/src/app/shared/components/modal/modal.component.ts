import { Component, Input, Output, EventEmitter, OnInit, OnDestroy, HostListener } from '@angular/core';
import { CommonModule } from '@angular/common';
import { IconComponent } from '../icon/icon.component';

@Component({
  selector: 'app-modal',
  standalone: true,
  imports: [CommonModule, IconComponent],
  templateUrl: './modal.component.html'
})
export class ModalComponent implements OnInit, OnDestroy {
  @Input() title: string = '';
  @Input() show: boolean = false;
  @Input() size: 'small' | 'medium' | 'large' | 'full' = 'medium';
  @Input() closable: boolean = true;
  @Input() closeOnBackdrop: boolean = true;
  @Output() showChange = new EventEmitter<boolean>();
  @Output() close = new EventEmitter<void>();

  ngOnInit(): void {
    if (this.show) {
      document.body.style.overflow = 'hidden';
    }
  }

  ngOnDestroy(): void {
    document.body.style.overflow = '';
  }

  @HostListener('document:keydown.escape', ['$event'])
  onEscapeKey(event: KeyboardEvent): void {
    if (this.show && this.closable) {
      this.closeModal();
    }
  }

  onBackdropClick(): void {
    if (this.closeOnBackdrop && this.closable) {
      this.closeModal();
    }
  }

  closeModal(): void {
    this.show = false;
    this.showChange.emit(false);
    this.close.emit();
    document.body.style.overflow = '';
  }

  onContentClick(event: Event): void {
    event.stopPropagation();
  }
}

