export interface BranchMetric {
  id: string;
  name: string;
  type: 'company' | 'warehouse';
  revenue: number;
  revenueGrowth: number;
  orderCount: number;
  stockHealthScore: number;
  lowStockCount: number;
  rank: number;
  trend: 'up' | 'down' | 'flat';
}

export interface BranchComparisonData {
  branches: BranchMetric[];
  periodFrom: string;
  periodTo: string;
}
