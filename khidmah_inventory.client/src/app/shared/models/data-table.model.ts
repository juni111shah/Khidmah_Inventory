export interface DataTableColumn<T = any> {
  key: string;
  label: string;
  sortable?: boolean;
  filterable?: boolean;
  searchable?: boolean;
  width?: string;
  align?: 'left' | 'center' | 'right';
  visible?: boolean;
  render?: (row: T, column: DataTableColumn<T>) => string;
  type?: 'text' | 'number' | 'date' | 'boolean' | 'badge' | 'custom';
  format?: (value: any) => string;
  filterType?: 'text' | 'select' | 'date' | 'number' | 'boolean';
  filterOptions?: { label: string; value: any }[];
}

export interface DataTableAction<T = any> {
  label: string;
  icon?: string;
  action: (row: T) => void;
  class?: string;
  condition?: (row: T) => boolean;
  tooltip?: string;
}

export interface DataTableConfig {
  showSearch?: boolean;
  showFilters?: boolean;
  showColumnToggle?: boolean;
  showPagination?: boolean;
  showActions?: boolean;
  showCheckbox?: boolean;
  pageSize?: number;
  pageSizeOptions?: number[];
  searchPlaceholder?: string;
  emptyMessage?: string;
  loadingMessage?: string;
}

export interface DataTableFilter {
  column: string;
  operator: string;
  value: any;
}

export interface DataTableSort {
  column: string;
  direction: 'asc' | 'desc';
}

