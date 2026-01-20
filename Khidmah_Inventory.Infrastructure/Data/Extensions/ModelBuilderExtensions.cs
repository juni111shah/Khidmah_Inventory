using Microsoft.EntityFrameworkCore;
using Khidmah_Inventory.Domain.Common;

namespace Khidmah_Inventory.Infrastructure.Data.Extensions;

public static class ModelBuilderExtensions
{
    public static void ApplyGlobalFilters<T>(this ModelBuilder modelBuilder) where T : class, IMultiTenant, ISoftDeletable
    {
        modelBuilder.Entity<T>().HasQueryFilter(e => !e.IsDeleted);
    }

    public static void ApplyMultiTenantFilter<T>(this ModelBuilder modelBuilder) where T : class, IMultiTenant
    {
        // Multi-tenant filtering is applied at runtime via interceptor
        // This method can be used for additional tenant-specific configurations
    }
}

