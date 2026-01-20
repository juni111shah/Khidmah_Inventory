# Standardize All Fields and Buttons - Migration Guide

## Overview

This guide helps migrate all components to use standardized Material UI components for consistent styling across the entire application.

## Standard Components to Use

### 1. Form Fields
- **Use**: `app-form-field` for all inputs, selects, and textareas
- **Height**: 40px (fixed)
- **Padding**: 8px 12px
- **Font Size**: 14px

### 2. Filter Fields
- **Use**: `app-filter-field` for search/filter inputs
- **Same styling as form-field**

### 3. Buttons
- **Use**: Material UI buttons (`mat-raised-button`, `mat-button`, `mat-stroked-button`)
- **Height**: 40px (fixed)
- **Padding**: 8px 16px
- **Font Size**: 14px

## Migration Patterns

### Replace Raw Select Elements

**Before:**
```html
<select [(ngModel)]="value" class="form-control">
  <option value="1">Option 1</option>
</select>
```

**After:**
```html
<app-form-field
  label="Label"
  type="select"
  [options]="selectOptions"
  [(ngModel)]="value">
</app-form-field>
```

**TypeScript:**
```typescript
import { FormFieldComponent, FormFieldOption } from '../../../shared/components/form-field/form-field.component';

selectOptions: FormFieldOption[] = [
  { value: 1, label: 'Option 1' }
];
```

### Replace Raw Input Elements

**Before:**
```html
<input type="text" [(ngModel)]="value" class="form-control" />
<input type="number" [(ngModel)]="quantity" class="form-control" />
<input type="email" [(ngModel)]="email" class="form-control" />
<input type="date" [(ngModel)]="date" class="form-control" />
```

**After:**
```html
<app-form-field
  label="Text"
  type="text"
  [(ngModel)]="value">
</app-form-field>

<app-form-field
  label="Quantity"
  type="number"
  [(ngModel)]="quantity">
</app-form-field>

<app-form-field
  label="Email"
  type="email"
  [(ngModel)]="email">
</app-form-field>

<app-form-field
  label="Date"
  type="date"
  [(ngModel)]="date">
</app-form-field>
```

### Replace Raw Textarea Elements

**Before:**
```html
<textarea [(ngModel)]="description" class="form-control" rows="3"></textarea>
```

**After:**
```html
<app-form-field
  label="Description"
  type="textarea"
  [(ngModel)]="description"
  [rows]="3">
</app-form-field>
```

### Replace Raw Buttons

**Before:**
```html
<button class="btn btn-primary" (click)="save()">Save</button>
<button class="btn btn-secondary" (click)="cancel()">Cancel</button>
```

**After:**
```html
<button mat-raised-button color="primary" (click)="save()">
  <mat-icon>save</mat-icon>
  Save
</button>

<button mat-stroked-button (click)="cancel()">
  <mat-icon>cancel</mat-icon>
  Cancel
</button>
```

**TypeScript Imports:**
```typescript
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';

imports: [MatButtonModule, MatIconModule]
```

## Components to Update

### High Priority (Visible Components)

1. ✅ **categories-list** - DONE
2. **warehouse-form** - Has many raw inputs
3. **category-form** - Has raw inputs and buttons
4. **role-form** - Has raw inputs and buttons
5. **stock-transfer** - Has raw inputs, selects, and textarea
6. **reports** - Has raw inputs and buttons
7. **user-profile** - Has raw inputs and buttons

### Medium Priority (List Components)

8. **roles-list** - Button needs update
9. **warehouses-list** - Select and button
10. **suppliers-list** - Select and button
11. **customers-list** - Select and button
12. **sales-orders-list** - Select and button
13. **purchase-orders-list** - Select and button
14. **stock-levels-list** - Button

### Low Priority (Other Components)

15. **auth/login** - Form fields
16. **auth/register** - Form fields
17. **barcode-scanner** - Buttons

## Quick Update Checklist

For each component:

- [ ] Replace `<select>` with `app-form-field type="select"`
- [ ] Replace `<input>` with `app-form-field type="text|number|email|date"`
- [ ] Replace `<textarea>` with `app-form-field type="textarea"`
- [ ] Replace `<button class="btn">` with `mat-raised-button` or `mat-stroked-button`
- [ ] Add Material UI imports (`MatButtonModule`, `MatIconModule`, `FormFieldComponent`)
- [ ] Convert options to `FormFieldOption[]` format
- [ ] Test the component

## Example: Complete Component Update

### Before (warehouse-form.component.html)
```html
<input type="text" [(ngModel)]="formData.name" class="form-control" required />
<input type="text" [(ngModel)]="formData.code" class="form-control" />
<textarea [(ngModel)]="formData.description" class="form-control" rows="3"></textarea>
<button (click)="save()" class="btn btn-primary">Save</button>
```

### After (warehouse-form.component.html)
```html
<app-form-field
  label="Name"
  type="text"
  [(ngModel)]="formData.name"
  [required]="true">
</app-form-field>

<app-form-field
  label="Code"
  type="text"
  [(ngModel)]="formData.code">
</app-form-field>

<app-form-field
  label="Description"
  type="textarea"
  [(ngModel)]="formData.description"
  [rows]="3">
</app-form-field>

<button mat-raised-button color="primary" (click)="save()">
  <mat-icon>save</mat-icon>
  Save
</button>
```

### TypeScript Updates
```typescript
import { FormFieldComponent } from '../../../shared/components/form-field/form-field.component';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';

imports: [
  // ... existing imports
  FormFieldComponent,
  MatButtonModule,
  MatIconModule
]
```

## Benefits

✅ **Consistent Height**: All fields are 40px  
✅ **Consistent Styling**: Same padding, font size, and appearance  
✅ **Material Design**: Follows Material Design guidelines  
✅ **Maintainable**: Single source of truth for all fields  
✅ **Accessible**: Built-in Material UI accessibility features  



