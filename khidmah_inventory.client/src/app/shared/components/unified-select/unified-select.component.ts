import { Component, Input, forwardRef, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ControlValueAccessor, FormsModule, NG_VALUE_ACCESSOR, ReactiveFormsModule } from '@angular/forms';
import { SelectSettings, SelectOption } from '../../models/component-settings.model';
import { ComponentSettingsService } from '../../services/component-settings.service';

@Component({
  selector: 'app-unified-select',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule
  ],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => UnifiedSelectComponent),
      multi: true
    }
  ],
  templateUrl: './unified-select.component.html'
})
export class UnifiedSelectComponent implements ControlValueAccessor, OnInit {
  @Input() id: string = 'select-' + Math.random().toString(36).substring(2, 9);
  @Input() label: string = '';
  @Input() placeholder: string = 'Select...';
  @Input() options: SelectOption[] = [];
  @Input() multiple: boolean = false;
  @Input() searchable: boolean = false;
  @Input() clearable: boolean = false;
  @Input() required: boolean = false;
  @Input() disabled: boolean = false;
  @Input() size: 'sm' | 'md' | 'lg' = 'md';
  @Input() icon: string = '';
  @Input() iconLibrary: 'fa' | 'material' | 'bi' = 'bi';
  @Input() error: string = '';
  @Input() hint: string = '';
  @Input() customClass: string = '';

  value: any = null;
  private onChange = (value: any) => {};
  private onTouched = () => {};

  constructor(private settingsService: ComponentSettingsService) {}

  ngOnInit(): void {
    if (this.id && !this.id.startsWith('select-')) {
      const settings = this.settingsService.getSelectSettings(this.id);
      if (settings) {
        Object.assign(this, settings);
      }
    }
  }

  writeValue(value: any): void {
    this.value = value;
  }

  registerOnChange(fn: (value: any) => void): void {
    this.onChange = fn;
  }

  registerOnTouched(fn: () => void): void {
    this.onTouched = fn;
  }

  setDisabledState(isDisabled: boolean): void {
    this.disabled = isDisabled;
  }

  onSelectionChange(event: Event): void {
    const target = event.target as HTMLSelectElement;
    if (this.multiple) {
      const selectedOptions = Array.from(target.selectedOptions).map(opt => opt.value);
      this.value = selectedOptions;
    } else {
      this.value = target.value;
    }
    this.onChange(this.value);
    this.onTouched();
    
    if (this.id && !this.id.startsWith('select-')) {
      this.settingsService.setSelectSettings(this.id, {});
    }
  }

  get groupedOptions(): { [key: string]: SelectOption[] } {
    const groups: { [key: string]: SelectOption[] } = {};
    this.options.forEach(option => {
      const group = option.group || 'default';
      if (!groups[group]) {
        groups[group] = [];
      }
      groups[group].push(option);
    });
    return groups;
  }

  get hasGroups(): boolean {
    return this.options.some(opt => opt.group);
  }

  get controlSizeClass(): string {
    switch (this.size) {
      case 'sm': return 'form-select-sm';
      case 'lg': return 'form-select-lg';
      default: return '';
    }
  }

  get groupSizeClass(): string {
    switch (this.size) {
      case 'sm': return 'input-group-sm';
      case 'lg': return 'input-group-lg';
      default: return '';
    }
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

