# Form Field Component - Usage Guide

## Overview

The `app-form-field` component is the **standardized, reusable Material UI component** for all input, select, and textarea fields throughout the application. 

### ✅ All Fields Use Material UI Components

- **Input**: Uses `<input matInput>` with Material UI styling
- **Select**: Uses `<mat-select>` with `<mat-option>` components
- **Textarea**: Uses `<textarea matInput>` with Material UI styling
- **Date**: Uses `<input matInput>` with `<mat-datepicker>` component

All fields are wrapped in `<mat-form-field>` with Material UI features:
- `<mat-label>` for labels
- `<mat-hint>` for hints
- `<mat-error>` for error messages
- `<mat-icon>` for icons

This ensures consistent height, size, and styling across the entire app.

## Standard Dimensions

All form fields have the following standardized dimensions:

- **Height**: 40px (fixed)
- **Padding**: 8px 12px
- **Font Size**: 14px
- **Line Height**: 1.5
- **Border Radius**: 0px (no rounded corners)

## Usage

### Basic Text Input

```html
<app-form-field
  label="Product Name"
  type="text"
  [(ngModel)]="productName"
  [required]="true"
  placeholder="Enter product name">
</app-form-field>
```

### Select Dropdown

```html
<app-form-field
  label="Category"
  type="select"
  [options]="categoryOptions"
  [(ngModel)]="selectedCategory"
  [required]="true">
</app-form-field>
```

### Number Input

```html
<app-form-field
  label="Quantity"
  type="number"
  [(ngModel)]="quantity"
  [required]="true">
</app-form-field>
```

### Email Input

```html
<app-form-field
  label="Email"
  type="email"
  [(ngModel)]="email"
  [required]="true"
  placeholder="Enter email address">
</app-form-field>
```

### Textarea

```html
<app-form-field
  label="Description"
  type="textarea"
  [(ngModel)]="description"
  [rows]="4"
  placeholder="Enter description">
</app-form-field>
```

### With Icon

```html
<app-form-field
  label="Search"
  type="text"
  [(ngModel)]="searchTerm"
  icon="search"
  iconPosition="left"
  placeholder="Search...">
</app-form-field>
```

### With Validation

```html
<app-form-field
  label="Email"
  type="email"
  [(ngModel)]="email"
  [required]="true"
  [error]="emailError"
  hint="Enter a valid email address">
</app-form-field>
```

## Input Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `label` | `string` | `''` | Field label |
| `placeholder` | `string` | `''` | Placeholder text |
| `type` | `FormFieldType` | `'text'` | Field type: `'text'`, `'select'`, `'number'`, `'date'`, `'email'`, `'tel'`, `'password'`, `'textarea'` |
| `options` | `FormFieldOption[]` | `[]` | Options for select type |
| `disabled` | `boolean` | `false` | Disable the field |
| `required` | `boolean` | `false` | Mark field as required |
| `error` | `string` | `''` | Error message to display |
| `hint` | `string` | `''` | Hint text to display |
| `icon` | `string` | `''` | Material icon name |
| `iconPosition` | `'left' \| 'right'` | `'left'` | Icon position |
| `customClass` | `string` | `''` | Additional CSS classes |
| `rows` | `number` | `3` | Number of rows for textarea |

## FormFieldOption Interface

```typescript
interface FormFieldOption {
  value: any;
  label: string;
  disabled?: boolean;
}
```

## Reactive Forms Support

The component implements `ControlValueAccessor`, so it works with both template-driven and reactive forms:

### Template-Driven Forms
```html
<app-form-field
  label="Name"
  type="text"
  [(ngModel)]="name"
  name="name">
</app-form-field>
```

### Reactive Forms
```html
<app-form-field
  label="Name"
  type="text"
  [formControl]="nameControl">
</app-form-field>
```

## Filter Fields

For filter/search fields, use `app-filter-field` which has the same styling but optimized for filters:

```html
<app-filter-field
  label="Search"
  type="text"
  [(ngModel)]="searchTerm"
  placeholder="Search..."
  [showClear]="true">
</app-filter-field>
```

## Material UI Components Used

This component uses **100% Material UI components**:

| Field Type | Material UI Component |
|------------|---------------------|
| Text Input | `<input matInput>` inside `<mat-form-field>` |
| Number Input | `<input matInput type="number">` inside `<mat-form-field>` |
| Email Input | `<input matInput type="email">` inside `<mat-form-field>` |
| Password Input | `<input matInput type="password">` inside `<mat-form-field>` |
| Select | `<mat-select>` with `<mat-option>` inside `<mat-form-field>` |
| Textarea | `<textarea matInput>` inside `<mat-form-field>` |
| Date | `<input matInput>` with `<mat-datepicker>` inside `<mat-form-field>` |

All fields are wrapped in `<mat-form-field appearance="outline">` which provides:
- Material Design styling
- Consistent appearance
- Built-in validation support
- Label animations
- Error and hint display

## Important Notes

1. **Always use `app-form-field` or `app-filter-field`** for all inputs, selects, and textareas
2. **Never use raw `<input>`, `<select>`, or `<textarea>` tags** - use the Material UI components instead
3. All fields use Material UI components (`matInput`, `mat-select`, `mat-form-field`)
4. All fields will automatically have the same height (40px) and styling
5. The component handles all Material UI styling internally
6. Border radius is always 0 (no rounded corners)
7. **100% Material UI** - no custom HTML inputs

## Migration Guide

If you have existing forms using raw inputs, replace them:

### Before
```html
<input type="text" [(ngModel)]="name" class="form-control">
<select [(ngModel)]="category" class="form-control">
  <option value="1">Category 1</option>
</select>
```

### After
```html
<app-form-field
  label="Name"
  type="text"
  [(ngModel)]="name">
</app-form-field>

<app-form-field
  label="Category"
  type="select"
  [options]="categoryOptions"
  [(ngModel)]="category">
</app-form-field>
```

