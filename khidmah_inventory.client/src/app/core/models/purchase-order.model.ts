import { PagedResult, FilterRequest } from './api-response.model';

export interface PurchaseOrder {
  id: string;
  orderNumber: string;
  supplierId: string;
  supplierName: string;
  orderDate: string;
  expectedDeliveryDate?: string;
  status: string;
  subTotal: number;
  taxAmount: number;
  discountAmount: number;
  totalAmount: number;
  notes?: string;
  termsAndConditions?: string;
  items: PurchaseOrderItem[];
  createdAt: string;
  updatedAt?: string;
}

export interface PurchaseOrderItem {
  id: string;
  productId: string;
  productName: string;
  productSKU: string;
  quantity: number;
  unitPrice: number;
  discountPercent: number;
  discountAmount: number;
  taxPercent: number;
  taxAmount: number;
  lineTotal: number;
  receivedQuantity?: number;
  notes?: string;
}

export interface CreatePurchaseOrderRequest {
  supplierId: string;
  orderDate: string;
  expectedDeliveryDate?: string;
  notes?: string;
  termsAndConditions?: string;
  status?: string;
  items: CreatePurchaseOrderItemRequest[];
}

export interface CreatePurchaseOrderItemRequest {
  productId: string;
  quantity: number;
  unitPrice: number;
  discountPercent?: number;
  taxPercent?: number;
  notes?: string;
}

export interface UpdatePurchaseOrderRequest {
  supplierId: string;
  orderDate: string;
  expectedDeliveryDate?: string;
  notes?: string;
  termsAndConditions?: string;
  status?: string;
  items: CreatePurchaseOrderItemRequest[];
}

export interface GetPurchaseOrdersListQuery {
  filterRequest?: FilterRequest;
  supplierId?: string;
  status?: string;
  fromDate?: string;
  toDate?: string;
}

export interface PagedPurchaseOrdersResult extends PagedResult<PurchaseOrder> {}

