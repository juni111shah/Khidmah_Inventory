using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Khidmah_Inventory.Application.Common.Calculations;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Infrastructure.Data;
using Khidmah_Inventory.Infrastructure.Data.Interceptors;
using Khidmah_Inventory.Infrastructure.Services;
using Khidmah_Inventory.Infrastructure.Services.Calculations;

namespace Khidmah_Inventory.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());

        // Register interceptors
        services.AddScoped<AuditableEntitySaveChangesInterceptor>();
        services.AddScoped<MultiTenantInterceptor>();
        services.AddScoped<DomainChangeBroadcastInterceptor>();

        // Register services
        services.AddScoped<IIdentityService, IdentityService>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<IMultiTenantService, MultiTenantService>();
        services.AddScoped<Application.Common.Interfaces.IFileStorageService, Services.FileStorageService>();
        services.AddScoped<Application.Common.Interfaces.IFileValidationService, Services.FileValidationService>();
        services.AddScoped<IDocumentService, DocumentService>();
        services.AddScoped<IActivityLogService, ActivityLogService>();
        services.AddScoped<ISearchService, SearchService>();
        services.AddScoped<IMachineLearningService, MachineLearningService>();
        services.AddScoped<IAiRecommendationService, AiRecommendationService>();
        services.AddScoped<IAutomationExecutor, AutomationProcessor>();
        services.AddScoped<IApiKeyService, ApiKeyService>();
        services.AddScoped<Application.Common.Interfaces.IAccountingPostingService, AccountingPostingService>();
        services.AddScoped<Application.Common.Interfaces.ICurrencyConversionService, CurrencyConversionService>();

        // Calculation engine (KPI / finance / inventory)
        services.AddScoped<ICostingStrategy, AverageCostCostingStrategy>();
        services.AddScoped<ITimeSeriesHelper, TimeSeriesHelper>();
        services.AddScoped<IFinanceCalculator, FinanceCalculator>();
        services.AddScoped<IInventoryCalculator, InventoryCalculator>();
        services.AddScoped<IKpiCalculator, KpiCalculator>();
        services.AddScoped<ITaskPlanner, TaskPlannerService>();
        services.AddScoped<IRouteOptimizer, RouteOptimizerService>();
        services.AddScoped<Application.Common.Services.IIntentParserService, IntentParserService>();

        // Register repositories
        services.AddScoped<Application.Common.Interfaces.IThemeRepository, Repositories.ThemeRepository>();
        services.AddScoped<Application.Common.Interfaces.ISettingsRepository, Repositories.SettingsRepository>();
        services.AddScoped<Application.Common.Interfaces.IUserRepository, Repositories.UserRepository>();

        services.AddHttpContextAccessor();

        return services;
    }
}

