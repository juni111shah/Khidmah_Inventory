import { PagedResult, FilterRequest } from './api-response.model';

export interface Supplier {
  id: string;
  name: string;
  code?: string;
  contactPerson?: string;
  email?: string;
  phoneNumber?: string;
  address?: string;
  city?: string;
  state?: string;
  country?: string;
  postalCode?: string;
  taxId?: string;
  paymentTerms?: string;
  creditLimit?: number;
  balance?: number;
  isActive: boolean;
  createdAt: string;
  updatedAt?: string;
}

export interface CreateSupplierRequest {
  name: string;
  code?: string;
  contactPerson?: string;
  email?: string;
  phoneNumber?: string;
  address?: string;
  city?: string;
  state?: string;
  country?: string;
  postalCode?: string;
  taxId?: string;
  paymentTerms?: string;
  creditLimit?: number;
}

export interface GetSuppliersListQuery {
  filterRequest?: FilterRequest;
  isActive?: boolean;
}

export interface PagedSuppliersResult extends PagedResult<Supplier> {}

