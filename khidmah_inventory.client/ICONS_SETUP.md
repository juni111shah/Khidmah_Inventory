# Icons Setup - Quick Reference

Both Font Awesome and Material Icons are now configured and ready to use throughout your application!

## ✅ What's Been Set Up

1. **Material Icons Font** - Added to `index.html`
2. **Font Awesome CSS** - Imported in `styles.css`
3. **Reusable Icon Component** - `app-icon` component for easy icon usage
4. **Icon Service** - Helper service with common icon names
5. **Updated Unified Button** - Now properly supports both icon libraries

## 🚀 Quick Start

### Using the Icon Component

```html
<!-- Material Icon (default) -->
<app-icon name="home"></app-icon>

<!-- Font Awesome Icon -->
<app-icon name="home" library="fa"></app-icon>

<!-- With size and color -->
<app-icon name="save" size="lg" color="#4CAF50"></app-icon>
```

### Using Icons in Buttons

```html
<!-- Material icon -->
<app-unified-button icon="save" iconLibrary="material">
  Save
</app-unified-button>

<!-- Font Awesome icon -->
<app-unified-button icon="save" iconLibrary="fa">
  Save
</app-unified-button>
```

### Using the Icon Service

```typescript
import { IconService } from './shared/services/icon.service';

constructor(private iconService: IconService) {}

// Get icon name
const iconName = this.iconService.getIcon('home', 'material');
```

## 📚 Documentation

For detailed usage instructions, see:
- `src/app/shared/components/icon/ICON_USAGE.md` - Complete icon usage guide

## 🎨 Available Icon Libraries

1. **Material Icons** (`library="material"`) - Default
   - Simple, clean icons
   - Great for Material Design apps
   - Browse: https://fonts.google.com/icons

2. **Material Outlined** (`library="material-outlined"`)
   - Outlined style Material icons
   - Modern look

3. **Font Awesome** (`library="fa"`)
   - Extensive icon library
   - Supports animations (spin, pulse)
   - Supports rotation and flipping
   - Browse: https://fontawesome.com/icons

## 💡 Common Examples

```html
<!-- Loading spinner -->
<app-icon name="spinner" library="fa" [spin]="true"></app-icon>

<!-- Success checkmark -->
<app-icon name="check_circle" color="#4CAF50" size="lg"></app-icon>

<!-- Edit button -->
<app-icon name="edit" size="sm" (click)="editItem()"></app-icon>

<!-- Delete with warning color -->
<app-icon name="delete" color="#F44336" size="sm"></app-icon>
```

## 🔧 Import the Icon Component

The icon component is standalone, so you can import it anywhere:

```typescript
import { IconComponent } from './shared/components/icon/icon.component';

@Component({
  imports: [IconComponent, ...]
})
```

That's it! You're ready to use icons throughout your application! 🎉

