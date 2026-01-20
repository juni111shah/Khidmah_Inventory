import { Component, Input, forwardRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ControlValueAccessor, FormsModule, NG_VALUE_ACCESSOR, ReactiveFormsModule } from '@angular/forms';

export type InputSize = 'sm' | 'md' | 'lg';

@Component({
  selector: 'app-unified-input',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule
  ],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => UnifiedInputComponent),
      multi: true
    }
  ],
  templateUrl: './unified-input.component.html'
})
export class UnifiedInputComponent implements ControlValueAccessor {
  @Input() label: string = '';
  @Input() placeholder: string = '';
  @Input() type: string = 'text';
  @Input() size: InputSize = 'md';
  @Input() icon: string = '';
  @Input() iconPosition: 'left' | 'right' = 'left';
  @Input() iconLibrary: 'fa' | 'material' | 'bi' = 'bi'; // Default to bi
  @Input() disabled: boolean = false;
  @Input() required: boolean = false;
  @Input() error: string = '';
  @Input() hint: string = '';
  @Input() customClass: string = '';
  @Input() autocomplete: string = 'off';
  @Input() id: string = 'input-' + Math.random().toString(36).substring(2, 9);
  @Input() showPasswordToggle: boolean = false;

  value: string = '';
  isPasswordVisible: boolean = false;
  private onChange = (value: string) => {};
  private onTouched = () => {};

  writeValue(value: string): void {
    this.value = value || '';
  }

  registerOnChange(fn: (value: string) => void): void {
    this.onChange = fn;
  }

  registerOnTouched(fn: () => void): void {
    this.onTouched = fn;
  }

  setDisabledState(isDisabled: boolean): void {
    this.disabled = isDisabled;
  }

  onInput(event: Event): void {
    const target = event.target as HTMLInputElement;
    this.value = target.value;
    this.onChange(this.value);
  }

  onBlur(): void {
    this.onTouched();
  }

  togglePasswordVisibility(): void {
    this.isPasswordVisible = !this.isPasswordVisible;
    this.type = this.isPasswordVisible ? 'text' : 'password';
  }

  get controlSizeClass(): string {
    switch (this.size) {
      case 'sm': return 'form-control-sm';
      case 'lg': return 'form-control-lg';
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
    return ''; // Material icons not supported anymore, or should be mapped to BI
  }
}

