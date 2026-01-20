# Frontend Implementation Patterns

## Overview

This document outlines common patterns and best practices for implementing frontend features in the Khidmah Inventory Management System.

## Architecture Patterns

### 1. Service Layer Pattern

All API calls should go through service classes:

```typescript
@Injectable({ providedIn: 'root' })
export class ProductApiService {
  private apiUrl = '/api/products';

  constructor(private http: HttpClient) {}

  getProducts(params?: any): Observable<PagedResult<ProductDto>> {
    return this.http.get<PagedResult<ProductDto>>(this.apiUrl, { params });
  }

  createProduct(product: CreateProductDto): Observable<ProductDto> {
    return this.http.post<ProductDto>(this.apiUrl, product);
  }
}
```

**Benefits**:
- Centralized API endpoint management
- Easy to mock for testing
- Consistent error handling
- Type safety

### 2. Component Structure

```
feature-name/
  feature-name-list/
    feature-name-list.component.ts
    feature-name-list.component.html
    feature-name-list.component.css
  feature-name-form/
    feature-name-form.component.ts
    feature-name-form.component.html
    feature-name-form.component.css
  feature-name-detail/
    feature-name-detail.component.ts
    feature-name-detail.component.html
    feature-name-detail.component.css
```

### 3. Form Handling Pattern

Use Reactive Forms with validation:

```typescript
export class ProductFormComponent {
  productForm: FormGroup;
  
  constructor(private fb: FormBuilder) {
    this.productForm = this.fb.group({
      name: ['', [Validators.required, Validators.maxLength(200)]],
      sku: ['', [Validators.required, Validators.maxLength(50)]],
      price: [0, [Validators.required, Validators.min(0)]]
    });
  }
  
  onSubmit(): void {
    if (this.productForm.valid) {
      // Submit form
    } else {
      // Mark all fields as touched to show validation errors
      Object.keys(this.productForm.controls).forEach(key => {
        this.productForm.get(key)?.markAsTouched();
      });
    }
  }
}
```

### 4. Loading State Pattern

```typescript
export class ProductListComponent {
  products: ProductDto[] = [];
  loading: boolean = false;
  error: string = '';

  loadProducts(): void {
    this.loading = true;
    this.error = '';
    
    this.productService.getProducts().subscribe({
      next: (data) => {
        this.products = data;
        this.loading = false;
      },
      error: (err) => {
        this.error = 'Failed to load products';
        this.loading = false;
      }
    });
  }
}
```

### 5. Error Handling Pattern

```typescript
handleError(error: any): void {
  if (error.status === 401) {
    // Unauthorized - redirect to login
    this.router.navigate(['/login']);
  } else if (error.status === 403) {
    // Forbidden - show unauthorized message
    this.showError('You do not have permission to perform this action');
  } else if (error.status === 404) {
    // Not found
    this.showError('Resource not found');
  } else if (error.status >= 500) {
    // Server error
    this.showError('Server error. Please try again later.');
  } else {
    // Validation errors
    const errors = error.error?.errors || ['An error occurred'];
    this.showError(errors.join(', '));
  }
}
```

### 6. Pagination Pattern

```typescript
export class ProductListComponent {
  pageNumber: number = 1;
  pageSize: number = 10;
  totalCount: number = 0;
  
  loadProducts(): void {
    const params = {
      pageNumber: this.pageNumber,
      pageSize: this.pageSize
    };
    
    this.productService.getProducts(params).subscribe({
      next: (result) => {
        this.products = result.items;
        this.totalCount = result.totalCount;
      }
    });
  }
  
  onPageChange(page: number): void {
    this.pageNumber = page;
    this.loadProducts();
  }
}
```

### 7. Search Pattern

```typescript
export class ProductListComponent {
  searchTerm: string = '';
  private searchSubject = new Subject<string>();
  
  ngOnInit(): void {
    // Debounce search input
    this.searchSubject.pipe(
      debounceTime(300),
      distinctUntilChanged()
    ).subscribe(term => {
      this.searchTerm = term;
      this.loadProducts();
    });
  }
  
  onSearchChange(term: string): void {
    this.searchSubject.next(term);
  }
}
```

### 8. Modal/Dialog Pattern

```typescript
export class ProductListComponent {
  constructor(private modalService: ModalService) {}
  
  openCreateModal(): void {
    const modalRef = this.modalService.open(ProductFormComponent, {
      data: { mode: 'create' }
    });
    
    modalRef.afterClosed().subscribe(result => {
      if (result) {
        this.loadProducts();
      }
    });
  }
  
  openEditModal(product: ProductDto): void {
    const modalRef = this.modalService.open(ProductFormComponent, {
      data: { mode: 'edit', product }
    });
    
    modalRef.afterClosed().subscribe(result => {
      if (result) {
        this.loadProducts();
      }
    });
  }
}
```

### 9. Toast Notification Pattern

```typescript
export class ProductFormComponent {
  constructor(private toastService: ToastService) {}
  
  onSubmit(): void {
    this.productService.createProduct(this.productForm.value).subscribe({
      next: () => {
        this.toastService.showSuccess('Product created successfully');
        this.router.navigate(['/products']);
      },
      error: (err) => {
        this.toastService.showError('Failed to create product');
      }
    });
  }
}
```

### 10. Table/List Pattern

```typescript
export class ProductListComponent {
  displayedColumns: string[] = ['name', 'sku', 'price', 'actions'];
  
  // In template:
  // <table>
  //   <thead>
  //     <tr>
  //       <th *ngFor="let col of displayedColumns">{{ col }}</th>
  //     </tr>
  //   </thead>
  //   <tbody>
  //     <tr *ngFor="let product of products">
  //       <td>{{ product.name }}</td>
  //       <td>{{ product.sku }}</td>
  //       <td>{{ product.price | currency }}</td>
  //       <td>
  //         <button (click)="edit(product)">Edit</button>
  //         <button (click)="delete(product)">Delete</button>
  //       </td>
  //     </tr>
  //   </tbody>
  // </table>
}
```

## Common Components

### 1. Data Table Component

Reusable table component with:
- Sorting
- Pagination
- Filtering
- Actions column

### 2. Form Field Component

Reusable form field with:
- Label
- Input/Select/Textarea
- Validation messages
- Required indicator

### 3. Loading Spinner Component

Show loading state during API calls.

### 4. Empty State Component

Show when no data is available.

### 5. Confirmation Dialog Component

Confirm destructive actions (delete, etc.).

## State Management

### Simple State (No NgRx needed)

For most features, component-level state is sufficient:

```typescript
export class ProductListComponent {
  products: ProductDto[] = [];
  loading: boolean = false;
  error: string = '';
  
  // State management methods
  loadProducts(): void { }
  addProduct(product: ProductDto): void { }
  updateProduct(product: ProductDto): void { }
  removeProduct(id: string): void { }
}
```

### Shared State (Services)

For state shared across components:

```typescript
@Injectable({ providedIn: 'root' })
export class ProductStateService {
  private productsSubject = new BehaviorSubject<ProductDto[]>([]);
  public products$ = this.productsSubject.asObservable();
  
  loadProducts(): void {
    this.productService.getProducts().subscribe(products => {
      this.productsSubject.next(products);
    });
  }
}
```

## Routing Patterns

### Feature Module Routing

```typescript
const routes: Routes = [
  {
    path: '',
    component: ProductListComponent
  },
  {
    path: 'create',
    component: ProductFormComponent
  },
  {
    path: ':id',
    component: ProductDetailComponent
  },
  {
    path: ':id/edit',
    component: ProductFormComponent
  }
];
```

### Route Guards

```typescript
// auth.guard.ts - protect routes
// role.guard.ts - check roles
// permission.guard.ts - check permissions
```

## Testing Patterns

### Unit Tests

```typescript
describe('ProductListComponent', () => {
  let component: ProductListComponent;
  let service: ProductApiService;
  
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        { provide: ProductApiService, useValue: mockProductService }
      ]
    });
    component = TestBed.createComponent(ProductListComponent).componentInstance;
  });
  
  it('should load products on init', () => {
    component.ngOnInit();
    expect(component.products.length).toBeGreaterThan(0);
  });
});
```

## Best Practices

1. **Always use TypeScript types** - Define interfaces for all DTOs
2. **Handle errors gracefully** - Show user-friendly error messages
3. **Show loading states** - Users should know when something is happening
4. **Validate forms** - Client-side validation improves UX
5. **Use async pipe** - For observables in templates
6. **Unsubscribe** - Use takeUntil or async pipe to prevent memory leaks
7. **Lazy load modules** - Improve initial load time
8. **Use OnPush change detection** - For better performance
9. **Accessibility** - Use semantic HTML and ARIA attributes
10. **Responsive design** - Ensure mobile compatibility

## Common Utilities

### Date Formatting

```typescript
formatDate(date: Date | string): string {
  return new Date(date).toLocaleDateString();
}
```

### Currency Formatting

```typescript
formatCurrency(amount: number): string {
  return new Intl.NumberFormat('en-US', {
    style: 'currency',
    currency: 'USD'
  }).format(amount);
}
```

### Debounce Utility

```typescript
debounce(func: Function, wait: number) {
  let timeout: any;
  return function executedFunction(...args: any[]) {
    const later = () => {
      clearTimeout(timeout);
      func(...args);
    };
    clearTimeout(timeout);
    timeout = setTimeout(later, wait);
  };
}
```

## Summary

Follow these patterns consistently across all features to maintain code quality, improve maintainability, and ensure a consistent user experience.

