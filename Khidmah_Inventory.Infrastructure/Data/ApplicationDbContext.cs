using Microsoft.EntityFrameworkCore;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Domain.Entities;
using Khidmah_Inventory.Domain.Common;
using Khidmah_Inventory.Infrastructure.Data.Interceptors;

namespace Khidmah_Inventory.Infrastructure.Data;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    private readonly AuditableEntitySaveChangesInterceptor _auditableEntitySaveChangesInterceptor;
    private readonly MultiTenantInterceptor _multiTenantInterceptor;

    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        AuditableEntitySaveChangesInterceptor auditableEntitySaveChangesInterceptor,
        MultiTenantInterceptor multiTenantInterceptor) : base(options)
    {
        _auditableEntitySaveChangesInterceptor = auditableEntitySaveChangesInterceptor;
        _multiTenantInterceptor = multiTenantInterceptor;
    }

    // Auth & Users
    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();

    // Company
    public DbSet<Company> Companies => Set<Company>();
    public DbSet<UserCompany> UserCompanies => Set<UserCompany>();

    // Products
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Brand> Brands => Set<Brand>();
    public DbSet<UnitOfMeasure> UnitOfMeasures => Set<UnitOfMeasure>();
    public DbSet<ProductVariant> ProductVariants => Set<ProductVariant>();
    public DbSet<ProductImage> ProductImages => Set<ProductImage>();

    // Warehouse
    public DbSet<Warehouse> Warehouses => Set<Warehouse>();
    public DbSet<WarehouseZone> WarehouseZones => Set<WarehouseZone>();
    public DbSet<Bin> Bins => Set<Bin>();

    // Inventory
    public DbSet<StockTransaction> StockTransactions => Set<StockTransaction>();
    public DbSet<StockLevel> StockLevels => Set<StockLevel>();
    public DbSet<Batch> Batches => Set<Batch>();
    public DbSet<SerialNumber> SerialNumbers => Set<SerialNumber>();

    // Purchase
    public DbSet<Supplier> Suppliers => Set<Supplier>();
    public DbSet<PurchaseOrder> PurchaseOrders => Set<PurchaseOrder>();
    public DbSet<PurchaseOrderItem> PurchaseOrderItems => Set<PurchaseOrderItem>();

    // Sales
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<SalesOrder> SalesOrders => Set<SalesOrder>();
    public DbSet<SalesOrderItem> SalesOrderItems => Set<SalesOrderItem>();
    public DbSet<PosSession> PosSessions => Set<PosSession>();

    // Settings
    public DbSet<Settings> Settings => Set<Settings>();

    // Collaboration
    public DbSet<ActivityLog> ActivityLogs => Set<ActivityLog>();
    public DbSet<Comment> Comments => Set<Comment>();

    // Workflows
    public DbSet<Workflow> Workflows => Set<Workflow>();
    public DbSet<WorkflowInstance> WorkflowInstances => Set<WorkflowInstance>();
    public DbSet<WorkflowHistory> WorkflowHistory => Set<WorkflowHistory>();

    // Reports
    public DbSet<CustomReport> CustomReports => Set<CustomReport>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.AddInterceptors(_auditableEntitySaveChangesInterceptor, _multiTenantInterceptor);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}

