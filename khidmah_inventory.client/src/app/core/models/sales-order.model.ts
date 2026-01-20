import { PagedResult, FilterRequest } from './api-response.model';

export interface SalesOrder {
  id: string;
  orderNumber: string;
  customerId: string;
  customerName: string;
  orderDate: string;
  expectedDeliveryDate?: string;
  status: string;
  subTotal: number;
  taxAmount: number;
  discountAmount: number;
  totalAmount: number;
  notes?: string;
  termsAndConditions?: string;
  items: SalesOrderItem[];
  createdAt: string;
  updatedAt?: string;
}

export interface SalesOrderItem {
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
  deliveredQuantity?: number;
  notes?: string;
}

export interface CreateSalesOrderRequest {
  customerId: string;
  orderDate: string;
  expectedDeliveryDate?: string;
  notes?: string;
  termsAndConditions?: string;
  items: CreateSalesOrderItemRequest[];
}

export interface CreateSalesOrderItemRequest {
  productId: string;
  quantity: number;
  unitPrice: number;
  discountPercent?: number;
  taxPercent?: number;
  notes?: string;
}

export interface GetSalesOrdersListQuery {
  filterRequest?: FilterRequest;
  customerId?: string;
  status?: string;
  fromDate?: string;
  toDate?: string;
}

export interface PagedSalesOrdersResult extends PagedResult<SalesOrder> {}

