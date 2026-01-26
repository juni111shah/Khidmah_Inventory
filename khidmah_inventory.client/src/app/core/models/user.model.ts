export interface User {
  id: string;
  email: string;
  userName: string;
  firstName: string;
  lastName: string;
  phoneNumber?: string;
  isActive: boolean;
  emailConfirmed: boolean;
  lastLoginAt?: string;
  avatarUrl?: string;
  roles: string[];
  permissions: string[];
  companies: Company[];
  defaultCompanyId?: string;
  createdAt: string;
  updatedAt: string;
}

export interface Company {
  id: string;
  name: string;
  isDefault: boolean;
  isActive: boolean;
}

export interface UpdateUserProfileRequest {
  firstName: string;
  lastName: string;
  phoneNumber?: string;
}

export interface ChangePasswordRequest {
  currentPassword: string;
  newPassword: string;
}

export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  pageNo: number;
  pageSize: number;
  totalPages: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}

export interface FilterRequest {
  pagination?: PaginationDto;
  filters?: FilterDto[];
  search?: SearchRequest;
}

export interface PaginationDto {
  pageNo: number;
  pageSize: number;
  sortBy?: string;
  sortOrder?: string;
}

export interface FilterDto {
  column: string;
  operator: string;
  value: any;
}

export interface SearchRequest {
  term: string;
  searchFields: string[];
  mode: SearchMode;
  isCaseSensitive: boolean;
}

export enum SearchMode {
  Contains = 'Contains',
  StartsWith = 'StartsWith',
  EndsWith = 'EndsWith',
  ExactMatch = 'ExactMatch'
}

