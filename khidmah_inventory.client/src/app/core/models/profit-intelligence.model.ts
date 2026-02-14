export interface MarginByProduct {
  productId: string;
  productName: string;
  categoryName: string;
  revenue: number;
  cost: number;
  profit: number;
  marginPercent: number;
  trend?: 'up' | 'down' | 'flat';
}

export interface MarginByCategory {
  categoryId: string;
  categoryName: string;
  revenue: number;
  cost: number;
  profit: number;
  marginPercent: number;
  productCount: number;
}

export interface DeadStockItem {
  productId: string;
  productName: string;
  sku: string;
  quantity: number;
  value: number;
  lastMovementDate?: string;
  daysInStock: number;
}

export interface AgingBucket {
  range: string;
  count: number;
  value: number;
  percent: number;
}

export interface MoverSegment {
  type: 'fast' | 'slow';
  productId: string;
  productName: string;
  quantitySold: number;
  daysSupply: number;
  velocity: number;
}

export interface ProfitIntelligenceData {
  marginByProduct: MarginByProduct[];
  marginByCategory: MarginByCategory[];
  deadStock: DeadStockItem[];
  agingInventory: AgingBucket[];
  capitalLocked: number;
  fastMovers: MoverSegment[];
  slowMovers: MoverSegment[];
  summary: {
    totalMarginPercent: number;
    totalDeadStockValue: number;
    totalCapitalLocked: number;
  };
}
