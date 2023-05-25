using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using QuickstartTemplate.ApplicationCore.Contracts;
using QuickstartTemplate.Infrastructure.DbContexts;

namespace QuickstartTemplate.Infrastructure;

public static class InfrastructureSetup
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {

        services.AddDbContextPool<IProjectDbContext, ProjectDbContext>((serviceProvider, optionsBuilder) =>
        {
            optionsBuilder.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));

            // Second level caching
            // optionsBuilder.AddInterceptors(serviceProvider.GetRequiredService<SecondLevelCacheInterceptor>());
        });

        // Second level caching is a query cache. The results of EF commands will be stored in the cache,so that the
        // same EF commands will retrieve their data from the cache rather than executing them against the database again.
        // https://github.com/VahidN/EFCoreSecondLevelCacheInterceptor
        // services.AddEFSecondLevelCache(options =>
        //     options.UseMemoryCacheProvider().DisableLogging(true).UseCacheKeyPrefix("EF_"));

        return services;
    }
}
