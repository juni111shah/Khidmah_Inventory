export interface ApiResponse<T> {
  success: boolean;
  message: string;
  statusCode: number;
  data?: T;
  errors: string[];
  timestamp: string;
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

export interface PaginationDto {
  pageNo: number;
  pageSize: number;
  sortBy?: string;
  sortOrder?: 'ascending' | 'descending';
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
  Contains = 0,
  StartsWith = 1,
  EndsWith = 2,
  ExactMatch = 3
}

export interface FilterRequest {
  pagination?: PaginationDto;
  filters?: FilterDto[];
  search?: SearchRequest;
}

