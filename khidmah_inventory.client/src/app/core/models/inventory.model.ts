import { PagedResult, FilterRequest } from './api-response.model';

export interface StockTransaction {
  id: string;
  productId: string;
  productName: string;
  productSKU: string;
  warehouseId: string;
  warehouseName: string;
  transactionType: string;
  quantity: number;
  unitCost?: number;
  totalCost?: number;
  referenceNumber?: string;
  referenceType?: string;
  referenceId?: string;
  batchNumber?: string;
  expiryDate?: string;
  notes?: string;
  transactionDate: string;
  balanceAfter: number;
  createdAt: string;
}

export interface StockLevel {
  id: string;
  productId: string;
  productName: string;
  productSKU: string;
  warehouseId: string;
  warehouseName: string;
  quantity: number;
  reservedQuantity: number;
  availableQuantity: number;
  averageCost?: number;
  lastUpdated: string;
}

export interface CreateStockTransactionRequest {
  productId: string;
  warehouseId: string;
  transactionType: string;
  quantity: number;
  unitCost?: number;
  referenceType?: string;
  referenceId?: string;
  referenceNumber?: string;
  batchNumber?: string;
  expiryDate?: string;
  notes?: string;
  transactionDate?: string;
}

export interface GetStockTransactionsListQuery {
  filterRequest?: FilterRequest;
  productId?: string;
  warehouseId?: string;
  transactionType?: string;
  fromDate?: string;
  toDate?: string;
}

export interface GetStockLevelsListQuery {
  filterRequest?: FilterRequest;
  productId?: string;
  warehouseId?: string;
  lowStockOnly?: boolean;
}

export interface PagedStockTransactionsResult extends PagedResult<StockTransaction> {}
export interface PagedStockLevelsResult extends PagedResult<StockLevel> {}

