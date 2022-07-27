using IdentityModel.AspNetCore.AccessTokenValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.OpenApi.Models;
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
            .AddDataAnnotationsLocalization(options =>
            {
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

          services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo()
            {
                Title = "v1",
                Version = "v1"
            });
            options.SwaggerDoc("v2", new OpenApiInfo()
            {
                Title = "v2",
                Version = "v2"
            });
            options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows
                    {
                        Password = new OpenApiOAuthFlow
                        {
                            TokenUrl = new Uri($"{_configuration["Authentication:Authority"]}/connect/token"),
                        },
                        ClientCredentials = new OpenApiOAuthFlow
                        {
                            TokenUrl = new Uri($"{_configuration["Authentication:Authority"]}/connect/token"),
                        }
                    }
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "oauth2"
                            },
                            Scheme = "oauth2",
                            Name = "oauth2",
                            In = ParameterLocation.Header
                        },
                        new List<string>()
                    }
                });

                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = @"JWT Authorization header using the Bearer scheme. \r\n\r\n 
                      Enter 'Bearer' [space] and then your token in the text input below.
                      \r\n\r\nExample: 'Bearer 12345abcdef'",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "Bearer",
                            Name = "Bearer",
                            In = ParameterLocation.Header,

                        },
                        new List<string>()
                    }
                });
        });

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
            app.UseSwagger(options => options.RouteTemplate = "api-docs/{documentName}/swagger.json");
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/api-docs/v1/swagger.json", "v1");
                options.SwaggerEndpoint("/api-docs/v2/swagger.json", "v2");
            });
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