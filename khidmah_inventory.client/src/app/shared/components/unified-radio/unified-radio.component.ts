import { Component, Input, forwardRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ControlValueAccessor, FormsModule, NG_VALUE_ACCESSOR, ReactiveFormsModule } from '@angular/forms';

export interface RadioOption {
  value: any;
  label: string;
  disabled?: boolean;
}

@Component({
  selector: 'app-unified-radio',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule
  ],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => UnifiedRadioComponent),
      multi: true
    }
  ],
  templateUrl: './unified-radio.component.html'
})
export class UnifiedRadioComponent implements ControlValueAccessor {
  @Input() label: string = '';
  @Input() options: RadioOption[] = [];
  @Input() disabled: boolean = false;
  @Input() required: boolean = false;
  @Input() color: 'primary' | 'accent' | 'warn' = 'primary';
  @Input() layout: 'vertical' | 'horizontal' = 'vertical';
  @Input() customClass: string = '';
  @Input() name: string = 'radio-group-' + Math.random().toString(36).substring(2, 9);

  value: any = null;
  private onChange = (value: any) => {};
  private onTouched = () => {};

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

  onRadioChange(value: any): void {
    this.value = value;
    this.onChange(value);
    this.onTouched();
  }

  get radioClasses(): string {
    const classes: string[] = ['unified-radio'];
    if (this.layout === 'horizontal') {
      classes.push('d-flex flex-wrap gap-3');
    }
    
    if (this.customClass) {
      classes.push(this.customClass);
    }
    
    return classes.join(' ');
  }
}

