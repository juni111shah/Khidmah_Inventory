# Material UI Form Fields - Complete Guide

## Overview

All **input**, **select**, and **textarea** fields throughout the application use **Material UI components** and are implemented through reusable components.

## ✅ Material UI Components Used

### 1. Form Field Component (`app-form-field`)

**Primary reusable component** for all form fields using Material UI:

- **Input**: `<input matInput>` inside `<mat-form-field>`
- **Select**: `<mat-select>` with `<mat-option>` components
- **Textarea**: `<textarea matInput>` inside `<mat-form-field>`
- **Date**: `<input matInput>` with `<mat-datepicker>` component

### 2. Filter Field Component (`app-filter-field`)

**Optimized for filters/search** using the same Material UI components:

- Same Material UI components as form-field
- Includes clear button option
- Same 40px height for consistency

## Material UI Components Breakdown

| Component | Material UI Elements | Description |
|-----------|---------------------|-------------|
| **Input** | `<mat-form-field>`, `<input matInput>`, `<mat-label>`, `<mat-hint>`, `<mat-error>` | Text, number, email, tel, password inputs |
| **Select** | `<mat-form-field>`, `<mat-select>`, `<mat-option>`, `<mat-label>` | Dropdown selections |
| **Textarea** | `<mat-form-field>`, `<textarea matInput>`, `<mat-label>` | Multi-line text input |
| **Date** | `<mat-form-field>`, `<input matInput>`, `<mat-datepicker>`, `<mat-datepicker-toggle>` | Date picker |

## Standard Dimensions

All Material UI form fields have consistent dimensions:

- **Height**: 40px (fixed)
- **Padding**: 8px 12px
- **Font Size**: 14px
- **Line Height**: 1.5
- **Border Radius**: 0px (no rounded corners)
- **Appearance**: `outline` (Material UI outline style)

## Usage Examples

### Text Input (Material UI)

```html
<app-form-field
  label="Product Name"
  type="text"
  [(ngModel)]="productName"
  [required]="true"
  placeholder="Enter product name">
</app-form-field>
```

**Material UI Components Used:**
- `<mat-form-field appearance="outline">`
- `<mat-label>`
- `<input matInput>`

### Select Dropdown (Material UI)

```html
<app-form-field
  label="Category"
  type="select"
  [options]="categoryOptions"
  [(ngModel)]="selectedCategory"
  [required]="true">
</app-form-field>
```

**Material UI Components Used:**
- `<mat-form-field appearance="outline">`
- `<mat-label>`
- `<mat-select>`
- `<mat-option>` (for each option)

### Textarea (Material UI)

```html
<app-form-field
  label="Description"
  type="textarea"
  [(ngModel)]="description"
  [rows]="4"
  placeholder="Enter description">
</app-form-field>
```

**Material UI Components Used:**
- `<mat-form-field appearance="outline">`
- `<mat-label>`
- `<textarea matInput>`

### Date Picker (Material UI)

```html
<app-form-field
  label="Date"
  type="date"
  [(ngModel)]="selectedDate"
  [required]="true">
</app-form-field>
```

**Material UI Components Used:**
- `<mat-form-field appearance="outline">`
- `<mat-label>`
- `<input matInput>` with `[matDatepicker]`
- `<mat-datepicker-toggle>`
- `<mat-datepicker>`

## Material UI Features

All form fields include Material UI features:

1. **Labels**: `<mat-label>` with floating animation
2. **Hints**: `<mat-hint>` for helper text
3. **Errors**: `<mat-error>` for validation messages
4. **Icons**: `<mat-icon>` with `matPrefix` or `matSuffix`
5. **Validation**: Built-in Material UI validation styling
6. **Accessibility**: Full ARIA support from Material UI

## Component Structure

```
<mat-form-field appearance="outline">
  <mat-label>Label</mat-label>
  <mat-icon matPrefix>icon</mat-icon>
  <input matInput />          <!-- or -->
  <mat-select>                <!-- or -->
  <textarea matInput />        <!-- or -->
  <input matInput [matDatepicker]="picker" />
  <mat-datepicker-toggle matIconSuffix [for]="picker"></mat-datepicker-toggle>
  <mat-datepicker #picker></mat-datepicker>
  <mat-icon matSuffix>icon</mat-icon>
  <mat-hint>Hint text</mat-hint>
  <mat-error>Error message</mat-error>
</mat-form-field>
```

## Key Points

✅ **100% Material UI** - All fields use Material UI components  
✅ **Reusable** - Single component for all field types  
✅ **Consistent** - Same height, size, and styling everywhere  
✅ **Accessible** - Full Material UI accessibility features  
✅ **Validated** - Built-in Material UI validation support  

## Migration Checklist

When creating or updating forms:

- [ ] Use `app-form-field` for all inputs
- [ ] Use `app-form-field` for all selects
- [ ] Use `app-form-field` for all textareas
- [ ] Never use raw `<input>`, `<select>`, or `<textarea>` tags
- [ ] All fields will automatically use Material UI components
- [ ] All fields will have consistent 40px height

## Benefits

1. **Consistency**: All fields look and behave the same
2. **Material Design**: Follows Material Design guidelines
3. **Accessibility**: Built-in ARIA support
4. **Validation**: Material UI validation styling
5. **Maintainability**: Single source of truth for all fields
6. **Reusability**: Use the same component everywhere

