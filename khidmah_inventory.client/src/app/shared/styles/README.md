# Shared Styles System

This directory contains the centralized styles system for the application, promoting code reusability and maintainability.

## Structure

```
shared/styles/
├── _variables.css      # CSS custom properties (theme variables)
├── _base.css           # Base/reset styles
├── _mixins.css         # Utility classes (flex, spacing, text, etc.)
├── _animations.css     # Animation keyframes and utilities
├── _components.css     # Shared component base styles
├── _responsive.css     # Responsive utilities
└── index.css           # Main entry point (imports all)
```

## Usage

### 1. CSS Variables (Theme)

All theme variables are defined in `_variables.css` and can be accessed using `var(--variable-name)`:

```css
.my-component {
  color: var(--primary-color);
  padding: var(--spacing-md);
  border-radius: var(--border-radius);
}
```

### 2. Utility Classes

Use utility classes from `_mixins.css` for common patterns:

```html
<div class="flex flex-between gap-md">
  <span class="text-primary font-weight-semibold">Title</span>
  <button class="btn-base btn-raised">Action</button>
</div>
```

### 3. Component Base Styles

Component base styles are defined in `_components.css`:

- `.btn-base` - Base button styles
- `.card-base` - Base card styles
- `.input-base` - Base input styles
- `.badge-base` - Base badge styles
- `.overlay-base` - Base overlay styles

### 4. Style Helpers (TypeScript)

Use the `StylesService` for dynamic style manipulation:

```typescript
import { StylesService } from '@shared/services/styles.service';

constructor(private stylesService: StylesService) {}

// Set CSS variable
this.stylesService.setCSSVariable('--primary-color', '#FF0000');

// Add/remove classes
this.stylesService.addClass(element, 'active');
this.stylesService.toggleClass(element, 'hidden');
```

### 5. Style Helper Utilities

Use helper functions from `@shared/utils/style-helpers`:

```typescript
import { classNames, getSpacingClass, getCSSVar } from '@shared/utils/style-helpers';

// Combine class names
const classes = classNames('btn', 'btn-primary', isActive && 'active');

// Get spacing value
const spacing = getSpacingClass('lg'); // Returns 'var(--spacing-lg)'

// Get CSS variable
const primaryColor = getCSSVar('--primary-color');
```

## Best Practices

1. **Always use CSS variables** instead of hardcoded values
2. **Use utility classes** for common patterns (flex, spacing, text)
3. **Extend base styles** in component CSS when possible
4. **Keep component-specific styles minimal** - prefer shared utilities
5. **Use the StylesService** for dynamic style changes
6. **Follow the naming convention**: `--property-name` for CSS variables

## Available CSS Variables

### Colors
- `--primary-color`
- `--secondary-color`
- `--accent-color`
- `--background-color`
- `--surface-color`
- `--text-color`
- `--text-secondary-color`
- `--error-color`
- `--success-color`
- `--warning-color`
- `--info-color`

### Spacing
- `--spacing-xs` (4px)
- `--spacing-sm` (8px)
- `--spacing-md` (16px)
- `--spacing-lg` (24px)
- `--spacing-xl` (32px)
- `--spacing-xxl` (48px)

### Typography
- `--font-family`
- `--font-size-xs` through `--font-size-xxxl`
- `--font-weight-normal` through `--font-weight-bold`

### Layout
- `--border-radius`
- `--spacing`
- `--z-index-base`, `--z-index-dropdown`, `--z-index-modal`, etc.

## Responsive Design

Use responsive utilities from `_responsive.css`:

```html
<div class="hide-mobile show-desktop">Desktop only</div>
<div class="flex-mobile-column">Stack on mobile</div>
```

## Animation

Use animation classes from `_animations.css`:

```html
<div class="animate-fade-in">Fades in</div>
<div class="animate-slide-in">Slides in</div>
<div class="animate-scale-in">Scales in</div>
```

