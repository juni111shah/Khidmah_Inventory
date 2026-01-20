# Complete Component Library Guide

This guide covers all reusable components in the Khidmah Inventory system with full customization support.

## 📚 Table of Contents

1. [Settings System](#settings-system)
2. [Form Components](#form-components)
3. [Layout Components](#layout-components)
4. [Display Components](#display-components)
5. [Navigation Components](#navigation-components)
6. [Utility Components](#utility-components)
7. [Usage Examples](#usage-examples)

## ⚙️ Settings System

### ComponentSettingsService

All components can be customized through the `ComponentSettingsService`:

```typescript
import { ComponentSettingsService } from './shared/services/component-settings.service';

constructor(private settingsService: ComponentSettingsService) {}

// Set settings for a specific component
this.settingsService.setButtonSettings('save-button', {
  variant: 'primary',
  size: 'lg',
  borderRadius: '12px',
  backgroundColor: '#673AB7'
});

// Get settings
const settings = this.settingsService.getButtonSettings('save-button');
```

### Settings Panel

Use the settings panel to manage all component settings:

```html
<app-settings-panel></app-settings-panel>
```

## 📝 Form Components

### UnifiedButtonComponent

**Usage:**
```html
<app-unified-button
  id="save-btn"
  variant="primary"
  style="material"
  size="lg"
  icon="save"
  iconLibrary="material"
  [loading]="isLoading"
  (clicked)="onSave()">
  Save
</app-unified-button>
```

**Settings:**
- `variant`: 'primary' | 'secondary' | 'success' | 'danger' | 'warning' | 'info'
- `size`: 'xs' | 'sm' | 'md' | 'lg' | 'xl'
- `style`: 'material' | 'bootstrap' | 'custom'
- `icon`: string
- `iconLibrary`: 'fa' | 'material'
- `outlined`: boolean
- `fullWidth`: boolean
- `loading`: boolean

### UnifiedInputComponent

**Usage:**
```html
<app-unified-input
  id="email-input"
  label="Email"
  type="email"
  style="material"
  icon="email"
  iconLibrary="material"
  [(ngModel)]="email"
  required="true">
</app-unified-input>
```

### UnifiedSelectComponent

**Usage:**
```html
<app-unified-select
  id="category-select"
  label="Category"
  [options]="categories"
  style="material"
  [(ngModel)]="selectedCategory">
</app-unified-select>
```

**Options:**
```typescript
categories = [
  { value: '1', label: 'Electronics', icon: 'devices', iconLibrary: 'material' },
  { value: '2', label: 'Clothing', icon: 'fa-tshirt', iconLibrary: 'fa' },
  { value: '3', label: 'Food', group: 'Other' }
];
```

## 📊 Layout Components

### UnifiedTableComponent

**Usage:**
```html
<app-unified-table
  id="products-table"
  [columns]="columns"
  [data]="products"
  style="material"
  [pagination]="true"
  [sorting]="true"
  [showCheckbox]="true"
  [showActions]="true">
  
  <ng-container slot="actions" let-row>
    <app-unified-button variant="primary" size="sm" (clicked)="edit(row)">
      Edit
    </app-unified-button>
  </ng-container>
</app-unified-table>
```

**Columns:**
```typescript
columns = [
  { key: 'id', label: 'ID', sortable: true },
  { key: 'name', label: 'Name', sortable: true, filterable: true },
  { key: 'price', label: 'Price', sortable: true, align: 'right' },
  { key: 'actions', label: 'Actions', template: actionsTemplate }
];
```

### UnifiedModalComponent

**Usage:**
```html
<app-unified-modal
  id="confirm-modal"
  [isOpen]="showModal"
  size="md"
  headerTitle="Confirm Action"
  [showHeader]="true"
  [showFooter]="true"
  (closed)="showModal = false">
  
  <p>Are you sure you want to proceed?</p>
  
  <ng-container slot="footer">
    <app-unified-button variant="secondary" (clicked)="showModal = false">
      Cancel
    </app-unified-button>
    <app-unified-button variant="primary" (clicked)="confirm()">
      Confirm
    </app-unified-button>
  </ng-container>
</app-unified-modal>
```

### UnifiedCardComponent

**Usage:**
```html
<app-unified-card
  id="product-card"
  style="material"
  elevation="3"
  header="Product Details"
  footer="Last updated: Today"
  [hoverable]="true"
  [clickable]="true">
  
  <p>Card content here</p>
</app-unified-card>
```

## 🎨 Display Components

### UnifiedBadgeComponent (to be created)

**Usage:**
```html
<app-unified-badge
  id="notification-badge"
  variant="danger"
  content="5"
  position="top-right">
</app-unified-badge>
```

### UnifiedAlertComponent (to be created)

**Usage:**
```html
<app-unified-alert
  id="success-alert"
  variant="success"
  title="Success!"
  message="Your changes have been saved."
  [dismissible]="true"
  icon="check_circle"
  iconLibrary="material">
</app-unified-alert>
```

### UnifiedProgressComponent (to be created)

**Usage:**
```html
<app-unified-progress
  id="upload-progress"
  [value]="uploadProgress"
  variant="primary"
  [showLabel]="true"
  [striped]="true"
  [animated]="true">
</app-unified-progress>
```

## 🧭 Navigation Components

### UnifiedBreadcrumbComponent (to be created)

**Usage:**
```html
<app-unified-breadcrumb
  id="page-breadcrumb"
  [items]="breadcrumbItems"
  separator="slash"
  [showIcons]="true">
</app-unified-breadcrumb>
```

### UnifiedPaginationComponent

**Usage:**
```html
<app-pagination
  [currentPage]="currentPage"
  [totalPages]="totalPages"
  [pageSize]="pageSize"
  [showFirstLast]="true"
  (pageChange)="onPageChange($event)">
</app-pagination>
```

## 🔧 Utility Components

### UnifiedTooltipComponent (to be created)

**Usage:**
```html
<app-unified-tooltip
  id="help-tooltip"
  content="This is a helpful tooltip"
  position="top"
  trigger="hover">
  <button>Hover me</button>
</app-unified-tooltip>
```

### UnifiedDropdownComponent (to be created)

**Usage:**
```html
<app-unified-dropdown
  id="user-menu"
  [items]="menuItems"
  trigger="click"
  position="bottom-right">
  <button>Menu</button>
</app-unified-dropdown>
```

## 💡 Complete Example

```typescript
import { Component, OnInit } from '@angular/core';
import { ComponentSettingsService } from './shared/services/component-settings.service';

@Component({
  template: `
    <app-unified-card id="main-card" style="material" elevation="2" header="Product Form">
      <form>
        <app-unified-input
          id="name-input"
          label="Product Name"
          [(ngModel)]="product.name"
          required="true">
        </app-unified-input>

        <app-unified-select
          id="category-select"
          label="Category"
          [options]="categories"
          [(ngModel)]="product.category">
        </app-unified-select>

        <div class="actions">
          <app-unified-button
            id="cancel-btn"
            variant="secondary"
            (clicked)="onCancel()">
            Cancel
          </app-unified-button>
          
          <app-unified-button
            id="save-btn"
            variant="primary"
            [loading]="isSaving"
            (clicked)="onSave()">
            Save
          </app-unified-button>
        </div>
      </form>
    </app-unified-card>
  `
})
export class ProductFormComponent implements OnInit {
  product = { name: '', category: '' };
  categories = [
    { value: '1', label: 'Electronics' },
    { value: '2', label: 'Clothing' }
  ];
  isSaving = false;

  constructor(private settingsService: ComponentSettingsService) {}

  ngOnInit(): void {
    // Customize components via settings
    this.settingsService.setButtonSettings('save-btn', {
      size: 'lg',
      borderRadius: '12px'
    });
  }

  onSave(): void {
    this.isSaving = true;
    // Save logic...
  }

  onCancel(): void {
    // Cancel logic...
  }
}
```

## 🎯 Best Practices

1. **Always use IDs**: Set `id` on components to enable settings customization
2. **Consistent Styling**: Use the same `style` prop across related components
3. **Settings Service**: Use ComponentSettingsService for dynamic customization
4. **Reusability**: Create component presets using the settings service
5. **Accessibility**: Always provide labels, tooltips, and ARIA attributes

## 📦 Component Status

### ✅ Completed
- UnifiedButtonComponent
- UnifiedCardComponent
- UnifiedInputComponent
- UnifiedSelectComponent
- UnifiedTableComponent
- UnifiedModalComponent
- PaginationComponent
- SettingsPanelComponent

### 🚧 To Be Created
- UnifiedTextareaComponent
- UnifiedCheckboxComponent
- UnifiedRadioComponent
- UnifiedSwitchComponent
- UnifiedDatePickerComponent
- UnifiedFileUploadComponent
- UnifiedDrawerComponent
- UnifiedListComponent
- UnifiedMenuComponent
- UnifiedBadgeComponent
- UnifiedAlertComponent
- UnifiedProgressComponent
- UnifiedTabsComponent
- UnifiedAccordionComponent
- UnifiedTooltipComponent
- UnifiedPopoverComponent
- UnifiedDropdownComponent
- UnifiedBreadcrumbComponent
- UnifiedStepperComponent
- UnifiedChipComponent
- UnifiedAvatarComponent
- UnifiedSkeletonComponent
- UnifiedDividerComponent
- UnifiedSpacerComponent
- UnifiedGridComponent
- UnifiedContainerComponent

## 🔄 Settings Persistence

All settings are automatically saved to localStorage and persist across sessions. Settings can also be:
- Exported as JSON
- Imported from JSON
- Reset to defaults
- Managed via the Settings Panel

## 📖 Additional Resources

- See `component-settings.model.ts` for all available settings interfaces
- See `component-settings.service.ts` for service methods
- See individual component files for detailed prop documentation

