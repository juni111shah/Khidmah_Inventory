/** Single KPI with current, previous, change and trend. */
export interface KpiValueDto {
  key: string;
  label: string;
  currentValue: number;
  previousValue?: number;
  percentageChange?: number;
  trendIndicator: 'up' | 'down' | 'neutral';
  unit?: string;
  format?: string;
}

export interface TopProductKpiDto {
  productId: string;
  name: string;
  sku?: string;
  revenue: number;
  quantitySold: number;
}

export interface ExecutiveKpisDto {
  revenueToday: KpiValueDto;
  profitToday: KpiValueDto;
  lowStockCount: KpiValueDto;
  pendingApprovals: KpiValueDto;
  topProducts: TopProductKpiDto[];
  deadInventoryCount: KpiValueDto;
}

export interface SalesKpisDto {
  revenue: KpiValueDto;
  cogs: KpiValueDto;
  grossProfit: KpiValueDto;
  grossMarginPercent: KpiValueDto;
  averageOrderValue: KpiValueDto;
  orderCount: KpiValueDto;
  salesGrowthPercent: KpiValueDto;
}

export interface StockAgingBucketsDto {
  days0To30: number;
  days30To60: number;
  days60To90: number;
  days90Plus: number;
}

export interface InventoryKpisDto {
  stockValue: KpiValueDto;
  inventoryTurnover: KpiValueDto;
  daysOfInventory: KpiValueDto;
  sellThroughRate: KpiValueDto;
  deadStockCount: KpiValueDto;
  agingBuckets: StockAgingBucketsDto;
}

export interface CustomerKpisDto {
  customerCount: KpiValueDto;
  repeatRatePercent: KpiValueDto;
  averageLifetimeValue: KpiValueDto;
}

export interface KpiFilters {
  dateFrom?: string;
  dateTo?: string;
  warehouseId?: string;
  productId?: string;
  categoryId?: string;
  deadStockDays?: number;
}
