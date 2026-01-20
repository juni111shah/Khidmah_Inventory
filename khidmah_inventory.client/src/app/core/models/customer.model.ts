import { PagedResult, FilterRequest } from './api-response.model';

export interface Customer {
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

export interface CreateCustomerRequest {
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

export interface GetCustomersListQuery {
  filterRequest?: FilterRequest;
  isActive?: boolean;
}

export interface PagedCustomersResult extends PagedResult<Customer> {}

