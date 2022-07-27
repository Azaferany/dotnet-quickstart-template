using IdentityModel.AspNetCore.AccessTokenValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using QuickstartTemplate.ApplicationCore;
using QuickstartTemplate.ApplicationCore.Resources;
using QuickstartTemplate.Infrastructure;
using Serilog;

namespace QuickstartTemplate.WebApi;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        _configuration = configuration; 
    }

    private readonly IConfiguration _configuration;

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddLocalization();

        services.AddControllers()
            .AddDataAnnotationsLocalization(options => {
                options.DataAnnotationLocalizerProvider = (type, factory) =>
                    factory.Create(typeof(SharedResource));
            });

        services.AddApiVersioning(o =>
        {
            o.AssumeDefaultVersionWhenUnspecified = false;
            o.DefaultApiVersion = new ApiVersion(1, 0);
            o.ReportApiVersions = true;
            o.ApiVersionReader = new UrlSegmentApiVersionReader();
        });
        
        services.AddAuthentication("Bearer")

            // JWT tokens (default scheme)
            .AddJwtBearer("Bearer", options =>
            {
                _configuration.Bind("Authentication", options);

                options.MapInboundClaims = false;
                options.TokenValidationParameters.ValidTypes = new[] { "at+jwt" };
                options.SaveToken = true;
                // if token does not contain a dot, it is a reference token
                options.ForwardDefaultSelector = Selector.ForwardReferenceToken("Introspection");
            })

            // reference tokens
            .AddOAuth2Introspection("Introspection", options =>
            {
                _configuration.Bind("Authentication", options);

                options.EnableCaching = true;
            });

        services.AddScopeTransformation();

        services.AddAuthorization(options =>
        {
            options.AddPolicy("admin",
                policy => policy.RequireScope("QuickstartTemplate:admin"));
            options.AddPolicy("read",
                policy => policy.RequireScope("QuickstartTemplate:read"));
            options.AddPolicy("write",
                policy => policy.RequireScope("QuickstartTemplate:write"));
        });
        
        services.AddInfrastructure();
        services.AddApplication();
        
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        services.AddVersionedApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'VVV";
            options.SubstituteApiVersionInUrl = true;
        });
        
        services.AddSwaggerGen();
        
        //for more information read
        //https://docs.microsoft.com/en-us/aspnet/core/fundamentals/http-logging/?view=aspnetcore-6.0
        //dont log Response if grpc is added it will break; track bug in below issue
        //https://github.com/dotnet/aspnetcore/issues/39317
        services.AddHttpLogging(options => _configuration.Bind("HttpLogging", options));
    }

    public void Configure(WebApplication app)
    {
        app.UseRequestLocalization(options =>
            options.AddSupportedCultures("en-US", "fr-IR").SetDefaultCulture("en-US"));
        
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        app.UseRouting();
        
        app.UseSerilogRequestLogging();
        
        //https://josef.codes/asp-net-core-6-http-logging-log-requests-responses/
        app.UseHttpLogging();
        
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();
    }
}