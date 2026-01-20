import { PagedResult } from './api-response.model';
import { FilterRequest } from './user.model';

export interface Warehouse {
  id: string;
  name: string;
  code?: string;
  description?: string;
  address?: string;
  city?: string;
  state?: string;
  country?: string;
  postalCode?: string;
  phoneNumber?: string;
  email?: string;
  isDefault: boolean;
  isActive: boolean;
  zoneCount: number;
  createdAt: string;
  updatedAt?: string;
}

export interface CreateWarehouseRequest {
  name: string;
  code?: string;
  description?: string;
  address?: string;
  city?: string;
  state?: string;
  country?: string;
  postalCode?: string;
  phoneNumber?: string;
  email?: string;
  isDefault?: boolean;
}

export interface UpdateWarehouseRequest extends CreateWarehouseRequest {}

export interface GetWarehousesListQuery {
  filterRequest?: FilterRequest;
  isActive?: boolean;
}

export interface PagedWarehousesResult extends PagedResult<Warehouse> {}

