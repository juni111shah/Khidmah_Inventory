using Khidmah_Inventory.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Khidmah_Inventory.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    // Auth & Users
    DbSet<User> Users { get; }
    DbSet<Role> Roles { get; }
    DbSet<Permission> Permissions { get; }
    DbSet<UserRole> UserRoles { get; }
    DbSet<RolePermission> RolePermissions { get; }

    // Company
    DbSet<Company> Companies { get; }
    DbSet<UserCompany> UserCompanies { get; }

    // Products
    DbSet<Product> Products { get; }
    DbSet<Category> Categories { get; }
    DbSet<Brand> Brands { get; }
    DbSet<UnitOfMeasure> UnitOfMeasures { get; }
    DbSet<ProductVariant> ProductVariants { get; }
    DbSet<ProductImage> ProductImages { get; }

    // Warehouse
    DbSet<Warehouse> Warehouses { get; }
    DbSet<WarehouseZone> WarehouseZones { get; }
    DbSet<Bin> Bins { get; }

    // Inventory
    DbSet<StockTransaction> StockTransactions { get; }
    DbSet<StockLevel> StockLevels { get; }
    DbSet<Batch> Batches { get; }
    DbSet<SerialNumber> SerialNumbers { get; }

    // Purchase
    DbSet<Supplier> Suppliers { get; }
    DbSet<PurchaseOrder> PurchaseOrders { get; }
    DbSet<PurchaseOrderItem> PurchaseOrderItems { get; }

    // Sales
    DbSet<Customer> Customers { get; }
    DbSet<SalesOrder> SalesOrders { get; }
    DbSet<SalesOrderItem> SalesOrderItems { get; }
    DbSet<PosSession> PosSessions { get; }

    // Settings
    DbSet<Settings> Settings { get; }

    // Collaboration
    DbSet<ActivityLog> ActivityLogs { get; }
    DbSet<Comment> Comments { get; }

    // Workflows
    DbSet<Workflow> Workflows { get; }
    DbSet<WorkflowInstance> WorkflowInstances { get; }
    DbSet<WorkflowHistory> WorkflowHistory { get; }

    // Reports
    DbSet<CustomReport> CustomReports { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

