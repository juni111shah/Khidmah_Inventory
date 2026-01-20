import { Component, Input, forwardRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ControlValueAccessor, FormsModule, NG_VALUE_ACCESSOR, ReactiveFormsModule } from '@angular/forms';

export type TextareaSize = 'sm' | 'md' | 'lg';

@Component({
  selector: 'app-unified-textarea',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule
  ],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => UnifiedTextareaComponent),
      multi: true
    }
  ],
  templateUrl: './unified-textarea.component.html'
})
export class UnifiedTextareaComponent implements ControlValueAccessor {
  @Input() label: string = '';
  @Input() placeholder: string = '';
  @Input() rows: number = 3;
  @Input() cols: number = 0;
  @Input() size: TextareaSize = 'md';
  @Input() icon: string = '';
  @Input() iconPosition: 'left' | 'right' = 'left';
  @Input() iconLibrary: 'fa' | 'material' | 'bi' = 'bi';
  @Input() disabled: boolean = false;
  @Input() required: boolean = false;
  @Input() error: string = '';
  @Input() hint: string = '';
  @Input() customClass: string = '';
  @Input() maxLength: number = 0;
  @Input() minLength: number = 0;
  @Input() resize: 'none' | 'both' | 'horizontal' | 'vertical' = 'vertical';
  @Input() id: string = 'textarea-' + Math.random().toString(36).substring(2, 9);

  value: string = '';
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
    const target = event.target as HTMLTextAreaElement;
    this.value = target.value;
    this.onChange(this.value);
  }

  onBlur(): void {
    this.onTouched();
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
    return '';
  }

  get characterCount(): number {
    return this.value ? this.value.length : 0;
  }
}

