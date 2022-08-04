using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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

            using (var scope = sp.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<ProjectDbContext>();

                db.Database.EnsureCreated();
            }
        });
        
        builder.ConfigureAppConfiguration(configurationBuilder =>
            configurationBuilder.AddJsonFile(Path.Combine(Directory.GetCurrentDirectory(), "appsettings.Test.json")));
    }
}