import { Injectable } from '@angular/core';
import { PermissionService } from './permission.service';
import { Observable, map } from 'rxjs';

export interface NavigationItem {
  label: string;
  icon?: string;
  route?: string;
  children?: NavigationItem[];
  badge?: string | number;
  badgeColor?: string;
  permission?: string | string[];
  permissionMode?: 'any' | 'all';
  role?: string | string[];
  roleMode?: 'any' | 'all';
}

@Injectable({
  providedIn: 'root'
})
export class NavigationService {
  private allMenuItems: NavigationItem[] = [
    {
      label: 'Dashboard',
      icon: 'speedometer2',
      route: '/dashboard',
      permission: 'Dashboard:Read'
    },
    {
      label: 'Daily Briefing',
      icon: 'sunrise',
      route: '/briefing',
      permission: 'Dashboard:Read'
    },
    {
      label: 'Command Center',
      icon: 'bullseye',
      route: '/command-center',
      permission: 'Dashboard:Read'
    },
    {
      label: 'Reports',
      icon: 'file-earmark-bar-graph',
      route: '/reports',
      permission: ['Reports:Sales:Read', 'Reports:Inventory:Read', 'Reports:Purchase:Read'],
      permissionMode: 'any'
    },
    {
      label: 'KPI Center',
      icon: 'graph-up',
      permission: 'Kpi:Read',
      children: [
        { label: 'Executive center', icon: 'briefcase', route: '/kpi/executive', permission: 'Kpi:Read' },
        { label: 'Sales performance', icon: 'currency-dollar', route: '/kpi/sales', permission: 'Kpi:Read' },
        { label: 'Inventory health', icon: 'boxes', route: '/kpi/inventory', permission: 'Kpi:Read' },
        { label: 'Customer intelligence', icon: 'people', route: '/kpi/customers', permission: 'Kpi:Read' }
      ]
    },
    {
      label: 'Finance',
      icon: 'journal-bookmark',
      permission: ['Finance:Accounts:List', 'Currency:List', 'ExchangeRates:List'],
      permissionMode: 'any',
      children: [
        { label: 'Chart of accounts', icon: 'list-ul', route: '/finance/accounts', permission: 'Finance:Accounts:List' },
        { label: 'Currencies', icon: 'currency-exchange', route: '/currency', permission: 'Currency:List' },
        { label: 'Exchange rates', icon: 'arrow-left-right', route: '/exchange-rates', permission: 'ExchangeRates:List' },
        { label: 'Journal entries', icon: 'journal-text', route: '/finance/journals', permission: 'Finance:Journals:Read' },
        { label: 'P&L', icon: 'graph-up', route: '/finance/pl', permission: 'Finance:Statements:Read' },
        { label: 'Balance sheet', icon: 'balance-scale', route: '/finance/balance-sheet', permission: 'Finance:Statements:Read' },
        { label: 'Cash flow', icon: 'cash-stack', route: '/finance/cash-flow', permission: 'Finance:Statements:Read' }
      ]
    },
    {
      label: 'Intelligence',
      icon: 'graph-up-arrow',
      permission: ['Reports:Sales:Read', 'Reports:Inventory:Read', 'Dashboard:Read'],
      permissionMode: 'any',
      children: [
        { label: 'Profit intelligence', icon: 'currency-dollar', route: '/intelligence/profit', permission: 'Reports:Inventory:Read' },
        { label: 'Branch performance', icon: 'building', route: '/intelligence/branch', permission: 'Reports:Sales:Read' },
        { label: 'Staff performance', icon: 'people', route: '/intelligence/staff', permission: 'Reports:Sales:Read' },
        { label: 'Predictive risk', icon: 'exclamation-triangle', route: '/intelligence/risks', permission: 'Dashboard:Read' },
        { label: 'Decision support', icon: 'lightbulb', route: '/intelligence/decisions', permission: 'Dashboard:Read' }
      ]
    },
    {
      label: 'Automation',
      icon: 'gear-wide-connected',
      route: '/automation',
      permission: 'Dashboard:Read'
    },
    {
      label: 'Users',
      icon: 'people',
      route: '/users',
      permission: 'Users:List'
    },
    {
      label: 'Roles',
      icon: 'shield-lock',
      route: '/roles',
      permission: 'Roles:List'
    },
    {
      label: 'Companies',
      icon: 'building',
      route: '/companies',
      permission: 'Companies:Update'
    },
    {
      label: 'Workflows',
      icon: 'diagram-3',
      route: '/workflows',
      permission: ['Workflows:Create', 'Workflows:Approve'],
      permissionMode: 'any'
    },
    {
      label: 'Integration Center',
      icon: 'plug',
      route: '/platform',
      permission: ['Platform:ApiKeys:List', 'Platform:Webhooks:List', 'Platform:Integrations:List', 'Platform:ScheduledReports:List', 'Platform:ApiKeys:Usage'],
      permissionMode: 'any'
    },
    {
      label: 'Categories',
      icon: 'folder',
      route: '/categories',
      permission: 'Categories:List'
    },
    {
      label: 'Products',
      icon: 'box-seam',
      route: '/products',
      permission: 'Products:List'
    },
    {
      label: 'Warehouses',
      icon: 'house-door',
      route: '/warehouses',
      permission: 'Warehouses:List'
    },
    {
      label: 'Autonomous Warehouse',
      icon: 'robot',
      permission: 'Warehouses:List',
      children: [
        { label: 'Dashboard', icon: 'speedometer2', route: '/autonomous', permission: 'Warehouses:List' },
        { label: 'Routes', icon: 'signpost-2', route: '/autonomous/routes', permission: 'Warehouses:Read' },
        { label: 'Live Ops', icon: 'broadcast', route: '/autonomous/live-ops', permission: 'Warehouses:List' }
      ]
    },
    {
      label: 'AI Copilot',
      icon: 'chat-dots',
      route: '/copilot',
      permission: 'Dashboard:Read'
    },
    {
      label: 'Inventory',
      icon: 'boxes',
      permission: ['Inventory:StockLevel:List', 'Inventory:StockTransaction:Create'],
      permissionMode: 'any',
      children: [
        {
          label: 'Stock Levels',
          icon: 'layers',
          route: '/inventory/stock-levels',
          permission: 'Inventory:StockLevel:List'
        },
        {
          label: 'Transfer Stock',
          icon: 'arrow-left-right',
          route: '/inventory/transfer',
          permission: 'Inventory:StockTransaction:Create'
        },
        {
          label: 'Batches & Lots',
          icon: 'archive',
          route: '/inventory/batches',
          permission: 'Inventory:Batch:List'
        },
        {
          label: 'Serial Numbers',
          icon: 'upc-scan',
          route: '/inventory/serial-numbers',
          permission: 'Inventory:SerialNumber:List'
        },
        {
          label: 'Hands-free picking',
          icon: 'mic',
          route: '/inventory/hands-free',
          permission: 'Inventory:StockTransaction:Create'
        },
        {
          label: 'Hands-free supervisor',
          icon: 'person-badge',
          route: '/inventory/hands-free/supervisor',
          permission: 'Inventory:StockLevel:List'
        },
        {
          label: 'Reorder',
          icon: 'arrow-repeat',
          route: '/reorder',
          permission: 'Reordering:Suggestions:Read'
        }
      ]
    },
    {
      label: 'Purchase',
      icon: 'cart-check',
      permission: ['Suppliers:List', 'PurchaseOrders:List'],
      permissionMode: 'any',
      children: [
        {
          label: 'Suppliers',
          icon: 'truck',
          route: '/suppliers',
          permission: 'Suppliers:List'
        },
        {
          label: 'Purchase Orders',
          icon: 'file-earmark-text',
          route: '/purchase-orders',
          permission: 'PurchaseOrders:List'
        }
      ]
    },
    {
      label: 'Sales',
      icon: 'cash-coin',
      permission: ['Customers:List', 'SalesOrders:List'],
      permissionMode: 'any',
      children: [
        {
          label: 'Customers',
          icon: 'person-lines-fill',
          route: '/customers',
          permission: 'Customers:List'
        },
        {
          label: 'Sales Orders',
          icon: 'bag-check',
          route: '/sales-orders',
          permission: 'SalesOrders:List'
        },
        {
          label: 'Point of Sale (POS)',
          icon: 'shop',
          route: '/pos',
          permission: 'SalesOrders:Create'
        }
      ]
    },
    {
      label: 'Notifications',
      icon: 'bell',
      route: '/notifications',
      permission: 'Settings:Notification:Read'
    },
    {
      label: 'Settings',
      icon: 'gear',
      route: '/settings',
      permission: 'Settings:UI:Read'
    }
  ];

  constructor(private permissionService: PermissionService) {}

  getMenuItems(): Observable<NavigationItem[]> {
    return this.permissionService.currentUser$.pipe(
      map(() => this.filterMenuItemsByPermission(this.allMenuItems))
    );
  }

  getMenuItemsSync(): NavigationItem[] {
    return this.filterMenuItemsByPermission(this.allMenuItems);
  }

  private filterMenuItemsByPermission(items: NavigationItem[]): NavigationItem[] {
    return items
      .filter(item => this.hasAccess(item))
      .map(item => {
        if (item.children && item.children.length > 0) {
          const filteredChildren = this.filterMenuItemsByPermission(item.children);
          // Only include parent if it has accessible children or has its own route
          if (filteredChildren.length > 0 || item.route) {
            return {
              ...item,
              children: filteredChildren.length > 0 ? filteredChildren : undefined
            };
          }
          return null;
        }
        return item;
      })
      .filter((item): item is NavigationItem => item !== null);
  }

  private hasAccess(item: NavigationItem): boolean {
    // Check permission
    if (item.permission) {
      if (Array.isArray(item.permission)) {
        const hasPermission = item.permissionMode === 'all'
          ? this.permissionService.hasAllPermissions(item.permission)
          : this.permissionService.hasAnyPermission(item.permission);
        if (!hasPermission) {
          return false;
        }
      } else {
        if (!this.permissionService.hasPermission(item.permission)) {
          return false;
        }
      }
    }

    // Check role
    if (item.role) {
      if (Array.isArray(item.role)) {
        const hasRole = item.roleMode === 'all'
          ? item.role.every(r => this.permissionService.hasRole(r))
          : this.permissionService.hasAnyRole(item.role);
        if (!hasRole) {
          return false;
        }
      } else {
        if (!this.permissionService.hasRole(item.role)) {
          return false;
        }
      }
    }

    // If no permission or role specified, allow access
    return true;
  }
}

