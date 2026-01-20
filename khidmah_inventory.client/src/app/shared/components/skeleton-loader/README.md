# Skeleton Loader Components

A comprehensive set of reusable skeleton loader components for displaying loading states throughout the application.

## Components

### 1. SkeletonLoaderComponent (Base Component)
The base skeleton loader component that all other components use.

**Usage:**
```html
<app-skeleton-loader
  [width]="'100px'"
  [height]="'20px'"
  [shape]="'rounded'"
  [animation]="'shimmer'">
</app-skeleton-loader>
```

**Inputs:**
- `width`: Width of the skeleton (default: '100%')
- `height`: Height of the skeleton (default: '20px')
- `shape`: Shape type - 'circle' | 'rounded' | 'rectangle' (default: 'rounded')
- `animation`: Animation type - 'pulse' | 'wave' | 'shimmer' (default: 'shimmer')
- `borderRadius`: Custom border radius (optional)

---

### 2. SkeletonButtonComponent
Skeleton loader for buttons.

**Usage:**
```html
<app-skeleton-button
  [width]="'120px'"
  [height]="'36px'"
  [block]="false"
  [animation]="'shimmer'">
</app-skeleton-button>
```

**Inputs:**
- `width`: Button width (default: '120px')
- `height`: Button height (default: '36px')
- `block`: Full width button (default: false)
- `animation`: Animation type (default: 'shimmer')

---

### 3. SkeletonFieldComponent
Skeleton loader for form fields with optional label.

**Usage:**
```html
<app-skeleton-field
  [showLabel]="true"
  [labelWidth]="'100px'"
  [fieldWidth]="'100%'"
  [fieldHeight]="'40px'"
  [animation]="'shimmer'">
</app-skeleton-field>
```

**Inputs:**
- `showLabel`: Show label skeleton (default: true)
- `labelWidth`: Label width (default: '80px')
- `fieldWidth`: Field width (default: '100%')
- `fieldHeight`: Field height (default: '40px')
- `animation`: Animation type (default: 'shimmer')

---

### 4. SkeletonCardComponent
Skeleton loader for cards with header, body, and footer.

**Usage:**
```html
<app-skeleton-card
  [showHeader]="true"
  [showHeaderActions]="false"
  [headerTitleWidth]="'200px'"
  [showFooter]="false"
  [hasContent]="false"
  [contentLines]="contentLines"
  [animation]="'shimmer'">
</app-skeleton-card>
```

**Inputs:**
- `showHeader`: Show header skeleton (default: true)
- `showHeaderActions`: Show header actions (default: false)
- `headerTitleWidth`: Header title width (default: '200px')
- `showFooter`: Show footer skeleton (default: false)
- `hasContent`: Has custom content via ng-content (default: false)
- `contentLines`: Array of content line configurations
- `animation`: Animation type (default: 'shimmer')

**Content Lines Example:**
```typescript
contentLines = [
  { width: '100%', height: '16px' },
  { width: '90%', height: '16px' },
  { width: '80%', height: '16px' }
];
```

---

### 5. SkeletonTableComponent
Skeleton loader for tables with configurable headers and rows.

**Usage:**
```html
<app-skeleton-table
  [showHeader]="true"
  [headers]="headers"
  [rows]="rows"
  [rowCount]="5"
  [animation]="'shimmer'">
</app-skeleton-table>
```

**Inputs:**
- `showHeader`: Show table header (default: true)
- `headers`: Array of header configurations with width
- `rows`: Array of row configurations (optional, auto-generated if rowCount provided)
- `rowCount`: Number of rows to generate (default: 5)
- `animation`: Animation type (default: 'shimmer')

**Headers Example:**
```typescript
headers = [
  { width: '25%' },
  { width: '25%' },
  { width: '25%' },
  { width: '25%' }
];
```

---

### 6. SkeletonListComponent
Skeleton loader for lists with optional avatars.

**Usage:**
```html
<app-skeleton-list
  [items]="items"
  [itemCount]="3"
  [showAvatar]="false"
  [avatarSize]="'40px'"
  [direction]="'vertical'"
  [animation]="'shimmer'">
</app-skeleton-list>
```

**Inputs:**
- `items`: Array of item configurations (optional, auto-generated if itemCount provided)
- `itemCount`: Number of items to generate (default: 3)
- `showAvatar`: Show avatar skeleton (default: false)
- `avatarSize`: Avatar size (default: '40px')
- `direction`: Layout direction - 'vertical' | 'horizontal' (default: 'vertical')
- `animation`: Animation type (default: 'shimmer')

---

### 7. SkeletonFormComponent
Skeleton loader for forms with multiple fields.

**Usage:**
```html
<app-skeleton-form
  [fields]="fields"
  [fieldCount]="4"
  [showLabels]="true"
  [showActions]="true"
  [defaultLabelWidth]="'100px'"
  [defaultFieldHeight]="'40px'"
  [fieldSpacing]="'20px'"
  [animation]="'shimmer'">
</app-skeleton-form>
```

**Inputs:**
- `fields`: Array of field configurations (optional, auto-generated if fieldCount provided)
- `fieldCount`: Number of fields to generate (default: 4)
- `showLabels`: Show field labels (default: true)
- `showActions`: Show form actions (default: true)
- `defaultLabelWidth`: Default label width (default: '100px')
- `defaultFieldHeight`: Default field height (default: '40px')
- `fieldSpacing`: Spacing between fields (default: '20px')
- `animation`: Animation type (default: 'shimmer')

---

## Integration Examples

### In a Component Template

```html
<!-- Loading State -->
<div *ngIf="loading">
  <app-skeleton-card
    [showHeader]="true"
    [contentLines]="[
      { width: '100%', height: '16px' },
      { width: '80%', height: '16px' }
    ]">
  </app-skeleton-card>
</div>

<!-- Content -->
<div *ngIf="!loading">
  <!-- Actual content -->
</div>
```

### In Data Table (Already Integrated)

The `DataTableComponent` automatically uses `SkeletonTableComponent` when `loading` is true.

### In Forms

```html
<form *ngIf="!loading">
  <!-- Form fields -->
</form>

<app-skeleton-form
  *ngIf="loading"
  [fieldCount]="5"
  [showActions]="true">
</app-skeleton-form>
```

### In Lists

```html
<div *ngIf="loading">
  <app-skeleton-list
    [itemCount]="5"
    [showAvatar]="true"
    [avatarSize]="'48px'">
  </app-skeleton-list>
</div>
```

---

## Animation Types

1. **shimmer** (default): Smooth shimmer effect
2. **pulse**: Pulsing opacity effect
3. **wave**: Wave animation effect

---

## Best Practices

1. **Match Actual Content**: Make skeleton loaders match the size and layout of actual content
2. **Use Appropriate Component**: Use the most specific skeleton component (e.g., SkeletonTableComponent for tables)
3. **Consistent Animation**: Use the same animation type throughout the app
4. **Performance**: Skeleton loaders are lightweight and don't impact performance
5. **Accessibility**: Skeleton loaders provide visual feedback during loading states

---

## Styling

Skeleton loaders use CSS variables for theming:
- `--color-white`: Background color
- `--color-border`: Border color
- `--border-radius`: Border radius
- `--box-shadow-sm`: Shadow

Customize these variables in your global styles for consistent theming.

