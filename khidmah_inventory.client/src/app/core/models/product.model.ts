import { PagedResult } from './api-response.model';
import { FilterRequest } from './user.model';

export interface Product {
  id: string;
  name: string;
  description?: string;
  sku: string;
  barcode?: string;
  categoryId?: string;
  categoryName?: string;
  brandId?: string;
  brandName?: string;
  unitOfMeasureId: string;
  unitOfMeasureName: string;
  unitOfMeasureCode: string;
  purchasePrice: number;
  salePrice: number;
  costPrice?: number;
  minStockLevel?: number;
  maxStockLevel?: number;
  reorderPoint?: number;
  trackQuantity: boolean;
  trackBatch: boolean;
  trackExpiry: boolean;
  isActive: boolean;
  imageUrl?: string;
  weight: number;
  weightUnit?: string;
  length?: number;
  width?: number;
  height?: number;
  dimensionsUnit?: string;
  createdAt: string;
  updatedAt?: string;
}

export interface CreateProductRequest {
  name: string;
  description?: string;
  sku: string;
  barcode?: string;
  categoryId?: string;
  brandId?: string;
  unitOfMeasureId: string;
  purchasePrice: number;
  salePrice: number;
  costPrice?: number;
  minStockLevel?: number;
  maxStockLevel?: number;
  reorderPoint?: number;
  trackQuantity?: boolean;
  trackBatch?: boolean;
  trackExpiry?: boolean;
  imageUrl?: string;
  weight?: number;
  weightUnit?: string;
  length?: number;
  width?: number;
  height?: number;
  dimensionsUnit?: string;
}

export interface UpdateProductRequest extends CreateProductRequest {}

export interface GetProductsListQuery {
  filterRequest?: FilterRequest;
  categoryId?: string;
  brandId?: string;
  isActive?: boolean;
}

export interface PagedProductsResult extends PagedResult<Product> {}

