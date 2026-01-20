/**
 * Shared Components Library
 * 
 * This file exports all reusable UI components for easy import throughout the application.
 * All components use Material UI and have centralized styling.
 * 
 * Usage:
 * import { UnifiedButtonComponent, FormFieldComponent } from '@shared/components';
 */

// Buttons
export { UnifiedButtonComponent } from './unified-button/unified-button.component';
export type { ButtonVariant, ButtonSize, ButtonType } from './unified-button/unified-button.component';

// Form Fields
export { FormFieldComponent } from './form-field/form-field.component';
export type { FormFieldType, FormFieldOption } from './form-field/form-field.component';

export { FilterFieldComponent } from './filter-field/filter-field.component';
export type { FilterOption } from './filter-field/filter-field.component';

export { GenericFilterComponent } from './generic-filter/generic-filter.component';
export type { FilterField, AppliedFilter } from './generic-filter/generic-filter.component';

export { FilterPanelComponent } from './filter-panel/filter-panel.component';
export type { FilterPanelField, FilterPanelConfig } from './filter-panel/filter-panel.component';

export { UnifiedInputComponent } from './unified-input/unified-input.component';
export type { InputSize } from './unified-input/unified-input.component';

export { UnifiedSelectComponent } from './unified-select/unified-select.component';

export { UnifiedTextareaComponent } from './unified-textarea/unified-textarea.component';
export type { TextareaSize } from './unified-textarea/unified-textarea.component';

export { UnifiedCheckboxComponent } from './unified-checkbox/unified-checkbox.component';

export { UnifiedRadioComponent } from './unified-radio/unified-radio.component';

export { UnifiedDatePickerComponent } from './unified-date-picker/unified-date-picker.component';

export { UnifiedFileUploadComponent } from './unified-file-upload/unified-file-upload.component';

// Layout Components
export { UnifiedCardComponent } from './unified-card/unified-card.component';
export type { CardVariant, CardElevation } from './unified-card/unified-card.component';

export { UnifiedModalComponent } from './unified-modal/unified-modal.component';

export { UnifiedTableComponent } from './unified-table/unified-table.component';
export type { TableColumn } from './unified-table/unified-table.component';

// Other Components
export { BadgeComponent } from './badge/badge.component';
export { LoadingSpinnerComponent } from './loading-spinner/loading-spinner.component';
export { ToastComponent } from './toast/toast.component';
export { IconComponent } from './icon/icon.component';
export { PaginationComponent } from './pagination/pagination.component';

export { ExportComponent } from './export/export.component';
export type { ExportFormat, ExportOptions } from '../../core/services/export.service';

