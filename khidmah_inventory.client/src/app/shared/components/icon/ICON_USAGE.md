# Icon Usage Guide

This guide explains how to use icons throughout the application using both Font Awesome and Material Icons.

## Icon Component

The `app-icon` component provides a unified way to use icons from different libraries.

### Basic Usage

```html
<!-- Material Icon (default) -->
<app-icon name="home"></app-icon>

<!-- Font Awesome Icon -->
<app-icon name="home" library="fa"></app-icon>

<!-- Material Outlined Icon -->
<app-icon name="home" library="material-outlined"></app-icon>
```

### Component Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `name` | `string` | `''` | Icon name (required) |
| `library` | `'fa' \| 'material' \| 'material-outlined'` | `'material'` | Icon library to use |
| `size` | `'xs' \| 'sm' \| 'md' \| 'lg' \| 'xl' \| 'xxl'` | `'md'` | Icon size |
| `color` | `string` | `''` | Custom color (CSS color value) |
| `customClass` | `string` | `''` | Additional CSS classes |
| `spin` | `boolean` | `false` | Enable spinning animation (FA only) |
| `pulse` | `boolean` | `false` | Enable pulse animation (FA only) |
| `rotate` | `number` | `0` | Rotation angle in degrees |
| `flip` | `'horizontal' \| 'vertical' \| 'both' \| ''` | `''` | Flip icon (FA only) |

### Examples

```html
<!-- Small icon with custom color -->
<app-icon name="save" size="sm" color="#4CAF50"></app-icon>

<!-- Large spinning icon (Font Awesome) -->
<app-icon name="spinner" library="fa" size="lg" [spin]="true"></app-icon>

<!-- Rotated icon -->
<app-icon name="arrow_forward" [rotate]="90"></app-icon>

<!-- Icon with custom class -->
<app-icon name="user" customClass="my-custom-icon"></app-icon>
```

## Using Icon Service

The `IconService` provides common icon names for both libraries:

```typescript
import { IconService } from './shared/services/icon.service';

constructor(private iconService: IconService) {}

// Get Material icon name
const homeIcon = this.iconService.getIcon('home', 'material'); // Returns 'home'

// Get Font Awesome icon name
const homeIconFa = this.iconService.getIcon('home', 'fa'); // Returns 'home' (FA format)

// Use in template
```

```html
<app-icon [name]="iconService.getIcon('home', 'material')"></app-icon>
```

## Font Awesome Icons

### Icon Naming

Font Awesome icons can be used in two ways:

1. **Simple name** (recommended):
   ```html
   <app-icon name="home" library="fa"></app-icon>
   ```
   This automatically adds the `fa-` prefix and uses solid style (`fas`).

2. **Full class name**:
   ```html
   <app-icon name="fas fa-home" library="fa"></app-icon>
   ```
   Use this when you need a specific style (solid, regular, brands, etc.).

### Font Awesome Styles

- `fas` - Solid (default)
- `far` - Regular
- `fab` - Brands
- `fal` - Light
- `fad` - Duotone

### Font Awesome Features

```html
<!-- Spinning icon -->
<app-icon name="spinner" library="fa" [spin]="true"></app-icon>

<!-- Pulsing icon -->
<app-icon name="heart" library="fa" [pulse]="true"></app-icon>

<!-- Rotated icon -->
<app-icon name="arrow-right" library="fa" [rotate]="45"></app-icon>

<!-- Flipped icon -->
<app-icon name="arrow-right" library="fa" flip="horizontal"></app-icon>
```

## Material Icons

### Icon Naming

Material Icons use simple names without prefixes:

```html
<app-icon name="home"></app-icon>
<app-icon name="settings"></app-icon>
<app-icon name="delete"></app-icon>
```

### Material Outlined Icons

For outlined style Material icons:

```html
<app-icon name="home" library="material-outlined"></app-icon>
```

### Finding Material Icons

Visit [Material Icons](https://fonts.google.com/icons) to browse available icons.

## Using Icons in Buttons

The `unified-button` component supports icons:

```html
<!-- Material icon button -->
<app-unified-button icon="save" iconLibrary="material">
  Save
</app-unified-button>

<!-- Font Awesome icon button -->
<app-unified-button icon="save" iconLibrary="fa">
  Save
</app-unified-button>

<!-- Icon on the right -->
<app-unified-button icon="arrow_forward" iconPosition="right" iconLibrary="material">
  Next
</app-unified-button>
```

## Using Icons in Other Components

### In Templates

```html
<div class="my-component">
  <app-icon name="info" size="sm" color="blue"></app-icon>
  <span>Information</span>
</div>
```

### With Conditional Rendering

```html
<app-icon 
  *ngIf="isLoading"
  name="spinner" 
  library="fa" 
  [spin]="true"
  size="lg">
</app-icon>
```

### With ngFor

```html
<div *ngFor="let item of items">
  <app-icon [name]="item.icon" [library]="item.library"></app-icon>
  <span>{{ item.label }}</span>
</div>
```

## Icon Sizes

Available sizes:
- `xs` - 0.75rem (12px)
- `sm` - 0.875rem (14px)
- `md` - 1rem (16px) - default
- `lg` - 1.25rem (20px)
- `xl` - 1.5rem (24px)
- `xxl` - 2rem (32px)

## Icon Colors

You can set icon colors using:

1. **Color property**:
   ```html
   <app-icon name="check" color="#4CAF50"></app-icon>
   ```

2. **CSS classes** (from global styles):
   ```html
   <app-icon name="check" customClass="icon-success"></app-icon>
   ```

Available color classes:
- `icon-primary`
- `icon-secondary`
- `icon-success`
- `icon-danger`
- `icon-warning`
- `icon-info`

## Best Practices

1. **Consistency**: Choose one icon library per feature or component for consistency
2. **Material Icons**: Use Material Icons when using Angular Material components
3. **Font Awesome**: Use Font Awesome for more variety and special effects (spin, pulse, etc.)
4. **Accessibility**: Always provide text labels or aria-labels when icons are used as buttons
5. **Size**: Use appropriate sizes - `sm` for inline text, `md` for buttons, `lg` for headers
6. **Color**: Use theme colors when possible for consistency

## Common Icon Patterns

### Loading State
```html
<app-icon name="spinner" library="fa" [spin]="true" size="sm"></app-icon>
```

### Success Message
```html
<app-icon name="check_circle" color="#4CAF50" size="lg"></app-icon>
```

### Error Message
```html
<app-icon name="error" color="#F44336" size="lg"></app-icon>
```

### Navigation
```html
<app-icon name="arrow_back" (click)="goBack()"></app-icon>
```

### Actions
```html
<app-icon name="edit" size="sm" (click)="edit()"></app-icon>
<app-icon name="delete" size="sm" color="#F44336" (click)="delete()"></app-icon>
```

