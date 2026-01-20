# Shared Components - Quick Start Guide

## 🚀 What Was Created

A comprehensive shared component library with **centralized styling** for all common UI elements (fields, buttons, etc.) that can be used throughout the entire application.

## 📦 Components Created

### Form Fields
1. ✅ **FormFieldComponent** - Universal form field (text, select, textarea, date, etc.)
2. ✅ **UnifiedInputComponent** - Text input with Material UI
3. ✅ **UnifiedTextareaComponent** - Multi-line text input
4. ✅ **UnifiedSelectComponent** - Dropdown select
5. ✅ **UnifiedCheckboxComponent** - Checkbox
6. ✅ **UnifiedRadioComponent** - Radio button group
7. ✅ **UnifiedDatePickerComponent** - Date picker
8. ✅ **UnifiedFileUploadComponent** - File upload with preview

### Buttons
9. ✅ **UnifiedButtonComponent** - Enhanced button with variants, sizes, icons, loading states

### Infrastructure
10. ✅ **Index File** (`index.ts`) - Easy imports for all components
11. ✅ **Centralized Styles** (`_shared-components.scss`) - All styling in one place
12. ✅ **Comprehensive README** - Full documentation

## 🎨 Key Features

### Centralized Styling
- All components use `_shared-components.scss` for consistent styling
- Standard dimensions: **40px height** for all form fields
- **0px border radius** (no rounded corners)
- **14px font size** (standard)

### Material UI First
- All components use Angular Material UI
- Consistent appearance across the app
- Built-in accessibility

### Easy to Use
```typescript
// Import from index
import { 
  UnifiedButtonComponent, 
  FormFieldComponent,
  UnifiedTextareaComponent 
} from '@shared/components';
```

## 📝 Quick Examples

### Button
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

### Input Field
```html
<app-form-field
  label="Product Name"
  type="text"
  [(ngModel)]="productName"
  [required]="true">
</app-form-field>
```

### Textarea
```html
<app-unified-textarea
  label="Description"
  [(ngModel)]="description"
  [rows]="4"
  [maxLength]="500">
</app-unified-textarea>
```

### Select
```html
<app-form-field
  label="Category"
  type="select"
  [options]="categories"
  [(ngModel)]="selectedCategory">
</app-form-field>
```

### Checkbox
```html
<app-unified-checkbox
  label="I agree"
  [(ngModel)]="agreed"
  [required]="true">
</app-unified-checkbox>
```

### Date Picker
```html
<app-unified-date-picker
  label="Select Date"
  [(ngModel)]="selectedDate">
</app-unified-date-picker>
```

## 🎯 Benefits

1. **Fast Development**: Use pre-built components instead of writing from scratch
2. **Consistent UI**: All components share the same styling
3. **Easy Maintenance**: Change styles in one place (`_shared-components.scss`)
4. **Type Safety**: Full TypeScript support with proper types
5. **Accessibility**: Built-in ARIA support from Material UI
6. **Form Integration**: Works with both template-driven and reactive forms

## 📖 Full Documentation

See `khidmah_inventory.client/src/app/shared/components/README.md` for complete documentation with all props, examples, and usage patterns.

## 🔄 Next Steps

1. **Start Using**: Replace existing form fields and buttons with shared components
2. **Customize**: Modify `_shared-components.scss` to adjust global styles
3. **Extend**: Add new components following the same pattern

---

**All styling is centralized in `_shared-components.scss` - modify this file to change styles across the entire app!**

