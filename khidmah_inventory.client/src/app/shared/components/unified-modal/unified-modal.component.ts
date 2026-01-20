import { Component, Input, Output, EventEmitter, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ButtonSettings } from '../../models/component-settings.model';
import { ComponentSettingsService } from '../../services/component-settings.service';
import { UnifiedButtonComponent } from '../unified-button/unified-button.component';

@Component({
  selector: 'app-unified-modal',
  standalone: true,
  imports: [
    CommonModule,
    UnifiedButtonComponent
  ],
  templateUrl: './unified-modal.component.html'
})
export class UnifiedModalComponent implements OnInit {
  @Input() id: string = '';
  @Input() isOpen: boolean = false;
  @Input() size: 'xs' | 'sm' | 'md' | 'lg' | 'xl' | 'fullscreen' = 'md';
  @Input() backdrop: boolean | 'static' = true;
  @Input() keyboard: boolean = true;
  @Input() centered: boolean = true;
  @Input() scrollable: boolean = true;
  @Input() closeOnOutsideClick: boolean = true;
  @Input() showHeader: boolean = true;
  @Input() showFooter: boolean = true;
  @Input() headerTitle: string = '';
  @Input() footerButtons: ButtonSettings[] = [];
  @Input() customClass: string = '';

  @Output() opened = new EventEmitter<void>();
  @Output() closed = new EventEmitter<void>();
  @Output() backdropClick = new EventEmitter<void>();

  constructor(private settingsService: ComponentSettingsService) {}

  ngOnInit(): void {
    if (this.id) {
      const settings = this.settingsService.getModalSettings(this.id);
      if (settings) {
        Object.assign(this, settings);
      }
    }
  }

  open(): void {
    this.isOpen = true;
    this.opened.emit();
  }

  close(): void {
    this.isOpen = false;
    this.closed.emit();
  }

  onBackdropClick(): void {
    if (this.closeOnOutsideClick) {
      this.close();
    }
    this.backdropClick.emit();
  }

  get modalDialogClasses(): string {
    const classes: string[] = ['modal-dialog'];
    
    if (this.centered) classes.push('modal-dialog-centered');
    if (this.scrollable) classes.push('modal-dialog-scrollable');
    
    if (this.size === 'sm') classes.push('modal-sm');
    if (this.size === 'lg') classes.push('modal-lg');
    if (this.size === 'xl') classes.push('modal-xl');
    if (this.size === 'fullscreen') classes.push('modal-fullscreen');
    
    if (this.customClass) {
      classes.push(this.customClass);
    }
    
    return classes.join(' ');
  }
}

