import { Component, Input, forwardRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ControlValueAccessor, FormsModule, NG_VALUE_ACCESSOR, ReactiveFormsModule } from '@angular/forms';
import { UnifiedInputComponent } from '../unified-input/unified-input.component';
import { UnifiedSelectComponent } from '../unified-select/unified-select.component';
import { UnifiedTextareaComponent } from '../unified-textarea/unified-textarea.component';
import { UnifiedDatePickerComponent } from '../unified-date-picker/unified-date-picker.component';

export type FormFieldType = 'text' | 'select' | 'number' | 'date' | 'email' | 'tel' | 'password' | 'textarea';

export interface FormFieldOption {
  value: any;
  label: string;
  disabled?: boolean;
}

@Component({
  selector: 'app-form-field',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    UnifiedInputComponent,
    UnifiedSelectComponent,
    UnifiedTextareaComponent,
    UnifiedDatePickerComponent
  ],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => FormFieldComponent),
      multi: true
    }
  ],
  templateUrl: './form-field.component.html'
})
export class FormFieldComponent implements ControlValueAccessor {
  @Input() label: string = '';
  @Input() placeholder: string = '';
  @Input() type: FormFieldType = 'text';
  @Input() options: FormFieldOption[] = [];
  @Input() disabled: boolean = false;
  @Input() required: boolean = false;
  @Input() error: string = '';
  @Input() hint: string = '';
  @Input() icon: string = '';
  @Input() iconPosition: 'left' | 'right' = 'left';
  @Input() customClass: string = '';
  @Input() rows: number = 3;

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

  onValueChange(value: any): void {
    this.value = value;
    this.onChange(this.value);
    this.onTouched();
  }
}

