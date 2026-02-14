import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { CategoryApiService } from '../../../core/services/category-api.service';
import { Category, CreateCategoryRequest, UpdateCategoryRequest } from '../../../core/models/category.model';
import { ToastComponent } from '../../../shared/components/toast/toast.component';
import { LoadingSpinnerComponent } from '../../../shared/components/loading-spinner/loading-spinner.component';
import { IconComponent } from '../../../shared/components/icon/icon.component';
import { PermissionService } from '../../../core/services/permission.service';
import { HasPermissionDirective } from '../../../shared/directives/has-permission.directive';
import { FormFieldComponent } from '../../../shared/components/form-field/form-field.component';
import { UnifiedButtonComponent } from '../../../shared/components/unified-button/unified-button.component';
import { UnifiedCardComponent } from '../../../shared/components/unified-card/unified-card.component';
import { HeaderService } from '../../../core/services/header.service';
import { ContentLoaderComponent } from '../../../shared/components/content-loader/content-loader.component';
import { SkeletonDetailHeaderComponent } from '../../../shared/components/skeleton-detail-header/skeleton-detail-header.component';
import { SkeletonFormComponent } from '../../../shared/components/skeleton-form/skeleton-form.component';

@Component({
  selector: 'app-category-form',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ToastComponent,
    LoadingSpinnerComponent,
    IconComponent,
    HasPermissionDirective,
    FormFieldComponent,
    UnifiedButtonComponent,
    UnifiedCardComponent,
    ContentLoaderComponent,
    SkeletonDetailHeaderComponent,
    SkeletonFormComponent
  ],
  templateUrl: './category-form.component.html'
})
export class CategoryFormComponent implements OnInit {
  category: Category | null = null;
  parentCategories: Category[] = [];
  loading = false;
  saving = false;
  isEditMode = false;

  formData = {
    name: '',
    description: '',
    code: '',
    parentCategoryId: '',
    displayOrder: 0
  };

  showToast = false;
  toastMessage = '';
  toastType: 'success' | 'error' | 'warning' | 'info' = 'success';

  constructor(
    private categoryApiService: CategoryApiService,
    private route: ActivatedRoute,
    public router: Router,
    public permissionService: PermissionService,
    private headerService: HeaderService
  ) {}

  ngOnInit(): void {
    const categoryId = this.route.snapshot.paramMap.get('id');
    this.isEditMode = !!categoryId;

    this.headerService.setHeaderInfo({
      title: this.isEditMode ? 'Edit Category' : 'Create Category',
      description: this.isEditMode ? 'Modify existing category details' : 'Add a new product category'
    });

    this.loadParentCategories();

    if (this.isEditMode && categoryId) {
      this.loadCategory(categoryId);
    }
  }

  loadCategory(id: string): void {
    this.loading = true;
    this.categoryApiService.getCategory(id).subscribe({
      next: (response) => {
        if (response.success && response.data) {
          this.category = response.data;
          this.formData = {
            name: response.data.name,
            description: response.data.description || '',
            code: response.data.code || '',
            parentCategoryId: response.data.parentCategoryId || '',
            displayOrder: response.data.displayOrder
          };
        } else {
          this.showToastMessage('error', response.message || 'Failed to load category');
        }
        this.loading = false;
      },
      error: () => {
        this.showToastMessage('error', 'Error loading category');
        this.loading = false;
      }
    });
  }

  loadParentCategories(): void {
    this.categoryApiService.getCategories({}).subscribe({
      next: (response) => {
        if (response.success && response.data) {
          this.parentCategories = response.data.items.filter(
            (c: Category) => !this.isEditMode || c.id !== this.category?.id
          );
        }
      }
    });
  }

  save(): void {
    if (!this.formData.name.trim()) {
      this.showToastMessage('error', 'Category name is required');
      return;
    }

    this.saving = true;

    if (this.isEditMode && this.category) {
      const updateRequest: UpdateCategoryRequest = {
        name: this.formData.name,
        description: this.formData.description || undefined,
        code: this.formData.code || undefined,
        parentCategoryId: this.formData.parentCategoryId || undefined,
        displayOrder: this.formData.displayOrder
      };

      this.categoryApiService.updateCategory(this.category.id, updateRequest).subscribe({
        next: (response) => {
          if (response.success) {
            this.showToastMessage('success', 'Category updated successfully');
            setTimeout(() => {
              this.router.navigate(['/categories']);
            }, 1500);
          } else {
            this.showToastMessage('error', response.message || 'Failed to update category');
          }
          this.saving = false;
        },
        error: () => {
          this.showToastMessage('error', 'Error updating category');
          this.saving = false;
        }
      });
    } else {
      const createRequest: CreateCategoryRequest = {
        name: this.formData.name,
        description: this.formData.description || undefined,
        code: this.formData.code || undefined,
        parentCategoryId: this.formData.parentCategoryId || undefined,
        displayOrder: this.formData.displayOrder
      };

      this.categoryApiService.createCategory(createRequest).subscribe({
        next: (response) => {
          if (response.success) {
            this.showToastMessage('success', 'Category created successfully');
            setTimeout(() => {
              this.router.navigate(['/categories']);
            }, 1500);
          } else {
            this.showToastMessage('error', response.message || 'Failed to create category');
          }
          this.saving = false;
        },
        error: () => {
          this.showToastMessage('error', 'Error creating category');
          this.saving = false;
        }
      });
    }
  }

  showToastMessage(type: 'success' | 'error' | 'warning' | 'info', message: string): void {
    this.toastType = type;
    this.toastMessage = message;
    this.showToast = true;
    setTimeout(() => {
      this.showToast = false;
    }, 3000);
  }

  get parentOptions(): { value: any; label: string }[] {
    return [
      { value: '', label: 'None (Root Category)' },
      ...this.parentCategories.map(c => ({ value: c.id, label: c.name }))
    ];
  }
}
