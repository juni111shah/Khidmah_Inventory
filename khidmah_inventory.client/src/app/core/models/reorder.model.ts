export interface SupplierSuggestion {
  supplierId: string;
  supplierName: string;
  lastPurchasePrice?: number;
  averageDeliveryDays?: number;
  purchaseCount: number;
  lastPurchaseDate?: string;
  recommendedPrice?: number;
  score: number;
}

export interface ReorderSuggestion {
  productId: string;
  productName: string;
  productSKU: string;
  warehouseId: string;
  warehouseName: string;
  currentStock: number;
  minStockLevel?: number;
  reorderPoint?: number;
  maxStockLevel?: number;
  suggestedQuantity: number;
  averageDailySales?: number;
  daysOfStockRemaining: number;
  priority: 'Critical' | 'High' | 'Medium' | 'Low';
  supplierSuggestions: SupplierSuggestion[];
}

export interface GetReorderSuggestionsQuery {
  warehouseId?: string;
  priority?: string;
  includeInStock?: boolean;
}

export interface ReorderItemDto {
  productId: string;
  warehouseId: string;
  quantity: number;
  unitPrice?: number;
}

export interface GeneratePOFromSuggestionsRequest {
  items: ReorderItemDto[];
  supplierId: string;
  expectedDeliveryDate?: string;
  notes?: string;
}
