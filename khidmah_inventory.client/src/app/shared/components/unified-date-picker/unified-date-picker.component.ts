import { Component, Input, forwardRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ControlValueAccessor, FormsModule, NG_VALUE_ACCESSOR, ReactiveFormsModule } from '@angular/forms';

@Component({
  selector: 'app-unified-date-picker',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule
  ],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => UnifiedDatePickerComponent),
      multi: true
    }
  ],
  templateUrl: './unified-date-picker.component.html'
})
export class UnifiedDatePickerComponent implements ControlValueAccessor {
  @Input() label: string = '';
  @Input() placeholder: string = '';
  @Input() disabled: boolean = false;
  @Input() required: boolean = false;
  @Input() error: string = '';
  @Input() hint: string = '';
  @Input() customClass: string = '';
  @Input() minDate: Date | null = null;
  @Input() maxDate: Date | null = null;
  @Input() startView: 'month' | 'year' | 'multi-year' = 'month';
  @Input() touchUI: boolean = false;
  @Input() id: string = 'datepicker-' + Math.random().toString(36).substring(2, 9);

  value: string = ''; // Using string for date input compatibility
  private onChange = (value: any) => {};
  private onTouched = () => {};

  writeValue(value: any): void {
    if (value instanceof Date) {
      this.value = value.toISOString().split('T')[0];
    } else {
      this.value = value || '';
    }
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

  onDateChange(event: Event): void {
    const target = event.target as HTMLInputElement;
    this.value = target.value;
    const dateValue = this.value ? new Date(this.value) : null;
    this.onChange(dateValue);
    this.onTouched();
  }

  onBlur(): void {
    this.onTouched();
  }

  get minDateStr(): string | null {
    return this.minDate ? this.minDate.toISOString().split('T')[0] : null;
  }

  get maxDateStr(): string | null {
    return this.maxDate ? this.maxDate.toISOString().split('T')[0] : null;
  }
}

