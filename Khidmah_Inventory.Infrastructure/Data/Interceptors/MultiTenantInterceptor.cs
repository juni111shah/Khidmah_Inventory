using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Khidmah_Inventory.Domain.Common;
using Khidmah_Inventory.Application.Common.Interfaces;

namespace Khidmah_Inventory.Infrastructure.Data.Interceptors;

public class MultiTenantInterceptor : SaveChangesInterceptor
{
    private readonly IMultiTenantService _multiTenantService;

    public MultiTenantInterceptor(IMultiTenantService multiTenantService)
    {
        _multiTenantService = multiTenantService;
    }

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        SetCompanyId(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        SetCompanyId(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void SetCompanyId(DbContext? context)
    {
        if (context == null || !_multiTenantService.IsMultiTenantEnabled) return;

        var companyId = _multiTenantService.GetCurrentCompanyId();
        if (!companyId.HasValue) return;

        foreach (var entry in context.ChangeTracker.Entries<IMultiTenant>())
        {
            if (entry.State == EntityState.Added)
            {
                if (entry.Entity is BaseEntity baseEntity)
                {
                    baseEntity.SetCompanyId(companyId.Value);
                }
            }
        }
    }
}

