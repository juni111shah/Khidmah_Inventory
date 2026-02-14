export interface ProductIntelligence {
  productId: string;
  salesVelocity: number;
  stockDaysRemaining?: number;
  reorderRisk: string;
  marginTrend: string;
  currentMarginPercent: number;
  previousMarginPercent?: number;
  priceHistory: PriceHistoryPoint[];
  forecastVsActualVariance?: number;
  abcClassification: string;
  abcRevenueSharePercent: number;
  recommendedActions: string[];
}

export interface PriceHistoryPoint {
  date: string;
  salePrice: number;
  costPrice?: number;
}

export interface DashboardIntelligence {
  predictions: Prediction[];
  anomalies: Anomaly[];
  risks: Risk[];
  opportunities: Opportunity[];
}

export interface Prediction {
  label: string;
  value: string;
  trend: string;
  period: string;
}

export interface Anomaly {
  metric: string;
  description: string;
  severity: string;
  detectedAt: string;
  drillDownRoute?: string;
}

export interface Risk {
  title: string;
  description: string;
  severity: string;
  actionRoute?: string;
  entityId?: string;
}

export interface Opportunity {
  title: string;
  description: string;
  actionRoute?: string;
}

export interface AiRecommendation {
  productId: string;
  productName: string;
  productSKU: string;
  riskLevel: string;
  suggestedReorderQuantity: number;
  suggestedSalePrice: number;
  recommendedSupplierName: string;
  stockoutProbability: number;
  abnormalSalesDetected: boolean;
  reasoning: string;
}
