import { Component, Input, Output, EventEmitter, HostListener } from '@angular/core';
import { CommonModule } from '@angular/common';
import { IconComponent } from '../icon/icon.component';

export interface DropdownOption {
  label: string;
  value: any;
  icon?: string;
  disabled?: boolean;
  divider?: boolean;
}

@Component({
  selector: 'app-dropdown',
  standalone: true,
  imports: [CommonModule, IconComponent],
  templateUrl: './dropdown.component.html'
})
export class DropdownComponent {
  @Input() options: DropdownOption[] = [];
  @Input() placeholder: string = 'Select an option';
  @Input() value: any = null;
  @Input() disabled: boolean = false;
  @Output() valueChange = new EventEmitter<any>();
  @Output() optionClick = new EventEmitter<DropdownOption>();

  isOpen: boolean = false;

  @HostListener('document:click', ['$event'])
  onDocumentClick(event: MouseEvent): void {
    const target = event.target as HTMLElement;
    if (!target.closest('.dropdown')) {
      this.isOpen = false;
    }
  }

  toggleDropdown(): void {
    if (!this.disabled) {
      this.isOpen = !this.isOpen;
    }
  }

  selectOption(option: DropdownOption): void {
    if (option.disabled || option.divider) {
      return;
    }
    this.value = option.value;
    this.valueChange.emit(option.value);
    this.optionClick.emit(option);
    this.isOpen = false;
  }

  getSelectedLabel(): string {
    const selected = this.options.find(opt => opt.value === this.value);
    return selected ? selected.label : this.placeholder;
  }
}

