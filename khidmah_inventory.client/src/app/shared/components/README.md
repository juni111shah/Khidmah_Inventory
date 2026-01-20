# Shared Components Library

This directory contains all reusable UI components for the Khidmah Inventory application. All components use **Material UI** and have **centralized styling** for consistency across the entire application.

## 📦 Quick Import

Import components easily from the index file:

```typescript
import { 
  UnifiedButtonComponent, 
  FormFieldComponent,
  UnifiedTextareaComponent,
  UnifiedCheckboxComponent 
} from '@shared/components';
```

## 🎨 Design Principles

1. **Material UI First**: All components use Angular Material UI components
2. **Consistent Styling**: All components share common styles from `_shared-components.scss`
3. **Standard Dimensions**: 
   - Form fields: **40px height** (fixed)
   - Font size: **14px** (standard)
   - Border radius: **0px** (no rounded corners)
4. **Accessibility**: All components include proper ARIA attributes
5. **Reusability**: Components work with both template-driven and reactive forms

## 📚 Available Components

### Form Fields

#### 1. FormFieldComponent (`app-form-field`)
**Universal form field** supporting text, select, textarea, date, number, email, tel, and password.

```html
<app-form-field
  label="Product Name"
  type="text"
  [(ngModel)]="productName"
  [required]="true"
  placeholder="Enter product name">
</app-form-field>

<app-form-field
  label="Category"
  type="select"
  [options]="categoryOptions"
  [(ngModel)]="selectedCategory">
</app-form-field>
```

**Props:**
- `label`: Field label
- `type`: 'text' | 'select' | 'number' | 'date' | 'email' | 'tel' | 'password' | 'textarea'
- `options`: Array of options (for select type)
- `required`: boolean
- `disabled`: boolean
- `error`: Error message
- `hint`: Helper text
- `icon`: Icon name
- `iconPosition`: 'left' | 'right'
- `rows`: Number of rows (for textarea)

---

#### 2. UnifiedInputComponent (`app-unified-input`)
**Text input field** with Material UI styling.

```html
<app-unified-input
  label="Email"
  type="email"
  [(ngModel)]="email"
  [required]="true"
  icon="email"
  placeholder="Enter email">
</app-unified-input>
```

**Props:**
- `label`: Field label
- `type`: Input type (text, email, password, number, etc.)
- `size`: 'sm' | 'md' | 'lg'
- `icon`: Icon name
- `iconLibrary`: 'fa' | 'material'
- `required`: boolean
- `disabled`: boolean
- `error`: Error message
- `hint`: Helper text

---

#### 3. UnifiedTextareaComponent (`app-unified-textarea`)
**Multi-line text input** with character count support.

```html
<app-unified-textarea
  label="Description"
  [(ngModel)]="description"
  [rows]="4"
  [maxLength]="500"
  placeholder="Enter description">
</app-unified-textarea>
```

**Props:**
- `label`: Field label
- `rows`: Number of rows
- `maxLength`: Maximum character count
- `resize`: 'none' | 'both' | 'horizontal' | 'vertical'
- `size`: 'sm' | 'md' | 'lg'
- `required`: boolean
- `disabled`: boolean

---

#### 4. UnifiedSelectComponent (`app-unified-select`)
**Dropdown select** with search and grouping support.

```html
<app-unified-select
  label="Category"
  [options]="categoryOptions"
  [(ngModel)]="selectedCategory"
  [multiple]="false"
  [searchable]="true">
</app-unified-select>
```

**Props:**
- `label`: Field label
- `options`: Array of `{ value: any, label: string, group?: string }`
- `multiple`: Allow multiple selection
- `searchable`: Enable search
- `clearable`: Show clear button
- `size`: 'sm' | 'md' | 'lg'

---

#### 5. UnifiedCheckboxComponent (`app-unified-checkbox`)
**Checkbox** with Material UI styling.

```html
<app-unified-checkbox
  label="I agree to the terms"
  [(ngModel)]="agreed"
  [required]="true">
</app-unified-checkbox>
```

**Props:**
- `label`: Checkbox label
- `labelPosition`: 'before' | 'after'
- `color`: 'primary' | 'accent' | 'warn'
- `indeterminate`: boolean
- `required`: boolean
- `disabled`: boolean

---

#### 6. UnifiedRadioComponent (`app-unified-radio`)
**Radio button group** with vertical or horizontal layout.

```html
<app-unified-radio
  label="Select Option"
  [options]="radioOptions"
  [(ngModel)]="selectedOption"
  layout="vertical">
</app-unified-radio>
```

**Props:**
- `label`: Group label
- `options`: Array of `{ value: any, label: string, disabled?: boolean }`
- `layout`: 'vertical' | 'horizontal'
- `color`: 'primary' | 'accent' | 'warn'
- `required`: boolean
- `disabled`: boolean

---

#### 7. UnifiedDatePickerComponent (`app-unified-date-picker`)
**Date picker** with Material UI calendar.

```html
<app-unified-date-picker
  label="Select Date"
  [(ngModel)]="selectedDate"
  [minDate]="minDate"
  [maxDate]="maxDate">
</app-unified-date-picker>
```

**Props:**
- `label`: Field label
- `minDate`: Minimum selectable date
- `maxDate`: Maximum selectable date
- `startView`: 'month' | 'year' | 'multi-year'
- `touchUI`: boolean
- `required`: boolean
- `disabled`: boolean

---

#### 8. UnifiedFileUploadComponent (`app-unified-file-upload`)
**File upload** with preview and progress support.

```html
<app-unified-file-upload
  label="Upload Image"
  accept="image/*"
  [maxSize]="5242880"
  [showPreview]="true"
  (fileSelected)="onFileSelected($event)">
</app-unified-file-upload>
```

**Props:**
- `label`: Upload label
- `accept`: File types (e.g., "image/*", ".pdf")
- `multiple`: Allow multiple files
- `maxSize`: Maximum file size in bytes
- `showPreview`: Show file preview
- `previewType`: 'image' | 'file'
- `buttonVariant`: Button variant
- `buttonSize`: Button size

**Events:**
- `fileSelected`: Emitted when files are selected
- `fileRemoved`: Emitted when a file is removed

---

### Buttons

#### UnifiedButtonComponent (`app-unified-button`)
**Reusable button** with loading states, icons, and variants.

```html
<app-unified-button
  variant="primary"
  size="md"
  icon="save"
  [loading]="isSaving"
  (clicked)="onSave()">
  Save
</app-unified-button>
```

**Props:**
- `variant`: 'primary' | 'secondary' | 'success' | 'danger' | 'warning' | 'info' | 'accent'
- `size`: 'xs' | 'sm' | 'md' | 'lg' | 'xl'
- `type`: 'button' | 'submit' | 'reset'
- `icon`: Icon name
- `iconPosition`: 'left' | 'right'
- `iconLibrary`: 'fa' | 'material'
- `loading`: Show loading spinner
- `disabled`: Disable button
- `outlined`: Use outlined style
- `raised`: Use raised style (default: true)
- `fullWidth`: Full width button
- `tooltip`: Tooltip text

**Events:**
- `clicked`: Emitted on button click

---

### Layout Components

#### UnifiedCardComponent (`app-unified-card`)
**Card container** with header, footer, and elevation.

```html
<app-unified-card
  header="Card Title"
  footer="Card Footer"
  elevation="2"
  [hoverable]="true">
  Card content here
</app-unified-card>
```

---

#### UnifiedModalComponent (`app-unified-modal`)
**Modal dialog** with customizable size and footer buttons.

```html
<app-unified-modal
  [isOpen]="showModal"
  size="md"
  headerTitle="Confirm Action"
  (closed)="showModal = false">
  Modal content
</app-unified-modal>
```

---

#### UnifiedTableComponent (`app-unified-table`)
**Data table** with sorting, pagination, and actions.

```html
<app-unified-table
  [columns]="columns"
  [data]="data"
  [pagination]="true"
  [sorting]="true">
</app-unified-table>
```

---

## 🎨 Styling

All components use centralized styles from `_shared-components.scss`. Key styling rules:

### Standard Dimensions
- **Form Fields**: 40px height (fixed)
- **Font Size**: 14px (standard)
- **Border Radius**: 0px (no rounded corners)
- **Padding**: 8px 12px (inputs)

### CSS Variables
Components use CSS variables for theming:
- `--primary-color`: Primary color
- `--secondary-color`: Secondary color
- `--error-color`: Error color
- `--success-color`: Success color
- `--warning-color`: Warning color
- `--font-family`: Font family
- `--transition-duration`: Transition duration
- `--transition-timing`: Transition timing function

## 📝 Usage Examples

### Complete Form Example

```html
<form>
  <app-form-field
    label="Product Name"
    type="text"
    [(ngModel)]="product.name"
    [required]="true">
  </app-form-field>

  <app-form-field
    label="Category"
    type="select"
    [options]="categories"
    [(ngModel)]="product.categoryId">
  </app-form-field>

  <app-unified-textarea
    label="Description"
    [(ngModel)]="product.description"
    [rows]="4">
  </app-unified-textarea>

  <app-unified-date-picker
    label="Expiry Date"
    [(ngModel)]="product.expiryDate">
  </app-unified-date-picker>

  <div class="form-actions">
    <app-unified-button
      variant="secondary"
      (clicked)="onCancel()">
      Cancel
    </app-unified-button>

    <app-unified-button
      variant="primary"
      icon="save"
      [loading]="isSaving"
      (clicked)="onSave()">
      Save
    </app-unified-button>
  </div>
</form>
```

### Reactive Forms Example

```typescript
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

export class ProductFormComponent {
  form: FormGroup;

  constructor(private fb: FormBuilder) {
    this.form = this.fb.group({
      name: ['', Validators.required],
      categoryId: ['', Validators.required],
      description: [''],
      price: [0, [Validators.required, Validators.min(0)]]
    });
  }
}
```

```html
<form [formGroup]="form">
  <app-form-field
    label="Product Name"
    type="text"
    formControlName="name">
  </app-form-field>

  <app-form-field
    label="Category"
    type="select"
    [options]="categories"
    formControlName="categoryId">
  </app-form-field>
</form>
```

## 🔧 Component Development

When creating new shared components:

1. **Use Material UI**: Always use Angular Material components
2. **Import Shared Styles**: Import `_shared-components.scss` for consistency
3. **Implement ControlValueAccessor**: For form fields to work with both template-driven and reactive forms
4. **Add to Index**: Export new components from `index.ts`
5. **Document Props**: Add JSDoc comments for all inputs/outputs
6. **Follow Naming**: Use `Unified` prefix for reusable components

## 📖 Additional Resources

- [Material UI Form Fields Guide](./form-field/README.md)
- [Component Library Guide](../../../COMPONENT_LIBRARY_GUIDE.md)
- [Material UI Migration Guide](../../../MATERIAL_UI_MIGRATION.md)

## ✅ Migration Checklist

When updating existing forms to use shared components:

- [ ] Replace `<input>` with `<app-form-field>` or `<app-unified-input>`
- [ ] Replace `<select>` with `<app-form-field type="select">` or `<app-unified-select>`
- [ ] Replace `<textarea>` with `<app-unified-textarea>`
- [ ] Replace `<button mat-button>` with `<app-unified-button>`
- [ ] Remove custom styling (components handle it)
- [ ] Test form validation
- [ ] Test accessibility

---

**Last Updated**: 2024
**Maintained By**: Development Team
