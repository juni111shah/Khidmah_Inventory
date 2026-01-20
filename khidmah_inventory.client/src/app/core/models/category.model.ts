import { PagedResult } from './api-response.model';
import { FilterRequest } from './user.model';

export interface Category {
  id: string;
  name: string;
  description?: string;
  code?: string;
  parentCategoryId?: string;
  parentCategoryName?: string;
  displayOrder: number;
  imageUrl?: string;
  productCount: number;
  subCategoryCount: number;
  createdAt: string;
  updatedAt?: string;
}

export interface CategoryTree extends Category {
  subCategories: CategoryTree[];
}

export interface CreateCategoryRequest {
  name: string;
  description?: string;
  code?: string;
  parentCategoryId?: string;
  displayOrder?: number;
}

export interface UpdateCategoryRequest {
  name: string;
  description?: string;
  code?: string;
  parentCategoryId?: string;
  displayOrder?: number;
}

export interface GetCategoriesListQuery {
  filterRequest?: FilterRequest;
  parentCategoryId?: string;
}

export interface PagedCategoriesResult extends PagedResult<Category> {}

