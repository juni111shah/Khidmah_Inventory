# Reusable Data Table Component

A comprehensive, reusable data table component for listing and CRUD operations with filtering, pagination, column show/hide, and sorting capabilities.

## Features

✅ **Server-side Pagination** - Works with backend pagination  
✅ **Filtering** - Column-based filtering with multiple filter types  
✅ **Searching** - Global search across multiple fields  
✅ **Sorting** - Sortable columns with visual indicators  
✅ **Column Show/Hide** - Toggle column visibility  
✅ **CRUD Actions** - Built-in action buttons (view, edit, delete, etc.)  
✅ **Row Selection** - Optional checkbox selection  
✅ **Loading States** - Built-in loading indicators  
✅ **Empty States** - Customizable empty state messages  
✅ **Responsive** - Mobile-friendly design  
✅ **Type-safe** - Generic TypeScript support  

## Usage

### Basic Example

```typescript
import { Component } from '@angular/core';
import { DataTableComponent } from '../../shared/components/data-table/data-table.component';
import { DataTableColumn, DataTableAction, DataTableConfig } from '../../shared/models/data-table.model';

@Component({
  selector: 'app-users-list',
  standalone: true,
  imports: [DataTableComponent],
  template: `
    <app-data-table
      [columns]="columns"
      [data]="users"
      [loading]="loading"
      [config]="config"
      [actions]="actions"
      [pagedResult]="pagedResult"
      [filterRequest]="filterRequest"
      (filterChange)="onFilterChange($event)"
      (pageChange)="onPageChange($event)"
      (sortChange)="onSortChange($event)"
      (rowClick)="onRowClick($event)"
    ></app-data-table>
  `
})
export class UsersListComponent {
  users: User[] = [];
  loading = false;
  pagedResult: any = null;
  
  filterRequest: FilterRequest = {
    pagination: {
      pageNo: 1,
      pageSize: 10,
      sortBy: 'FirstName',
      sortOrder: 'ascending'
    },
    search: {
      term: '',
      searchFields: ['FirstName', 'LastName', 'Email'],
      mode: SearchMode.Contains,
      isCaseSensitive: false
    }
  };

  columns: DataTableColumn<User>[] = [
    {
      key: 'firstName',
      label: 'First Name',
      sortable: true,
      searchable: true,
      filterable: true,
      width: '150px'
    },
    {
      key: 'email',
      label: 'Email',
      sortable: true,
      searchable: true,
      filterable: true
    },
    {
      key: 'isActive',
      label: 'Status',
      sortable: true,
      filterable: true,
      filterType: 'select',
      filterOptions: [
        { label: 'All', value: '' },
        { label: 'Active', value: true },
        { label: 'Inactive', value: false }
      ],
      type: 'boolean'
    }
  ];

  actions: DataTableAction<User>[] = [
    {
      label: 'View',
      icon: 'eye',
      action: (row) => this.viewUser(row),
      tooltip: 'View user details'
    },
    {
      label: 'Edit',
      icon: 'edit',
      action: (row) => this.editUser(row),
      tooltip: 'Edit user'
    },
    {
      label: 'Delete',
      icon: 'trash',
      action: (row) => this.deleteUser(row),
      condition: (row) => row.isActive,
      class: 'danger',
      tooltip: 'Delete user'
    }
  ];

  config: DataTableConfig = {
    showSearch: true,
    showFilters: true,
    showColumnToggle: true,
    showPagination: true,
    showActions: true,
    pageSize: 10,
    pageSizeOptions: [5, 10, 25, 50, 100],
    searchPlaceholder: 'Search users...',
    emptyMessage: 'No users found'
  };

  onFilterChange(filterRequest: FilterRequest): void {
    this.filterRequest = filterRequest;
    this.loadUsers();
  }

  onPageChange(pageNo: number): void {
    this.loadUsers();
  }

  onSortChange(sort: DataTableSort): void {
    this.loadUsers();
  }

  onRowClick(user: User): void {
    this.viewUser(user);
  }
}
```

## Column Configuration

### DataTableColumn Interface

```typescript
interface DataTableColumn<T = any> {
  key: string;                    // Property key in data object
  label: string;                  // Column header label
  sortable?: boolean;             // Enable sorting (default: false)
  filterable?: boolean;           // Enable filtering (default: false)
  searchable?: boolean;           // Include in global search (default: true)
  width?: string;                 // Column width (e.g., '150px')
  align?: 'left' | 'center' | 'right';  // Text alignment
  visible?: boolean;              // Column visibility (default: true)
  render?: (row: T, column: DataTableColumn<T>) => string;  // Custom render function
  type?: 'text' | 'number' | 'date' | 'boolean' | 'badge' | 'custom';
  format?: (value: any) => string;  // Value formatter
  filterType?: 'text' | 'select' | 'date' | 'number' | 'boolean';
  filterOptions?: { label: string; value: any }[];  // For select filter
}
```

### Column Types

1. **text** (default) - Plain text
2. **number** - Numeric values
3. **date** - Date values (auto-formatted)
4. **boolean** - Boolean with check/cross icons
5. **badge** - Badge styling
6. **custom** - Use render function

### Column Examples

```typescript
// Text column
{
  key: 'name',
  label: 'Name',
  sortable: true,
  searchable: true
}

// Date column
{
  key: 'createdAt',
  label: 'Created',
  type: 'date',
  sortable: true,
  format: (value) => new Date(value).toLocaleDateString()
}

// Boolean column
{
  key: 'isActive',
  label: 'Status',
  type: 'boolean',
  sortable: true,
  filterable: true,
  filterType: 'select',
  filterOptions: [
    { label: 'All', value: '' },
    { label: 'Active', value: true },
    { label: 'Inactive', value: false }
  ]
}

// Badge column
{
  key: 'status',
  label: 'Status',
  type: 'badge',
  render: (row) => row.status
}

// Custom render
{
  key: 'roles',
  label: 'Roles',
  render: (row) => row.roles.join(', ')
}
```

## Actions Configuration

### DataTableAction Interface

```typescript
interface DataTableAction<T = any> {
  label: string;                  // Button label
  icon?: string;                  // Icon name (optional)
  action: (row: T) => void;       // Action handler
  class?: string;                 // CSS class (e.g., 'danger', 'success')
  condition?: (row: T) => boolean;  // Show condition
  tooltip?: string;               // Tooltip text
}
```

### Action Examples

```typescript
actions: DataTableAction<User>[] = [
  {
    label: 'View',
    icon: 'eye',
    action: (row) => this.viewUser(row),
    tooltip: 'View details'
  },
  {
    label: 'Edit',
    icon: 'edit',
    action: (row) => this.editUser(row),
    tooltip: 'Edit user'
  },
  {
    label: 'Delete',
    icon: 'trash',
    action: (row) => this.deleteUser(row),
    condition: (row) => row.isActive,  // Only show if active
    class: 'danger',
    tooltip: 'Delete user'
  }
];
```

## Configuration Options

### DataTableConfig Interface

```typescript
interface DataTableConfig {
  showSearch?: boolean;           // Show search box (default: true)
  showFilters?: boolean;           // Show filter panel (default: true)
  showColumnToggle?: boolean;     // Show column visibility toggle (default: true)
  showPagination?: boolean;       // Show pagination (default: true)
  showActions?: boolean;          // Show actions column (default: true)
  showCheckbox?: boolean;         // Show row selection checkbox (default: false)
  pageSize?: number;             // Default page size (default: 10)
  pageSizeOptions?: number[];    // Page size options (default: [5, 10, 25, 50, 100])
  searchPlaceholder?: string;    // Search input placeholder
  emptyMessage?: string;         // Empty state message
  loadingMessage?: string;       // Loading state message
}
```

## Events

| Event | Type | Description |
|-------|------|-------------|
| `filterChange` | `FilterRequest` | Emitted when filters/search change |
| `pageChange` | `number` | Emitted when page changes |
| `sortChange` | `DataTableSort` | Emitted when sorting changes |
| `rowClick` | `T` | Emitted when row is clicked |
| `rowSelect` | `T[]` | Emitted when rows are selected (if checkbox enabled) |

## Integration with Backend

The component works seamlessly with the backend `FilterRequest` pattern:

```typescript
filterRequest: FilterRequest = {
  pagination: {
    pageNo: 1,
    pageSize: 10,
    sortBy: 'FirstName',
    sortOrder: 'ascending'
  },
  search: {
    term: '',
    searchFields: ['FirstName', 'LastName', 'Email'],
    mode: SearchMode.Contains,
    isCaseSensitive: false
  },
  filters: [
    {
      column: 'isActive',
      operator: '=',
      value: true
    }
  ]
};
```

## Styling

The component uses CSS variables for theming:

```css
--border-color: #e0e0e0;
--bg-color: #f5f5f5;
--hover-bg: #e9e9e9;
--primary-color: #007bff;
--text-color: #333;
--text-muted: #666;
```

## Examples

### Products List

```typescript
columns: DataTableColumn<Product>[] = [
  { key: 'name', label: 'Product Name', sortable: true, searchable: true },
  { key: 'sku', label: 'SKU', sortable: true, searchable: true },
  { key: 'price', label: 'Price', type: 'number', sortable: true },
  { key: 'stock', label: 'Stock', type: 'number', sortable: true },
  { key: 'category', label: 'Category', filterable: true },
  { key: 'isActive', label: 'Active', type: 'boolean', filterable: true }
];
```

### Orders List

```typescript
columns: DataTableColumn<Order>[] = [
  { key: 'orderNumber', label: 'Order #', sortable: true },
  { key: 'customerName', label: 'Customer', sortable: true, searchable: true },
  { key: 'total', label: 'Total', type: 'number', sortable: true },
  { key: 'status', label: 'Status', type: 'badge', filterable: true },
  { key: 'orderDate', label: 'Date', type: 'date', sortable: true }
];
```

## Best Practices

1. **Define columns clearly** - Set appropriate types, widths, and alignment
2. **Use searchable fields** - Only include relevant fields in search
3. **Filter types** - Use select filters for boolean/enum fields
4. **Action conditions** - Use conditions to show/hide actions based on row state
5. **Loading states** - Always handle loading state properly
6. **Error handling** - Handle API errors gracefully
7. **Pagination** - Use server-side pagination for large datasets

## Files

- **Component**: `khidmah_inventory.client/src/app/shared/components/data-table/data-table.component.ts`
- **Template**: `khidmah_inventory.client/src/app/shared/components/data-table/data-table.component.html`
- **Styles**: `khidmah_inventory.client/src/app/shared/components/data-table/data-table.component.css`
- **Models**: `khidmah_inventory.client/src/app/shared/models/data-table.model.ts`
- **Example**: `khidmah_inventory.client/src/app/features/users/users-list/users-list-v2.component.ts`

## Migration from Old Components

To migrate from the old `UsersListComponent`:

1. Replace the table HTML with `<app-data-table>`
2. Define columns array
3. Define actions array
4. Define config object
5. Handle events (filterChange, pageChange, etc.)

The new component handles all the complexity internally!

