export enum TimeRangeType {
  Today = 0,
  Yesterday = 1,
  Last7Days = 2,
  Last30Days = 3,
  ThisMonth = 4,
  LastMonth = 5,
  ThisQuarter = 6,
  LastQuarter = 7,
  ThisYear = 8,
  LastYear = 9,
  Custom = 10
}

export enum AnalyticsType {
  Sales = 0,
  Purchase = 1,
  Inventory = 2,
  Profit = 3,
  Products = 4,
  Customers = 5,
  Suppliers = 6
}

export interface AnalyticsRequest {
  timeRange: TimeRangeType;
  customFromDate?: string;
  customToDate?: string;
  analyticsType: AnalyticsType;
  groupBy?: string;
  metrics?: string[];
}

export interface SalesAnalytics {
  fromDate: string;
  toDate: string;
  totalSales: number;
  totalCost: number;
  totalProfit: number;
  profitMargin: number;
  totalOrders: number;
  averageOrderValue: number;
  timeSeriesData: TimeSeriesData[];
  categoryBreakdown: CategoryAnalytics[];
  topProducts: ProductAnalytics[];
  topCustomers: CustomerAnalytics[];
}

export interface PurchaseAnalytics {
  fromDate: string;
  toDate: string;
  totalPurchases: number;
  totalOrders: number;
  averageOrderValue: number;
  timeSeriesData: TimeSeriesData[];
  topSuppliers: SupplierAnalytics[];
  topProducts: ProductAnalytics[];
}

export interface InventoryAnalytics {
  totalStockValue: number;
  totalProducts: number;
  lowStockItems: number;
  outOfStockItems: number;
  averageStockValue: number;
  categoryStockValues: CategoryStockValue[];
  warehouseStockValues: WarehouseStockValue[];
  fastMovingProducts: FastMovingProduct[];
  slowMovingProducts: SlowMovingProduct[];
}

export interface ProfitAnalytics {
  fromDate: string;
  toDate: string;
  totalRevenue: number;
  totalCost: number;
  totalProfit: number;
  profitMargin: number;
  grossProfitMargin: number;
  profitTrend: TimeSeriesData[];
  categoryProfits: CategoryProfit[];
}

export interface TimeSeriesData {
  label: string;
  date: string;
  value: number;
  secondaryValue?: number;
}

export interface CategoryAnalytics {
  categoryName: string;
  totalSales: number;
  totalCost: number;
  totalProfit: number;
  orderCount: number;
  percentage: number;
}

export interface ProductAnalytics {
  productId: string;
  productName: string;
  productSKU: string;
  totalSales: number;
  totalCost: number;
  totalProfit: number;
  quantitySold: number;
  averagePrice: number;
}

export interface CustomerAnalytics {
  customerId: string;
  customerName: string;
  totalSales: number;
  orderCount: number;
  averageOrderValue: number;
  percentage: number;
}

export interface SupplierAnalytics {
  supplierId: string;
  supplierName: string;
  totalPurchases: number;
  orderCount: number;
  averageOrderValue: number;
  percentage: number;
}

export interface CategoryStockValue {
  categoryName: string;
  stockValue: number;
  productCount: number;
  percentage: number;
}

export interface WarehouseStockValue {
  warehouseName: string;
  stockValue: number;
  productCount: number;
  percentage: number;
}

export interface FastMovingProduct {
  productId: string;
  productName: string;
  productSKU: string;
  quantitySold: number;
  salesValue: number;
  daysSinceLastSale: number;
}

export interface SlowMovingProduct {
  productId: string;
  productName: string;
  productSKU: string;
  quantitySold: number;
  salesValue: number;
  daysSinceLastSale: number;
  stockValue: number;
}

export interface CategoryProfit {
  categoryName: string;
  revenue: number;
  cost: number;
  profit: number;
  profitMargin: number;
  percentage: number;
}

