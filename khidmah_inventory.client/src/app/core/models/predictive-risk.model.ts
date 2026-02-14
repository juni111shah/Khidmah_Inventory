export type RiskType = 'OutOfStockSoon' | 'Overstock' | 'Expiry' | 'AbnormalSales';

export interface RiskItem {
  id: string;
  type: RiskType;
  severity: 'high' | 'medium' | 'low';
  title: string;
  description: string;
  entityType: string;
  entityId: string;
  productName?: string;
  warehouseName?: string;
  metric?: number;
  unit?: string;
  suggestedAction?: string;
  link?: string;
  createdAt: string;
}

export interface PredictiveRiskData {
  risks: RiskItem[];
  summary: {
    high: number;
    medium: number;
    low: number;
  };
}
