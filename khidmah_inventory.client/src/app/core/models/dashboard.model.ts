export interface Dashboard {
  summary: DashboardSummary;
  salesChartData: SalesChartData[];
  inventoryChartData: InventoryChartData[];
  topProducts: TopProduct[];
  lowStockProducts: LowStockProduct[];
  recentOrders: RecentOrder[];
}

export interface DashboardSummary {
  totalProducts: number;
  totalCategories: number;
  totalWarehouses: number;
  totalSuppliers: number;
  totalCustomers: number;
  totalStockValue: number;
  lowStockItems: number;
  pendingPurchaseOrders: number;
  pendingSalesOrders: number;
  todaySales: number;
  todayPurchases: number;
  monthlySales: number;
  monthlyPurchases: number;
}

export interface SalesChartData {
  date: string;
  sales: number;
  purchases: number;
}

export interface InventoryChartData {
  categoryName: string;
  stockValue: number;
  productCount: number;
}

export interface TopProduct {
  productId: string;
  productName: string;
  productSKU: string;
  totalSales: number;
  quantitySold: number;
}

export interface LowStockProduct {
  productId: string;
  productName: string;
  productSKU: string;
  warehouseName: string;
  currentStock: number;
  minStockLevel: number;
}

export interface RecentOrder {
  id: string;
  orderNumber: string;
  type: string;
  customerOrSupplierName: string;
  totalAmount: number;
  status: string;
  orderDate: string;
}

