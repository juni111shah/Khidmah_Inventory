export interface SalesReport {
  fromDate: string;
  toDate: string;
  totalSales: number;
  totalCost: number;
  totalProfit: number;
  profitMargin: number;
  totalOrders: number;
  items: SalesReportItem[];
}

export interface SalesReportItem {
  date: string;
  orderNumber: string;
  customerName: string;
  amount: number;
  cost: number;
  profit: number;
}

export interface InventoryReport {
  totalStockValue: number;
  totalProducts: number;
  lowStockItems: number;
  outOfStockItems: number;
  items: InventoryReportItem[];
}

export interface InventoryReportItem {
  productId: string;
  productName: string;
  productSKU: string;
  categoryName: string;
  warehouseName: string;
  quantity: number;
  averageCost: number;
  stockValue: number;
  minStockLevel?: number;
  maxStockLevel?: number;
  status: string;
}

export interface PurchaseReport {
  fromDate: string;
  toDate: string;
  totalPurchases: number;
  totalOrders: number;
  items: PurchaseReportItem[];
}

export interface PurchaseReportItem {
  date: string;
  orderNumber: string;
  supplierName: string;
  amount: number;
  status: string;
}

