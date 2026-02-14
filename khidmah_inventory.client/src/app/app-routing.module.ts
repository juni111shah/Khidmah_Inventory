import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { SettingsComponent } from './features/settings/settings.component';
import { UsersListComponent } from './features/users/users-list/users-list.component';
import { UserProfileComponent } from './features/users/user-profile/user-profile.component';
import { UserFormComponent } from './features/users/user-form/user-form.component';
import { RolesListComponent } from './features/roles/roles-list/roles-list.component';
import { RoleFormComponent } from './features/roles/role-form/role-form.component';
import { LoginComponent } from './features/auth/login/login.component';
import { RegisterComponent } from './features/auth/register/register.component';
import { CategoriesListComponent } from './features/categories/categories-list/categories-list.component';
import { CategoryFormComponent } from './features/categories/category-form/category-form.component';
import { ProductsListComponent } from './features/products/products-list/products-list.component';
import { ProductFormComponent } from './features/products/product-form/product-form.component';
import { WarehousesListComponent } from './features/warehouses/warehouses-list/warehouses-list.component';
import { WarehouseFormComponent } from './features/warehouses/warehouse-form/warehouse-form.component';
import { StockLevelsListComponent } from './features/inventory/stock-levels-list/stock-levels-list.component';
import { StockTransferComponent } from './features/inventory/stock-transfer/stock-transfer.component';
import { DashboardComponent } from './features/dashboard/dashboard.component';
import { ReportsComponent } from './features/reports/reports.component';
import { BarcodeScannerComponent } from './features/products/barcode-scanner/barcode-scanner.component';
import { SalesAnalyticsComponent } from './features/analytics/sales-analytics/sales-analytics.component';
import { SuppliersListComponent } from './features/suppliers/suppliers-list/suppliers-list.component';
import { SupplierFormComponent } from './features/suppliers/supplier-form/supplier-form.component';
import { PurchaseOrdersListComponent } from './features/purchase-orders/purchase-orders-list/purchase-orders-list.component';
import { PurchaseOrderFormComponent } from './features/purchase-orders/purchase-order-form/purchase-order-form.component';
import { CustomersListComponent } from './features/customers/customers-list/customers-list.component';
import { CustomerFormComponent } from './features/customers/customer-form/customer-form.component';
import { SalesOrdersListComponent } from './features/sales-orders/sales-orders-list/sales-orders-list.component';
import { SalesOrderFormComponent } from './features/sales-orders/sales-order-form/sales-order-form.component';
import { CompanyListComponent } from './features/companies/company-list/company-list.component';
import { CompanyFormComponent } from './features/companies/company-form/company-form.component';
import { CompanyUsersComponent } from './features/companies/company-users/company-users.component';
import { WorkflowListComponent } from './features/workflows/workflow-list/workflow-list.component';
import { WorkflowDesignerComponent } from './features/workflows/workflow-designer/workflow-designer.component';
import { ApprovalInboxComponent } from './features/workflows/approval-inbox/approval-inbox.component';
import { CommandCenterComponent } from './features/command-center/command-center.component';
import { AutomationListComponent } from './features/automation/automation-list/automation-list.component';
import { AutomationBuilderComponent } from './features/automation/automation-builder/automation-builder.component';
import { AutomationHistoryComponent } from './features/automation/automation-history/automation-history.component';
import { ProfitIntelligenceComponent } from './features/intelligence/profit-intelligence/profit-intelligence.component';
import { BranchPerformanceComponent } from './features/intelligence/branch-performance/branch-performance.component';
import { StaffPerformanceComponent } from './features/intelligence/staff-performance/staff-performance.component';
import { PredictiveRiskComponent } from './features/intelligence/predictive-risk/predictive-risk.component';
import { DecisionSupportPageComponent } from './features/intelligence/decision-support/decision-support-page.component';
import { DailyBriefingComponent } from './features/briefing/daily-briefing.component';
import { IntegrationCenterComponent } from './features/platform/integration-center/integration-center.component';
import { NotificationsListComponent } from './features/notifications/notifications-list/notifications-list.component';
import { ExecutiveCenterComponent } from './features/kpi/executive-center/executive-center.component';
import { SalesPerformanceComponent } from './features/kpi/sales-performance/sales-performance.component';
import { InventoryHealthComponent } from './features/kpi/inventory-health/inventory-health.component';
import { CustomerIntelligenceComponent } from './features/kpi/customer-intelligence/customer-intelligence.component';
import { ChartOfAccountsComponent } from './features/finance/chart-of-accounts/chart-of-accounts.component';
import { JournalEntriesComponent } from './features/finance/journal-entries/journal-entries.component';
import { ProfitLossComponent } from './features/finance/profit-loss/profit-loss.component';
import { BalanceSheetComponent } from './features/finance/balance-sheet/balance-sheet.component';
import { CashFlowComponent } from './features/finance/cash-flow/cash-flow.component';
import { CurrenciesListComponent } from './features/currency/currencies-list/currencies-list.component';
import { ExchangeRatesListComponent } from './features/exchange-rates/exchange-rates-list/exchange-rates-list.component';
import { HandsFreePickingComponent } from './features/hands-free/hands-free-picking/hands-free-picking.component';
import { AuthGuard } from './core/guards/auth.guard';
import { PermissionGuard } from './core/guards/permission.guard';

const routes: Routes = [
  // Public routes
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },

  // Protected routes with permissions
  {
    path: 'settings',
    component: SettingsComponent,
    canActivate: [AuthGuard, PermissionGuard],
    data: {
      permission: ['Settings:Company:Read', 'Settings:User:Read', 'Settings:System:Read', 'Settings:Notification:Read', 'Settings:UI:Read', 'Settings:Report:Read'],
      permissionMode: 'any',
      header: {
        title: 'Settings',
        description: 'Configure all aspects of your application'
      }
    }
  },
  {
    path: 'notifications',
    component: NotificationsListComponent,
    canActivate: [AuthGuard, PermissionGuard],
    data: {
      permission: 'Settings:Notification:Read',
      header: {
        title: 'Notifications',
        description: 'View and manage your notifications'
      }
    }
  },
  {
    path: 'users/profile',
    component: UserProfileComponent,
    canActivate: [AuthGuard],
    data: {
      header: {
        title: 'User Profile',
        description: 'View and manage your profile information'
      }
    }
  },
  {
    path: 'users',
    component: UsersListComponent,
    canActivate: [AuthGuard, PermissionGuard],
    data: {
      permission: 'Users:List',
      header: {
        title: 'Users',
        description: 'Manage system users and their access permissions'
      }
    }
  },
  {
    path: 'users/new',
    component: UserFormComponent,
    canActivate: [AuthGuard, PermissionGuard],
    data: {
      permission: 'Users:Create',
      header: {
        title: 'Create User',
        description: 'Add a new user to the system'
      }
    }
  },
  {
    path: 'users/:id',
    component: UserProfileComponent,
    canActivate: [AuthGuard, PermissionGuard],
    data: {
      permission: 'Users:Read',
      header: {
        title: 'User Details',
        description: 'View user information and details'
      }
    }
  },
  {
    path: 'users/:id/edit',
    component: UserFormComponent,
    canActivate: [AuthGuard, PermissionGuard],
    data: {
      permission: 'Users:Update',
      header: {
        title: 'Edit User',
        description: 'Update user information and settings'
      }
    }
  },
  {
    path: 'roles',
    component: RolesListComponent,
    canActivate: [AuthGuard, PermissionGuard],
    data: {
      permission: 'Roles:List',
      header: {
        title: 'Roles',
        description: 'Manage user roles and their permissions'
      }
    }
  },
  {
    path: 'roles/new',
    component: RoleFormComponent,
    canActivate: [AuthGuard, PermissionGuard],
    data: {
      permission: 'Roles:Create',
      header: {
        title: 'New Role',
        description: 'Create a new role with custom permissions'
      }
    }
  },
  {
    path: 'roles/:id',
    component: RoleFormComponent,
    canActivate: [AuthGuard, PermissionGuard],
    data: {
      permission: 'Roles:Read',
      header: {
        title: 'Role Details',
        description: 'View role information and permissions'
      }
    }
  },
  {
    path: 'roles/:id/edit',
    component: RoleFormComponent,
    canActivate: [AuthGuard, PermissionGuard],
    data: {
      permission: 'Roles:Update',
      header: {
        title: 'Edit Role',
        description: 'Update role information and permissions'
      }
    }
  },
  {
    path: 'categories',
    component: CategoriesListComponent,
    canActivate: [AuthGuard, PermissionGuard],
    data: {
      permission: 'Categories:List',
      header: {
        title: 'Categories',
        description: 'Manage product categories and organization'
      }
    }
  },
  {
    path: 'categories/new',
    component: CategoryFormComponent,
    canActivate: [AuthGuard, PermissionGuard],
    data: {
      permission: 'Categories:Create',
      header: {
        title: 'New Category',
        description: 'Create a new product category'
      }
    }
  },
  {
    path: 'categories/:id',
    component: CategoryFormComponent,
    canActivate: [AuthGuard, PermissionGuard],
    data: {
      permission: 'Categories:Read',
      header: {
        title: 'Category Details',
        description: 'View category information and details'
      }
    }
  },
  {
    path: 'categories/:id/edit',
    component: CategoryFormComponent,
    canActivate: [AuthGuard, PermissionGuard],
    data: {
      permission: 'Categories:Update',
      header: {
        title: 'Edit Category',
        description: 'Update category information'
      }
    }
  },
  {
    path: 'products',
    component: ProductsListComponent,
    canActivate: [AuthGuard, PermissionGuard],
    data: {
      permission: 'Products:List',
      header: {
        title: 'Products',
        description: 'Manage your product catalog and inventory'
      }
    }
  },
  {
    path: 'products/new',
    component: ProductFormComponent,
    canActivate: [AuthGuard, PermissionGuard],
    data: {
      permission: 'Products:Create',
      header: {
        title: 'New Product',
        description: 'Add a new product to your catalog'
      }
    }
  },
  {
    path: 'products/barcode-scanner',
    component: BarcodeScannerComponent,
    canActivate: [AuthGuard],
    data: {
      header: {
        title: 'Barcode Scanner',
        description: 'Scan and manage product barcodes'
      }
    }
  },
  {
    path: 'products/:id',
    component: ProductFormComponent,
    canActivate: [AuthGuard, PermissionGuard],
    data: {
      permission: 'Products:Read',
      header: {
        title: 'Product Details',
        description: 'View product information and details'
      }
    }
  },
  {
    path: 'products/:id/edit',
    component: ProductFormComponent,
    canActivate: [AuthGuard, PermissionGuard],
    data: {
      permission: 'Products:Update',
      header: {
        title: 'Edit Product',
        description: 'Update product information and details'
      }
    }
  },
  {
    path: 'warehouses',
    component: WarehousesListComponent,
    canActivate: [AuthGuard, PermissionGuard],
    data: {
      permission: 'Warehouses:List',
      header: {
        title: 'Warehouses',
        description: 'Manage warehouse locations and storage facilities'
      }
    }
  },
  {
    path: 'warehouses/new',
    component: WarehouseFormComponent,
    canActivate: [AuthGuard, PermissionGuard],
    data: {
      permission: 'Warehouses:Create',
      header: {
        title: 'New Warehouse',
        description: 'Add a new warehouse location'
      }
    }
  },
  {
    path: 'warehouses/:id',
    component: WarehouseFormComponent,
    canActivate: [AuthGuard, PermissionGuard],
    data: {
      permission: 'Warehouses:Read',
      header: {
        title: 'Warehouse Details',
        description: 'View warehouse information and details'
      }
    }
  },
  {
    path: 'warehouses/:id/edit',
    component: WarehouseFormComponent,
    canActivate: [AuthGuard, PermissionGuard],
    data: {
      permission: 'Warehouses:Update',
      header: {
        title: 'Edit Warehouse',
        description: 'Update warehouse information'
      }
    }
  },
  {
    path: 'inventory/stock-levels',
    component: StockLevelsListComponent,
    canActivate: [AuthGuard, PermissionGuard],
    data: {
      permission: 'Inventory:StockLevel:List',
      header: {
        title: 'Stock Levels',
        description: 'Monitor and manage inventory stock levels across warehouses'
      }
    }
  },
  {
    path: 'inventory/transfer',
    component: StockTransferComponent,
    canActivate: [AuthGuard, PermissionGuard],
    data: {
      permission: 'Inventory:StockTransaction:Create',
      header: {
        title: 'Transfer Stock',
        description: 'Transfer inventory between warehouses'
      }
    }
  },
  {
    path: 'inventory/batches',
    loadComponent: () => import('./features/inventory/batches-list/batches-list.component').then(m => m.BatchesListComponent),
    canActivate: [AuthGuard, PermissionGuard],
    data: {
      permission: 'Inventory:Batch:List',
      header: {
        title: 'Batches & Lots',
        description: 'Manage product batches and expiry dates'
      }
    }
  },
  {
    path: 'inventory/serial-numbers',
    loadComponent: () => import('./features/inventory/serial-numbers-list/serial-numbers-list.component').then(m => m.SerialNumbersListComponent),
    canActivate: [AuthGuard, PermissionGuard],
    data: {
      permission: 'Inventory:SerialNumber:List',
      header: {
        title: 'Serial Numbers',
        description: 'Manage unique item serial numbers'
      }
    }
  },
  {
    path: 'inventory/hands-free',
    component: HandsFreePickingComponent,
    canActivate: [AuthGuard, PermissionGuard],
    data: {
      permission: ['Inventory:StockLevel:List', 'Inventory:StockTransaction:Create'],
      permissionMode: 'all',
      header: {
        title: 'Hands-free picking',
        description: 'Voice and camera warehouse mode'
      }
    }
  },
  {
    path: 'inventory/hands-free/supervisor',
    loadComponent: () => import('./features/hands-free/hands-free-supervisor/hands-free-supervisor.component').then(m => m.HandsFreeSupervisorComponent),
    canActivate: [AuthGuard, PermissionGuard],
    data: {
      permission: 'Inventory:StockLevel:List',
      header: {
        title: 'Hands-free supervisor',
        description: 'Monitor picking sessions'
      }
    }
  },
  {
    path: 'reorder',
    loadChildren: () => import('./features/reorder/reorder.module').then(m => m.ReorderModule),
    canActivate: [AuthGuard],
    data: { header: { title: 'Reorder', description: 'Reorder management' } }
  },
  {
    path: 'companies',
    component: CompanyListComponent,
    canActivate: [AuthGuard, PermissionGuard],
    data: { permission: 'Companies:Update', header: { title: 'Companies', description: 'Manage companies' } }
  },
  {
    path: 'companies/new',
    component: CompanyFormComponent,
    canActivate: [AuthGuard, PermissionGuard],
    data: { permission: 'Companies:Update', header: { title: 'New company', description: 'Add company' } }
  },
  {
    path: 'companies/:id',
    component: CompanyFormComponent,
    canActivate: [AuthGuard, PermissionGuard],
    data: { permission: 'Companies:Update', header: { title: 'Company', description: 'Edit company' } }
  },
  {
    path: 'companies/:id/users',
    component: CompanyUsersComponent,
    canActivate: [AuthGuard, PermissionGuard],
    data: { permission: 'Companies:Update', header: { title: 'Company users', description: 'Assign users' } }
  },
  {
    path: 'workflows',
    component: WorkflowListComponent,
    canActivate: [AuthGuard, PermissionGuard],
    data: { permission: 'Workflows:Create', header: { title: 'Workflows', description: 'Manage workflows' } }
  },
  {
    path: 'workflows/designer',
    component: WorkflowDesignerComponent,
    canActivate: [AuthGuard, PermissionGuard],
    data: { permission: 'Workflows:Create', header: { title: 'Workflow designer', description: 'Create workflow' } }
  },
  {
    path: 'workflows/inbox',
    component: ApprovalInboxComponent,
    canActivate: [AuthGuard, PermissionGuard],
    data: { permission: 'Workflows:Approve', header: { title: 'Approval inbox', description: 'Pending approvals' } }
  },
  {
    path: 'reports',
    component: ReportsComponent,
    canActivate: [AuthGuard, PermissionGuard],
    data: {
      permission: ['Reports:Sales:Read', 'Reports:Inventory:Read', 'Reports:Purchase:Read'],
      permissionMode: 'any',
      header: {
        title: 'Reports',
        description: 'View and analyze business reports and analytics'
      }
    }
  },
  {
    path: 'kpi/executive',
    component: ExecutiveCenterComponent,
    canActivate: [AuthGuard, PermissionGuard],
    data: {
      permission: 'Kpi:Read',
      header: { title: 'Executive center', description: 'Key metrics and top products at a glance' }
    }
  },
  {
    path: 'kpi/sales',
    component: SalesPerformanceComponent,
    canActivate: [AuthGuard, PermissionGuard],
    data: {
      permission: 'Kpi:Read',
      header: { title: 'Sales performance', description: 'Revenue, margin, and order metrics' }
    }
  },
  {
    path: 'kpi/inventory',
    component: InventoryHealthComponent,
    canActivate: [AuthGuard, PermissionGuard],
    data: {
      permission: 'Kpi:Read',
      header: { title: 'Inventory health', description: 'Stock value, turnover, aging, and dead stock' }
    }
  },
  {
    path: 'kpi/customers',
    component: CustomerIntelligenceComponent,
    canActivate: [AuthGuard, PermissionGuard],
    data: {
      permission: 'Kpi:Read',
      header: { title: 'Customer intelligence', description: 'Customer count, repeat rate, and lifetime value' }
    }
  },
  {
    path: 'finance/accounts',
    component: ChartOfAccountsComponent,
    canActivate: [AuthGuard, PermissionGuard],
    data: {
      permission: 'Finance:Accounts:List',
      header: { title: 'Chart of accounts', description: 'Manage accounts and import standard template' }
    }
  },
  {
    path: 'finance/journals',
    component: JournalEntriesComponent,
    canActivate: [AuthGuard, PermissionGuard],
    data: {
      permission: 'Finance:Journals:Read',
      header: { title: 'Journal entries', description: 'View journal entries' }
    }
  },
  {
    path: 'finance/pl',
    component: ProfitLossComponent,
    canActivate: [AuthGuard, PermissionGuard],
    data: {
      permission: 'Finance:Statements:Read',
      header: { title: 'Profit & Loss', description: 'Income statement' }
    }
  },
  {
    path: 'finance/balance-sheet',
    component: BalanceSheetComponent,
    canActivate: [AuthGuard, PermissionGuard],
    data: {
      permission: 'Finance:Statements:Read',
      header: { title: 'Balance sheet', description: 'Assets, liabilities, equity' }
    }
  },
  {
    path: 'finance/cash-flow',
    component: CashFlowComponent,
    canActivate: [AuthGuard, PermissionGuard],
    data: {
      permission: 'Finance:Statements:Read',
      header: { title: 'Cash flow', description: 'Operating, investing, financing' }
    }
  },
  {
    path: 'currency',
    component: CurrenciesListComponent,
    canActivate: [AuthGuard, PermissionGuard],
    data: {
      permission: 'Currency:List',
      header: { title: 'Currencies', description: 'Manage company currencies and base currency' }
    }
  },
  {
    path: 'exchange-rates',
    component: ExchangeRatesListComponent,
    canActivate: [AuthGuard, PermissionGuard],
    data: {
      permission: 'ExchangeRates:List',
      header: { title: 'Exchange rates', description: 'Manage FX rates for multi-currency reporting' }
    }
  },
  {
    path: 'analytics/sales',
    component: SalesAnalyticsComponent,
    canActivate: [AuthGuard, PermissionGuard],
    data: {
      permission: 'Analytics:Sales:Read',
      header: {
        title: 'Sales Analytics',
        description: 'Analyze sales performance and trends'
      }
    }
  },
  {
    path: 'suppliers',
    component: SuppliersListComponent,
    canActivate: [AuthGuard, PermissionGuard],
    data: {
      permission: 'Suppliers:List',
      header: {
        title: 'Suppliers',
        description: 'Manage supplier information and relationships'
      }
    }
  },
  {
    path: 'suppliers/new',
    component: SupplierFormComponent,
    canActivate: [AuthGuard, PermissionGuard],
    data: {
      permission: 'Suppliers:Create',
      header: {
        title: 'New Supplier',
        description: 'Register a new supplier'
      }
    }
  },
  {
    path: 'suppliers/:id',
    component: SupplierFormComponent,
    canActivate: [AuthGuard, PermissionGuard],
    data: {
      permission: 'Suppliers:Read',
      header: {
        title: 'Supplier Details',
        description: 'View supplier information'
      }
    }
  },
  {
    path: 'suppliers/:id/edit',
    component: SupplierFormComponent,
    canActivate: [AuthGuard, PermissionGuard],
    data: {
      permission: 'Suppliers:Update',
      header: {
        title: 'Edit Supplier',
        description: 'Update supplier information'
      }
    }
  },
  {
    path: 'purchase-orders',
    component: PurchaseOrdersListComponent,
    canActivate: [AuthGuard, PermissionGuard],
    data: {
      permission: 'PurchaseOrders:List',
      header: {
        title: 'Purchase Orders',
        description: 'Manage purchase orders and procurement'
      }
    }
  },
  {
    path: 'purchase-orders/new',
    component: PurchaseOrderFormComponent,
    canActivate: [AuthGuard, PermissionGuard],
    data: {
      permission: 'PurchaseOrders:Create',
      header: {
        title: 'New Purchase Order',
        description: 'Create a new purchase order'
      }
    }
  },
  {
    path: 'purchase-orders/:id',
    component: PurchaseOrderFormComponent,
    canActivate: [AuthGuard, PermissionGuard],
    data: {
      permission: 'PurchaseOrders:Read',
      header: {
        title: 'Purchase Order Details',
        description: 'View purchase order details'
      }
    }
  },
  {
    path: 'purchase-orders/:id/edit',
    component: PurchaseOrderFormComponent,
    canActivate: [AuthGuard, PermissionGuard],
    data: {
      permission: 'PurchaseOrders:Update',
      header: {
        title: 'Edit Purchase Order',
        description: 'Modify purchase order details'
      }
    }
  },
  {
    path: 'customers',
    component: CustomersListComponent,
    canActivate: [AuthGuard, PermissionGuard],
    data: {
      permission: 'Customers:List',
      header: {
        title: 'Customers',
        description: 'Manage customer information and relationships'
      }
    }
  },
  {
    path: 'customers/new',
    component: CustomerFormComponent,
    canActivate: [AuthGuard, PermissionGuard],
    data: {
      permission: 'Customers:Create',
      header: {
        title: 'New Customer',
        description: 'Register a new customer'
      }
    }
  },
  {
    path: 'customers/:id',
    component: CustomerFormComponent,
    canActivate: [AuthGuard, PermissionGuard],
    data: {
      permission: 'Customers:Read',
      header: {
        title: 'Customer Details',
        description: 'View customer information'
      }
    }
  },
  {
    path: 'customers/:id/edit',
    component: CustomerFormComponent,
    canActivate: [AuthGuard, PermissionGuard],
    data: {
      permission: 'Customers:Update',
      header: {
        title: 'Edit Customer',
        description: 'Update customer information'
      }
    }
  },
  {
    path: 'sales-orders',
    component: SalesOrdersListComponent,
    canActivate: [AuthGuard, PermissionGuard],
    data: {
      permission: 'SalesOrders:List',
      header: {
        title: 'Sales Orders',
        description: 'Manage sales orders and transactions'
      }
    }
  },
  {
    path: 'sales-orders/new',
    component: SalesOrderFormComponent,
    canActivate: [AuthGuard, PermissionGuard],
    data: {
      permission: 'SalesOrders:Create',
      header: {
        title: 'New Sales Order',
        description: 'Create a new sales order'
      }
    }
  },
  {
    path: 'sales-orders/:id',
    component: SalesOrderFormComponent,
    canActivate: [AuthGuard, PermissionGuard],
    data: {
      permission: 'SalesOrders:Read',
      header: {
        title: 'Sales Order Details',
        description: 'View sales order details'
      }
    }
  },
  {
    path: 'sales-orders/:id/edit',
    component: SalesOrderFormComponent,
    canActivate: [AuthGuard, PermissionGuard],
    data: {
      permission: 'SalesOrders:Update',
      header: {
        title: 'Edit Sales Order',
        description: 'Modify sales order details'
      }
    }
  },
  {
    path: 'pos',
    loadChildren: () => import('./features/pos/pos.module').then(m => m.PosModule),
    canActivate: [AuthGuard],
    data: {
      header: {
        title: 'Point of Sale',
        description: 'Process retail sales and manage register sessions'
      }
    }
  },

  // Daily briefing (post-login landing)
  {
    path: 'briefing',
    component: DailyBriefingComponent,
    canActivate: [AuthGuard],
    data: {
      header: { title: 'Daily Briefing', description: 'Your business at a glance' }
    }
  },

  // Dashboard route
  {
    path: 'dashboard',
    component: DashboardComponent,
    canActivate: [AuthGuard, PermissionGuard],
    data: {
      permission: 'Dashboard:Read',
      header: {
        title: 'Dashboard',
        description: 'Overview of your business metrics and key information'
      }
    }
  },
  {
    path: 'command-center',
    component: CommandCenterComponent,
    canActivate: [AuthGuard, PermissionGuard],
    data: {
      permission: 'Dashboard:Read',
      header: { title: 'Command Center', description: 'Executive mission control' }
    }
  },
  {
    path: 'automation',
    component: AutomationListComponent,
    canActivate: [AuthGuard, PermissionGuard],
    data: { permission: 'Dashboard:Read', header: { title: 'Automation', description: 'Rule engine' } }
  },
  {
    path: 'automation/builder',
    component: AutomationBuilderComponent,
    canActivate: [AuthGuard, PermissionGuard],
    data: { permission: 'Dashboard:Read', header: { title: 'New rule', description: 'Automation builder' } }
  },
  {
    path: 'automation/history',
    component: AutomationHistoryComponent,
    canActivate: [AuthGuard, PermissionGuard],
    data: { permission: 'Dashboard:Read', header: { title: 'Execution history', description: 'Automation runs' } }
  },
  {
    path: 'intelligence/profit',
    component: ProfitIntelligenceComponent,
    canActivate: [AuthGuard, PermissionGuard],
    data: { permission: 'Reports:Inventory:Read', header: { title: 'Profit intelligence', description: 'Margin and dead stock' } }
  },
  {
    path: 'intelligence/branch',
    component: BranchPerformanceComponent,
    canActivate: [AuthGuard, PermissionGuard],
    data: { permission: 'Reports:Sales:Read', header: { title: 'Branch performance', description: 'Compare branches' } }
  },
  {
    path: 'intelligence/staff',
    component: StaffPerformanceComponent,
    canActivate: [AuthGuard, PermissionGuard],
    data: { permission: 'Reports:Sales:Read', header: { title: 'Staff performance', description: 'Sales and speed' } }
  },
  {
    path: 'intelligence/risks',
    component: PredictiveRiskComponent,
    canActivate: [AuthGuard, PermissionGuard],
    data: { permission: 'Dashboard:Read', header: { title: 'Predictive risk', description: 'AI warnings' } }
  },
  {
    path: 'intelligence/decisions',
    component: DecisionSupportPageComponent,
    canActivate: [AuthGuard, PermissionGuard],
    data: { permission: 'Dashboard:Read', header: { title: 'Decision support', description: 'Actionable intelligence & optimization' } }
  },
  {
    path: 'platform',
    component: IntegrationCenterComponent,
    canActivate: [AuthGuard, PermissionGuard],
    data: {
      permission: ['Platform:ApiKeys:List', 'Platform:Webhooks:List', 'Platform:Integrations:List', 'Platform:ScheduledReports:List', 'Platform:ApiKeys:Usage'],
      permissionMode: 'any',
      header: { title: 'Integration Center', description: 'API keys, webhooks, integrations & scheduled reports' }
    }
  },

  // Autonomous warehouse
  {
    path: 'autonomous',
    loadComponent: () => import('./features/autonomous-warehouse/autonomous-dashboard/autonomous-dashboard.component').then(m => m.AutonomousDashboardComponent),
    canActivate: [AuthGuard, PermissionGuard],
    data: {
      permission: 'Warehouses:List',
      header: { title: 'Autonomous Warehouse', description: 'Task planning, routes, live ops' }
    }
  },
  {
    path: 'autonomous/routes',
    loadComponent: () => import('./features/autonomous-warehouse/routes/routes.component').then(m => m.RoutesComponent),
    canActivate: [AuthGuard, PermissionGuard],
    data: { permission: 'Warehouses:Read', header: { title: 'Route optimization', description: 'Optimal task sequence' } }
  },
  {
    path: 'autonomous/live-ops',
    loadComponent: () => import('./features/autonomous-warehouse/live-ops/live-ops.component').then(m => m.LiveOpsComponent),
    canActivate: [AuthGuard, PermissionGuard],
    data: { permission: 'Warehouses:List', header: { title: 'Live Ops', description: 'Real-time operations board' } }
  },
  {
    path: 'copilot',
    loadComponent: () => import('./features/copilot/chat-assistant/chat-assistant.component').then(m => m.ChatAssistantComponent),
    canActivate: [AuthGuard, PermissionGuard],
    data: {
      permission: 'Dashboard:Read',
      header: { title: 'AI Copilot Assistant', description: 'Conversational chat + voice operations' }
    }
  },

  // Default route
  { path: '', redirectTo: '/dashboard', pathMatch: 'full' },
  { path: '**', redirectTo: '/dashboard' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
