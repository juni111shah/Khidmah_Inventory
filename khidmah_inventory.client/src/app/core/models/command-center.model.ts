import { Dashboard } from './dashboard.model';

/** Executive / Mission Control view - extends dashboard with extra widgets */
export interface CommandCenterData {
  dashboard: Dashboard | null;
  salesToday: number;
  salesWeek: number;
  salesMonth: number;
  purchaseVolume: number;
  profitEstimate: number;
  lowStockAlerts: number;
  expiringItemsCount: number;
  pendingApprovalsCount: number;
  topProducts: { productName: string; productId: string; value: number; trend: 'up' | 'down' | 'flat' }[];
  warehouseUtilization: { warehouseName: string; usedPercent: number; capacity?: number }[];
  recentActivities: { id: string; type: string; description: string; timeAgo: string; link?: string }[];
}
