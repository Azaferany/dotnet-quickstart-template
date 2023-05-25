using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using QuickstartTemplate.ApplicationCore.Contracts;
using QuickstartTemplate.Infrastructure.DbContexts;
using QuickstartTemplate.WebApi.IntegrationTests.Helpers;

namespace QuickstartTemplate.WebApi.IntegrationTests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");
        builder.ConfigureServices(services =>
        {

            #region remove Npgsql Ef and setup InMemory

            var descriptor = services.SingleOrDefault(
                d => d.ServiceType ==
                     typeof(DbContextOptions<ProjectDbContext>));

            services.Remove(descriptor);

            services.AddDbContextPool<IProjectDbContext, ProjectDbContext>((provider, options) =>
            {
                options.UseInMemoryDatabase("InMemoryDbForTesting");
            });

            var sp = services.BuildServiceProvider();

            using var scope = sp.CreateScope();
            var scopedServices = scope.ServiceProvider;
            var db = scopedServices.GetRequiredService<ProjectDbContext>();

            db.Database.EnsureCreated();

            #endregion

            services.Configure<JwtBearerOptions>("Bearer", options =>
            {
                var config = new OpenIdConnectConfiguration()
                {
                    Issuer = MockJwtTokens.Issuer,
                };

                config.SigningKeys.Add(MockJwtTokens.SecurityKey);
                options.Configuration = config;
                options.Audience = MockJwtTokens.Audience;
            });
        });

        builder.ConfigureAppConfiguration(configurationBuilder =>
            configurationBuilder.AddJsonFile(Path.Combine(Directory.GetCurrentDirectory(), "appsettings.Test.json")));
    }
}
