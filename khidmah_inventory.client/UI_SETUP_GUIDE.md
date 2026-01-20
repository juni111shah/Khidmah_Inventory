# UI Framework Setup Guide

This guide explains how Font Awesome, Bootstrap, and Angular Material are integrated into the Khidmah Inventory application with full customization support.

## 📦 Installed Packages

### Font Awesome
- `@fortawesome/fontawesome-free` - Font Awesome CSS library
- `@fortawesome/angular-fontawesome@^0.14.0` - Angular Font Awesome component

### Bootstrap
- `bootstrap` - Bootstrap CSS and JavaScript

### Angular Material
- `@angular/material@^17.0.0` - Angular Material components
- `@angular/cdk@^17.0.0` - Angular CDK (Component Dev Kit)

## 🎨 Configuration

### 1. Angular Configuration (`angular.json`)

Bootstrap and Font Awesome CSS are automatically loaded:
```json
"styles": [
  "node_modules/bootstrap/dist/css/bootstrap.min.css",
  "node_modules/@fortawesome/fontawesome-free/css/all.min.css",
  "src/styles.css"
],
"scripts": [
  "node_modules/bootstrap/dist/js/bootstrap.bundle.min.js"
]
```

### 2. Angular Material Modules

All Material modules are imported in `app.module.ts`:
- MatButtonModule
- MatCardModule
- MatFormFieldModule
- MatInputModule
- MatIconModule
- MatToolbarModule
- MatSidenavModule
- And many more...

### 3. Theme Customization

#### CSS Custom Properties
All three frameworks are customized using CSS variables defined in:
- `_variables.css` - Base theme variables
- `_material-theme.css` - Material Design overrides
- `_bootstrap-overrides.css` - Bootstrap customization
- `_fontawesome-overrides.css` - Font Awesome utilities

#### Theme Service
The `ThemeService` (`shared/services/theme.service.ts`) allows dynamic theme changes:
```typescript
import { ThemeService } from './shared/services/theme.service';

constructor(private themeService: ThemeService) {}

// Update theme
this.themeService.setTheme({
  primaryColor: '#673AB7',
  borderRadius: '12px',
  spacing: '20px'
});
```

## 🧩 Reusable Components

### UnifiedButtonComponent
A button component supporting all three frameworks:
```html
<app-unified-button 
  variant="primary" 
  style="material"
  icon="save"
  iconLibrary="material">
  Save
</app-unified-button>
```

### UnifiedCardComponent
A card component with multiple style options:
```html
<app-unified-card 
  style="material"
  elevation="3"
  header="Card Title">
  Content here
</app-unified-card>
```

### UnifiedInputComponent
A form input with icon support:
```html
<app-unified-input
  label="Email"
  style="material"
  icon="email"
  iconLibrary="material"
  [(ngModel)]="email">
</app-unified-input>
```

## 🎯 Usage Examples

### Material Design Components
```html
<!-- Material Button -->
<app-unified-button variant="primary" style="material" icon="save" iconLibrary="material">
  Save
</app-unified-button>

<!-- Material Card -->
<app-unified-card style="material" elevation="2" header="Title">
  Content
</app-unified-card>

<!-- Material Input -->
<app-unified-input label="Name" style="material" icon="person" iconLibrary="material">
</app-unified-input>
```

### Bootstrap Components
```html
<!-- Bootstrap Button -->
<app-unified-button variant="success" style="bootstrap" icon="fa-check" iconLibrary="fa">
  Submit
</app-unified-button>

<!-- Bootstrap Card -->
<app-unified-card style="bootstrap" variant="primary">
  Content
</app-unified-card>

<!-- Bootstrap Input -->
<app-unified-input label="Email" style="bootstrap" icon="fa-envelope" iconLibrary="fa">
</app-unified-input>
```

### Custom Components
```html
<!-- Custom Button -->
<app-unified-button variant="primary" style="custom" outlined="true">
  Custom Button
</app-unified-button>

<!-- Custom Card -->
<app-unified-card style="custom" variant="success" [hoverable]="true">
  Content
</app-unified-card>
```

## 🔧 Customization

### CSS Variables
All components respect CSS custom properties:
```css
:root {
  --primary-color: #2196F3;
  --secondary-color: #FF9800;
  --border-radius: 8px;
  --spacing: 16px;
  /* ... more variables */
}
```

### Dynamic Theme Changes
```typescript
// Change theme at runtime
this.themeService.setTheme({
  primaryColor: '#673AB7',
  secondaryColor: '#FF5722',
  borderRadius: '12px'
});
```

### Icon Libraries

**Font Awesome:**
```html
<i class="fa fa-home"></i>
<app-unified-button icon="fa-home" iconLibrary="fa">Home</app-unified-button>
```

**Material Icons:**
```html
<mat-icon>home</mat-icon>
<app-unified-button icon="home" iconLibrary="material">Home</app-unified-button>
```

## 📚 Component Documentation

See `src/app/shared/components/README.md` for detailed component documentation.

## 🎨 Style Files Structure

```
src/app/shared/styles/
├── _variables.css          # Base CSS variables
├── _base.css               # Base styles
├── _mixins.css             # CSS mixins
├── _animations.css          # Animations
├── _material-theme.css      # Material Design overrides
├── _bootstrap-overrides.css # Bootstrap customization
├── _fontawesome-overrides.css # Font Awesome utilities
├── _components.css          # Component styles
├── _responsive.css          # Responsive utilities
└── index.css               # Main import file
```

## 🚀 Best Practices

1. **Consistency**: Use one style (`material`, `bootstrap`, or `custom`) per feature
2. **Theme Service**: Use ThemeService for app-wide theme management
3. **Icons**: Prefer Material icons for Material components, Font Awesome for Bootstrap/Custom
4. **Accessibility**: Always provide labels, tooltips, and ARIA attributes
5. **Responsive**: Components adapt to theme spacing variables automatically

## 🔄 Integration with Existing Components

All new unified components are standalone and can be imported directly:
```typescript
import { UnifiedButtonComponent } from './shared/components/unified-button/unified-button.component';
import { UnifiedCardComponent } from './shared/components/unified-card/unified-card.component';
import { UnifiedInputComponent } from './shared/components/unified-input/unified-input.component';
```

## 📝 Notes

- All three frameworks work together seamlessly
- Theme changes apply globally via CSS custom properties
- Components are fully reusable and customizable
- No conflicts between frameworks - they complement each other
- Bootstrap JavaScript is loaded for interactive components (modals, dropdowns, etc.)

## 🎯 Next Steps

1. Use unified components in your features
2. Customize theme using ThemeService
3. Create additional unified components as needed
4. Follow the patterns established in the component library

