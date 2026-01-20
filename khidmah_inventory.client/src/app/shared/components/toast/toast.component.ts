import { Component, Input, Output, EventEmitter, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { IconComponent } from '../icon/icon.component';

export type ToastType = 'success' | 'error' | 'warning' | 'info';

@Component({
  selector: 'app-toast',
  standalone: true,
  imports: [CommonModule, IconComponent],
  templateUrl: './toast.component.html'
})
export class ToastComponent implements OnInit {
  @Input() message: string = '';
  @Input() type: ToastType = 'info';
  @Input() duration: number = 3000;
  @Input() show: boolean = false;
  @Output() showChange = new EventEmitter<boolean>();
  @Output() close = new EventEmitter<void>();

  private timeoutId: any;

  ngOnInit(): void {
    if (this.show && this.duration > 0) {
      this.timeoutId = setTimeout(() => {
        this.closeToast();
      }, this.duration);
    }
  }

  closeToast(): void {
    this.show = false;
    this.showChange.emit(false);
    this.close.emit();
    if (this.timeoutId) {
      clearTimeout(this.timeoutId);
    }
  }

  getIcon(): string {
    switch (this.type) {
      case 'success': return 'check_circle';
      case 'error': return 'error';
      case 'warning': return 'warning';
      case 'info': return 'info';
      default: return 'info';
    }
  }
}

