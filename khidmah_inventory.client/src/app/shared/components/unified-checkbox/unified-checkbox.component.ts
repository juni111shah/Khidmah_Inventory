import { Component, Input, forwardRef, OnChanges, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ControlValueAccessor, FormsModule, NG_VALUE_ACCESSOR, ReactiveFormsModule } from '@angular/forms';

@Component({
  selector: 'app-unified-checkbox',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule
  ],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => UnifiedCheckboxComponent),
      multi: true
    }
  ],
  templateUrl: './unified-checkbox.component.html'
})
export class UnifiedCheckboxComponent implements ControlValueAccessor, OnChanges {
  @Input() label: string = '';
  @Input() labelPosition: 'before' | 'after' = 'after';
  @Input() disabled: boolean = false;
  @Input() required: boolean = false;
  @Input() indeterminate: boolean = false;
  @Input() color: 'primary' | 'accent' | 'warn' = 'primary';
  @Input() customClass: string = '';
  @Input() id: string = 'checkbox-' + Math.random().toString(36).substring(2, 9);
  @Input() checked: boolean = false;

  @Output() change = new EventEmitter<boolean>();

  value: boolean = false;
  private onChange = (value: boolean) => {};
  private onTouched = () => {};

  writeValue(value: boolean): void {
    this.value = value || false;
    this.checked = this.value;
  }

  registerOnChange(fn: (value: boolean) => void): void {
    this.onChange = fn;
  }

  registerOnTouched(fn: () => void): void {
    this.onTouched = fn;
  }

  setDisabledState(isDisabled: boolean): void {
    this.disabled = isDisabled;
  }

  onCheckboxChange(event: Event): void {
    const target = event.target as HTMLInputElement;
    this.value = target.checked;
    this.checked = this.value;
    this.onChange(this.value);
    this.onTouched();
    this.change.emit(this.value);
  }

  ngOnChanges(): void {
    if (this.checked !== this.value) {
      this.value = this.checked;
    }
  }

  get checkboxClasses(): string {
    const classes: string[] = ['form-check'];
    
    if (this.customClass) {
      classes.push(this.customClass);
    }
    
    return classes.join(' ');
  }
}

