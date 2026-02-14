/** Explainable AI: why a suggestion was made */
export interface ExplainableInsight {
  type: 'reorder' | 'price_change' | 'risk';
  entityId: string;
  entityName: string;
  title: string;
  reasons: InsightReason[];
  severity: 'critical' | 'warning' | 'info';
  suggestedAction?: string;
  link?: string;
}

export interface InsightReason {
  label: string;
  value: string | number;
  unit?: string;
  trend?: 'up' | 'down' | 'stable';
}

/** What-If simulation result */
export interface WhatIfResult {
  scenario: string;
  inputs: { label: string; value: string | number }[];
  projections: {
    revenue?: number;
    margin?: number;
    stockoutDate?: string;
    profit?: number;
  };
  summary: string;
}

export interface WhatIfRequest {
  type: 'price' | 'demand' | 'supplier_delay';
  productId?: string;
  priceChangePercent?: number;
  demandChangePercent?: number;
  delayDays?: number;
  currentPrice?: number;
  currentStock?: number;
  dailyDemand?: number;
}

/** Optimization suggestion from engine */
export interface OptimizationSuggestion {
  id: string;
  type: 'reorder_quantity' | 'warehouse_transfer' | 'price_improvement' | 'slow_stock_discount';
  title: string;
  description: string;
  entityId?: string;
  entityName?: string;
  impact?: string;
  priority: 'high' | 'medium' | 'low';
  actionLabel?: string;
  actionLink?: string;
  payload?: Record<string, unknown>;
}

/** Opportunity finder item */
export interface Opportunity {
  id: string;
  type: 'high_margin' | 'co_purchased' | 'customer_target' | 'upsell';
  title: string;
  description: string;
  metric?: number;
  metricLabel?: string;
  link?: string;
  entityId?: string;
}

/** Management story: executive narrative */
export interface ManagementStory {
  generatedAt: string;
  summary: string;
  highlights: string[];
  risks: string[];
  recommendations: string[];
}

/** Forecast with confidence percentage */
export interface ForecastWithConfidence {
  productId: string;
  productName: string;
  confidencePercent: number;
  confidenceLabel: 'High' | 'Medium' | 'Low';
  averageDailyDemand?: number;
  trends?: string[];
}

/** Business health score 0-100 */
export interface BusinessHealthScore {
  score: number;
  label: string;
  grade: 'A' | 'B' | 'C' | 'D' | 'F';
  factors: HealthFactor[];
  updatedAt: string;
}

export interface HealthFactor {
  name: string;
  value: number;
  weight: number;
  status: 'good' | 'warning' | 'critical';
  description?: string;
}

/** Anomaly detection item */
export interface Anomaly {
  id: string;
  type: string;
  title: string;
  description: string;
  severity: 'high' | 'medium' | 'low';
  detectedAt: string;
  entityId?: string;
  entityType?: string;
  link?: string;
  metric?: number;
  expectedRange?: string;
}

/** Composite summary for dashboard */
export interface DecisionSupportSummary {
  explainableInsights: ExplainableInsight[];
  optimizationSuggestions: OptimizationSuggestion[];
  opportunities: Opportunity[];
  managementStory: ManagementStory | null;
  businessHealth: BusinessHealthScore | null;
  anomalies: Anomaly[];
  forecastConfidence?: ForecastWithConfidence[];
}
