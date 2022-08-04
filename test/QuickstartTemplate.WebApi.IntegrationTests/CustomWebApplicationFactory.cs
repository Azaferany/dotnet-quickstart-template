using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using QuickstartTemplate.ApplicationCore.Contracts;
using QuickstartTemplate.Infrastructure.DbContexts;

namespace QuickstartTemplate.WebApi.IntegrationTests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");
        builder.ConfigureServices(services =>
        {

            #region remove Npgsql Ef and setup InMemory 

            var dummyServices = new ServiceCollection();
            dummyServices.AddEntityFrameworkNpgsql();
            services.AddDbContextPool<IProjectDbContext, ProjectDbContext>((provider, options) =>
            {
                options.UseNpgsql("test npgsql");
                options.UseInternalServiceProvider(provider);
            });
            
            foreach (var service in dummyServices)
            {
                if(service.ServiceType == typeof(ILoggerFactory))
                    continue;
                
                services.RemoveAll(service.ServiceType);
            }
            
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType ==
                     typeof(DbContextOptions<ProjectDbContext>));

            services.Remove(descriptor);

            services.AddEntityFrameworkInMemoryDatabase();
            
            services.AddDbContextPool<IProjectDbContext, ProjectDbContext>((provider, options) =>
            {
                options.UseInMemoryDatabase("InMemoryDbForTesting");
                options.UseInternalServiceProvider(provider);
            });
            
            var sp = services.BuildServiceProvider();

            using var scope = sp.CreateScope();
            var scopedServices = scope.ServiceProvider;
            var db = scopedServices.GetRequiredService<ProjectDbContext>();

            db.Database.EnsureCreated();

            #endregion

        });
        
        builder.ConfigureAppConfiguration(configurationBuilder =>
            configurationBuilder.AddJsonFile(Path.Combine(Directory.GetCurrentDirectory(), "appsettings.Test.json")));
    }
}