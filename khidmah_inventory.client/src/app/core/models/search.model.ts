export interface GlobalSearchItemDto {
  id: string;
  nameOrNumber: string;
  route: string;
  extraInfo?: string;
}

export interface GlobalSearchResultDto {
  products: GlobalSearchItemDto[];
  customers: GlobalSearchItemDto[];
  suppliers: GlobalSearchItemDto[];
  purchaseOrders: GlobalSearchItemDto[];
  salesOrders: GlobalSearchItemDto[];
}

export interface GlobalSearchQuery {
  searchTerm: string;
  limitPerGroup?: number;
}

/** @deprecated Use GlobalSearchResultDto */
export interface SearchResultItem {
  entityType: string;
  entityId: string;
  title: string;
  description: string;
  url?: string;
  metadata?: Record<string, unknown>;
}

/** @deprecated Use GlobalSearchResultDto */
export interface SearchResult {
  items: SearchResultItem[];
  totalCount: number;
}
