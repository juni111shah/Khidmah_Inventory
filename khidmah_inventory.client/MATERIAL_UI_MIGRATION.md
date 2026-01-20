# Material UI Migration Guide

## Overview
This document outlines the migration from Bootstrap to Angular Material UI across all components.

## Common Material UI Imports

For standalone components, add these imports:

```typescript
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatRadioModule } from '@angular/material/radio';
import { MatIconModule } from '@angular/material/icon';
import { MatTabsModule } from '@angular/material/tabs';
import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatSortModule } from '@angular/material/sort';
import { MatDialogModule } from '@angular/material/dialog';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatChipsModule } from '@angular/material/chips';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatMenuModule } from '@angular/material/menu';
```

## Bootstrap to Material UI Conversions

### Buttons
```html
<!-- Bootstrap -->
<button class="btn btn-primary">Click</button>
<button class="btn btn-outlined">Click</button>
<button class="btn btn-raised">Click</button>

<!-- Material UI -->
<button mat-raised-button color="primary">Click</button>
<button mat-stroked-button>Click</button>
<button mat-flat-button>Click</button>
```

### Cards
```html
<!-- Bootstrap -->
<div class="card">
  <div class="card-header">Header</div>
  <div class="card-body">Body</div>
  <div class="card-footer">Footer</div>
</div>

<!-- Material UI -->
<mat-card>
  <mat-card-header>
    <mat-card-title>Header</mat-card-title>
  </mat-card-header>
  <mat-card-content>Body</mat-card-content>
  <mat-card-footer>Footer</mat-card-footer>
</mat-card>
```

### Form Fields
```html
<!-- Bootstrap -->
<div class="form-group">
  <label>Name</label>
  <input type="text" class="form-control" [(ngModel)]="name">
</div>

<!-- Material UI -->
<mat-form-field>
  <mat-label>Name</mat-label>
  <input matInput type="text" [(ngModel)]="name">
</mat-form-field>
```

### Select/Dropdown
```html
<!-- Bootstrap -->
<select class="form-select" [(ngModel)]="value">
  <option value="1">Option 1</option>
</select>

<!-- Material UI -->
<mat-form-field>
  <mat-label>Select</mat-label>
  <mat-select [(ngModel)]="value">
    <mat-option value="1">Option 1</mat-option>
  </mat-select>
</mat-form-field>
```

### Checkboxes
```html
<!-- Bootstrap -->
<input type="checkbox" [(ngModel)]="checked">

<!-- Material UI -->
<mat-checkbox [(ngModel)]="checked">Label</mat-checkbox>
```

### Tabs
```html
<!-- Bootstrap -->
<ul class="nav nav-tabs">
  <li class="nav-item"><a class="nav-link active">Tab 1</a></li>
</ul>

<!-- Material UI -->
<mat-tab-group>
  <mat-tab label="Tab 1">Content 1</mat-tab>
  <mat-tab label="Tab 2">Content 2</mat-tab>
</mat-tab-group>
```

### Tables
```html
<!-- Bootstrap -->
<table class="table">
  <thead>...</thead>
  <tbody>...</tbody>
</table>

<!-- Material UI -->
<table mat-table [dataSource]="dataSource">
  <!-- columns -->
  <tr mat-header-row *matHeaderRowDef="displayedColumns"></tr>
  <tr mat-row *matRowDef="let row; columns: displayedColumns;"></tr>
</table>
```

### Layout (Container/Row/Col)
```html
<!-- Bootstrap -->
<div class="container">
  <div class="row">
    <div class="col-md-6">Content</div>
  </div>
</div>

<!-- Material UI - Use Flexbox or Grid -->
<div style="display: flex; gap: 16px;">
  <div style="flex: 1;">Content</div>
</div>
```

## Migration Checklist

For each component:
- [ ] Add Material UI module imports to component TypeScript file
- [ ] Replace Bootstrap button classes with Material buttons
- [ ] Replace Bootstrap cards with mat-card
- [ ] Replace form-control with mat-form-field and matInput
- [ ] Replace Bootstrap selects with mat-select
- [ ] Replace Bootstrap checkboxes with mat-checkbox
- [ ] Replace Bootstrap tabs with mat-tab-group
- [ ] Replace Bootstrap tables with mat-table (if applicable)
- [ ] Remove Bootstrap container/row/col classes, use flexbox/grid
- [ ] Update component SCSS to remove Bootstrap-specific styles
- [ ] Test component functionality

## Completed Components

- ✅ Settings Component

## Remaining Components

All other feature and shared components need to be updated following the same pattern.

