import { Component, Input, Output, EventEmitter, forwardRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ControlValueAccessor, FormsModule, NG_VALUE_ACCESSOR, ReactiveFormsModule } from '@angular/forms';
import { UnifiedInputComponent } from '../unified-input/unified-input.component';
import { UnifiedSelectComponent } from '../unified-select/unified-select.component';

export interface FilterOption {
  label: string;
  value: any;
}

@Component({
  selector: 'app-filter-field',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    UnifiedInputComponent,
    UnifiedSelectComponent
  ],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => FilterFieldComponent),
      multi: true
    }
  ],
  templateUrl: './filter-field.component.html'
})
export class FilterFieldComponent implements ControlValueAccessor {
  @Input() label: string = '';
  @Input() placeholder: string = '';
  @Input() type: string = 'text';
  @Input() options: FilterOption[] = [];
  @Input() disabled: boolean = false;
  @Input() required: boolean = false;
  @Input() icon: string = '';
  @Input() customClass: string = '';
  @Input() showClear: boolean = false;
  @Output() clear = new EventEmitter<void>();
  @Output() keyup = new EventEmitter<KeyboardEvent>();

  value: any = null;
  private onChange = (value: any) => {};
  private onTouched = () => {};

  writeValue(value: any): void {
    this.value = value || null;
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

  get filterOptions(): any[] {
    return [{ label: 'All', value: '' }, ...this.options];
  }

  onValueChange(value: any): void {
    this.value = value;
    this.onChange(this.value);
    this.onTouched();
  }

  onKeyUp(event: KeyboardEvent): void {
    this.keyup.emit(event);
  }

  onClear(): void {
    this.value = null;
    this.onChange(null);
    this.clear.emit();
  }
}

