/** Status badge for briefing items */
export type BriefingStatus = 'CRITICAL' | 'WATCH' | 'GOOD';

export interface DailyBriefing {
  /** Welcome / context */
  generatedAt: string;
  /** Yesterday's sales total */
  yesterdaySales: number;
  /** e.g. +5.2 or -2.1 (percent vs previous period) */
  salesTrendPercent: number;
  salesTrendDirection: 'up' | 'down' | 'flat';
  /** Low stock: count and top items */
  lowStockRisks: LowStockRiskItem[];
  lowStockStatus: BriefingStatus;
  /** Expiring batches count and sample */
  expiringBatches: ExpiringBatchItem[];
  expiringStatus: BriefingStatus;
  /** Pending workflow approvals */
  pendingApprovals: PendingApprovalItem[];
  pendingApprovalsCount: number;
  /** Top selling products (e.g. last 7 days) */
  topSellingProducts: BriefingProductItem[];
  /** Slow moving items (suggestion) */
  slowMovingItems: BriefingProductItem[];
  /** Estimated profit (e.g. month or period) */
  estimatedProfit: number;
  profitTrendPercent: number;
  profitTrendDirection: 'up' | 'down' | 'flat';
  /** Unusual activity (e.g. spikes, anomalies) */
  unusualActivity: UnusualActivityItem[];
  /** AI / suggestion lines */
  aiSuggestions: AiSuggestionItem[];
}

export interface LowStockRiskItem {
  productId: string;
  productName: string;
  warehouseName: string;
  currentStock: number;
  minStockLevel: number;
  priority: 'Critical' | 'High' | 'Medium' | 'Low';
  link?: string;
}

export interface ExpiringBatchItem {
  batchId?: string;
  productName: string;
  batchNumber: string;
  quantity: number;
  expiryDate: string;
  daysUntilExpiry: number;
  link?: string;
}

export interface PendingApprovalItem {
  id: string;
  entityType: string;
  entityId: string;
  currentStep: string;
  description?: string;
  link?: string;
}

export interface BriefingProductItem {
  productId: string;
  productName: string;
  value: number;
  trend?: 'up' | 'down' | 'flat';
  unit?: string;
  link?: string;
}

export interface UnusualActivityItem {
  id: string;
  type: string;
  description: string;
  severity: 'high' | 'medium' | 'low';
  link?: string;
}

export interface AiSuggestionItem {
  id: string;
  title: string;
  description: string;
  actionLabel?: string;
  actionLink?: string;
  priority: BriefingStatus;
}
