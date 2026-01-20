# Skeleton Loader Usage Examples

## Quick Start

All skeleton components are standalone and can be imported directly:

```typescript
import { SkeletonCardComponent } from '@shared/components/skeleton-card/skeleton-card.component';
import { SkeletonTableComponent } from '@shared/components/skeleton-table/skeleton-table.component';
// etc.
```

---

## Common Patterns

### 1. Product List Page

```html
<!-- products.component.html -->
<div class="products-container">
  <!-- Loading State -->
  <div *ngIf="loading" class="products-grid">
    <app-skeleton-card
      *ngFor="let i of [1,2,3,4,5,6]"
      [showHeader]="true"
      [headerTitleWidth]="'150px'"
      [contentLines]="[
        { width: '100%', height: '120px' },
        { width: '80%', height: '16px' },
        { width: '60%', height: '16px' }
      ]"
      [showFooter]="true">
    </app-skeleton-card>
  </div>

  <!-- Actual Content -->
  <div *ngIf="!loading" class="products-grid">
    <div *ngFor="let product of products" class="product-card">
      <!-- Product content -->
    </div>
  </div>
</div>
```

### 2. Form Page

```html
<!-- product-form.component.html -->
<div class="form-container">
  <!-- Loading State -->
  <app-skeleton-form
    *ngIf="loading"
    [fieldCount]="6"
    [showLabels]="true"
    [showActions]="true"
    [defaultFieldHeight]="'40px'">
  </app-skeleton-form>

  <!-- Actual Form -->
  <form *ngIf="!loading" [formGroup]="productForm">
    <!-- Form fields -->
  </form>
</div>
```

### 3. Dashboard Cards

```html
<!-- dashboard.component.html -->
<div class="dashboard-grid">
  <div *ngFor="let card of cards" class="dashboard-card">
    <!-- Loading State -->
    <app-skeleton-card
      *ngIf="card.loading"
      [showHeader]="true"
      [headerTitleWidth]="'120px'"
      [contentLines]="[
        { width: '100%', height: '40px' },
        { width: '80%', height: '20px' }
      ]"
      [showFooter]="false">
    </app-skeleton-card>

    <!-- Actual Content -->
    <div *ngIf="!card.loading">
      <h3>{{ card.title }}</h3>
      <p>{{ card.value }}</p>
    </div>
  </div>
</div>
```

### 4. User Profile Page

```html
<!-- profile.component.html -->
<div class="profile-container">
  <!-- Loading State -->
  <div *ngIf="loading" class="profile-skeleton">
    <app-skeleton-loader
      [width]="'120px'"
      [height]="'120px'"
      [shape]="'circle'"
      [animation]="'shimmer'"
      class="profile-avatar">
    </app-skeleton-loader>
    
    <div class="profile-info">
      <app-skeleton-loader
        [width]="'200px'"
        [height]="'24px'"
        [style.margin-bottom]="'12px'">
      </app-skeleton-loader>
      
      <app-skeleton-form
        [fieldCount]="4"
        [showLabels]="true"
        [showActions]="false">
      </app-skeleton-form>
    </div>
  </div>

  <!-- Actual Content -->
  <div *ngIf="!loading" class="profile-content">
    <!-- Profile content -->
  </div>
</div>
```

### 5. Settings Page

```html
<!-- settings.component.html -->
<div class="settings-container">
  <!-- Loading State -->
  <div *ngIf="loading">
    <app-skeleton-card
      *ngFor="let i of [1,2,3]"
      [showHeader]="true"
      [headerTitleWidth]="'150px'"
      [contentLines]="[
        { width: '100%', height: '16px' },
        { width: '90%', height: '16px' },
        { width: '80%', height: '16px' }
      ]"
      [showFooter]="true">
    </app-skeleton-card>
  </div>

  <!-- Actual Content -->
  <div *ngIf="!loading">
    <!-- Settings content -->
  </div>
</div>
```

### 6. Search Results

```html
<!-- search-results.component.html -->
<div class="search-results">
  <!-- Loading State -->
  <app-skeleton-list
    *ngIf="loading"
    [itemCount]="5"
    [showAvatar]="true"
    [avatarSize]="'48px'"
    [direction]="'horizontal'">
  </app-skeleton-list>

  <!-- Actual Results -->
  <div *ngIf="!loading" class="results-list">
    <div *ngFor="let result of results" class="result-item">
      <!-- Result content -->
    </div>
  </div>
</div>
```

### 7. Button Loading States

```html
<!-- In any component -->
<div class="action-buttons">
  <!-- Loading State -->
  <app-skeleton-button
    *ngIf="saving"
    [width]="'120px'"
    [height]="'40px'">
  </app-skeleton-button>

  <!-- Actual Button -->
  <button
    *ngIf="!saving"
    (click)="save()"
    class="btn btn-primary">
    Save
  </button>
</div>
```

### 8. Inline Field Loading

```html
<!-- In forms or detail pages -->
<div class="field-group">
  <app-skeleton-field
    *ngIf="loading"
    [labelWidth]="'100px'"
    [fieldHeight]="'40px'">
  </app-skeleton-field>

  <div *ngIf="!loading" class="form-field">
    <label>Field Name</label>
    <input type="text" />
  </div>
</div>
```

---

## Component-Specific Examples

### Data Table (Already Integrated)

The data table component automatically shows skeleton loaders when `loading` is true:

```html
<app-data-table
  [columns]="columns"
  [data]="products"
  [loading]="loading"
  [pagedResult]="pagedResult">
</app-data-table>
```

### Custom Table Skeleton

If you need a custom table skeleton:

```html
<app-skeleton-table
  [showHeader]="true"
  [headers]="[
    { width: '30%' },
    { width: '25%' },
    { width: '25%' },
    { width: '20%' }
  ]"
  [rowCount]="10">
</app-skeleton-table>
```

### Card Grid

```html
<div class="cards-grid">
  <app-skeleton-card
    *ngFor="let i of [1,2,3,4,5,6]"
    [showHeader]="true"
    [headerTitleWidth]="'180px'"
    [contentLines]="[
      { width: '100%', height: '200px' },
      { width: '90%', height: '16px' },
      { width: '70%', height: '16px' }
    ]"
    [showFooter]="true">
  </app-skeleton-card>
</div>
```

---

## Advanced Usage

### Custom Skeleton with ng-content

```html
<app-skeleton-card [hasContent]="true">
  <app-skeleton-loader
    [width]="'100%'"
    [height]="'150px'"
    [style.margin-bottom]="'16px'">
  </app-skeleton-loader>
  
  <app-skeleton-loader
    [width]="'80%'"
    [height]="'20px'">
  </app-skeleton-loader>
</app-skeleton-card>
```

### Multiple Animations

```html
<!-- Pulse animation -->
<app-skeleton-loader
  [width]="'100px'"
  [height]="'20px'"
  [animation]="'pulse'">
</app-skeleton-loader>

<!-- Wave animation -->
<app-skeleton-loader
  [width]="'100px'"
  [height]="'20px'"
  [animation]="'wave'">
</app-skeleton-loader>

<!-- Shimmer animation (default) -->
<app-skeleton-loader
  [width]="'100px'"
  [height]="'20px'"
  [animation]="'shimmer'">
</app-skeleton-loader>
```

---

## Tips

1. **Match Dimensions**: Always match skeleton dimensions to actual content dimensions
2. **Consistent Spacing**: Use the same spacing in skeletons as in actual content
3. **Show Immediately**: Display skeletons immediately when loading starts
4. **Hide Smoothly**: Hide skeletons when content loads (use *ngIf, not opacity)
5. **Accessibility**: Skeletons provide visual feedback but don't replace proper loading indicators for screen readers

---

## Performance

Skeleton loaders are lightweight and performant:
- Pure CSS animations (no JavaScript)
- Minimal DOM elements
- No external dependencies
- Works with Angular's change detection

