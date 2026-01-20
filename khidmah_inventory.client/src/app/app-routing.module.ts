import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { SettingsComponent } from './features/settings/settings.component';
import { UsersListComponent } from './features/users/users-list/users-list.component';
import { UserProfileComponent } from './features/users/user-profile/user-profile.component';
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
import { PurchaseOrdersListComponent } from './features/purchase-orders/purchase-orders-list/purchase-orders-list.component';
import { CustomersListComponent } from './features/customers/customers-list/customers-list.component';
import { SalesOrdersListComponent } from './features/sales-orders/sales-orders-list/sales-orders-list.component';
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
    component: UserProfileComponent, 
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
  
  // Default route
  { path: '', redirectTo: '/dashboard', pathMatch: 'full' },
  { path: '**', redirectTo: '/dashboard' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
