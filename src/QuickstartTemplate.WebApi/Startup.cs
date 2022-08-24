using System.Reflection;
using IdentityModel.AspNetCore.AccessTokenValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Internal;
using Microsoft.OpenApi.Models;
using Npgsql;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Prometheus;
using Prometheus.DotNetRuntime;
using QuickstartTemplate.ApplicationCore;
using QuickstartTemplate.ApplicationCore.Resources;
using QuickstartTemplate.Infrastructure;
using QuickstartTemplate.Infrastructure.Common;
using QuickstartTemplate.Infrastructure.DbContexts;
using Serilog;
using Serilog.HttpClient;
using StackExchange.Redis;

namespace QuickstartTemplate.WebApi;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        _configuration = configuration;

        _connectionMultiplexer = !string.IsNullOrEmpty(_configuration.GetConnectionString("Redis"))
            ? ConnectionMultiplexer.Connect(_configuration.GetConnectionString("Redis"))
            : null;
    }

    private readonly IConfiguration _configuration;

    private readonly IConnectionMultiplexer? _connectionMultiplexer;

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddLocalization();

        services.AddControllers()
            .AddDataAnnotationsLocalization(options =>
            {
                options.DataAnnotationLocalizerProvider = (type, factory) =>
                    factory.Create(typeof(SharedResource));
            });

        services.AddCors(crosOption =>
        {
            crosOption.AddDefaultPolicy(
                builder => builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .SetPreflightMaxAge(TimeSpan.FromMinutes(10)));
        });

        services.AddApiVersioning(o =>
        {
            o.AssumeDefaultVersionWhenUnspecified = false;
            o.DefaultApiVersion = new ApiVersion(1, 0);
            o.ReportApiVersions = true;
            o.ApiVersionReader = new UrlSegmentApiVersionReader();
        });

        if (_connectionMultiplexer is null)
            services.AddDistributedMemoryCache();
        else
            services.AddStackExchangeRedisCache(options =>
                options.ConnectionMultiplexerFactory = async () => _connectionMultiplexer);

        services.AddAuthentication("Bearer")
            // JWT tokens (default scheme)
            .AddJwtBearer("Bearer", options =>
            {
                options.Audience = _configuration["Authentication:Authority"];
                options.Authority = _configuration["Authentication:ApiName"];
                options.MapInboundClaims = false;
                options.TokenValidationParameters.NameClaimType = "sub"; // user id accessible by HttpContext.User.Identity.Name
                options.SaveToken = true;
                // if token does not contain a dot, it is a reference token
                options.ForwardDefaultSelector = Selector.ForwardReferenceToken("Introspection");
            })

            // reference tokens
            .AddOAuth2Introspection("Introspection", options =>
            {
                options.Authority = _configuration["Authentication:Authority"];
                options.ClientId = _configuration["Authentication:ApiName"];
                options.ClientSecret = _configuration["Authentication:ApiSecret"];
                options.NameClaimType = "sub"; // user id accessible by HttpContext.User.Identity.Name
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

        services.AddHttpContextAccessor();
        services.AddSingleton<ISystemClock, SystemClock>();

        services.AddSingleton<IHttpMessageHandlerBuilderFilter, GlobalHttpMessageHandlerBuilderFilter>();

        // added handlers to this client will apply to all clients 
        services.AddHttpClient(GlobalHttpMessageHandlerBuilderFilter.GlobalMessageHandlerConfigure)
            //Collect metrics for all HttpClient instances created using IHttpClientFactory.
            //https://github.com/prometheus-net/prometheus-net#ihttpclientfactory-metrics
            .UseHttpClientMetrics()
            .LogRequestResponse(options => _configuration.Bind("LogRequestResponse", options));

        services.AddInfrastructure(_configuration);
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

            //Locate the XML file being generated by ASP.NET...
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            //... and tell Swagger to use those XML comments.
            options.IncludeXmlComments(xmlPath);

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
                Description = "JWT Authorization header using the Bearer scheme. Example: 'Bearer 12345abcdef'",
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

        services.AddOpenTelemetryTracing(builder =>
        {
            builder.AddSource(typeof(Startup).Assembly.FullName);
            builder.AddSource(typeof(ApplicationCoreSetup).Assembly.FullName);
            builder.AddSource(typeof(InfrastructureSetup).Assembly.FullName);
            //environment set docs
            //https://github.com/open-telemetry/opentelemetry-dotnet/blob/main/docs/trace/extending-the-sdk/README.md#resource-detector
            builder.SetResourceBuilder(ResourceBuilder.CreateDefault().AddEnvironmentVariableDetector());
            //environment set docs
            //https://github.com/open-telemetry/opentelemetry-dotnet/blob/main/src/OpenTelemetry.Exporter.Jaeger/README.md#environment-variables
            builder.AddJaegerExporter();
            builder.AddAspNetCoreInstrumentation();
            builder.AddElasticsearchClientInstrumentation();
            builder.AddHttpClientInstrumentation();
            builder.AddEntityFrameworkCoreInstrumentation(options => options.SetDbStatementForText = true);
            builder.AddNpgsql();

            if (_connectionMultiplexer is not null)
                builder.AddRedisInstrumentation(_connectionMultiplexer, options => options.SetVerboseDatabaseStatements = true);
        });

#if !DEBUG //https://github.com/djluck/prometheus-net.DotNetRuntime/issues/34
        var dotNetRuntimeStats = DotNetRuntimeStatsBuilder.Customize()
            .WithThreadPoolStats()
            .WithContentionStats()
            .WithGcStats()
            .WithJitStats()
            .WithExceptionStats()
            .WithErrorHandler(ex => Console.WriteLine("ERROR on per: " + ex))
            .StartCollecting();
        
        //https://github.com/prometheus-net/prometheus-net#eventcounter-integration
        // Collect below metrics and more 
        //https://www.npgsql.org/doc/diagnostics/metrics.html
        //https://docs.microsoft.com/en-us/ef/core/logging-events-diagnostics/event-counters?tabs=windows
        //https://docs.microsoft.com/en-us/dotnet/core/diagnostics/available-counters
        var eventCounter = EventCounterAdapter.StartListening();

        //https://github.com/prometheus-net/prometheus-net#net-6-meters-integration
        var meter = MeterAdapter.StartListening();
#endif

        services.AddHealthChecks()
            .AddDbContextCheck<ProjectDbContext>();
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
        app.UseCors();

        app.UseRouting();

        app.UseSerilogRequestLogging();

        //HTTP request metrics
        //https://github.com/prometheus-net/prometheus-net#aspnet-core-http-request-metrics
        app.UseHttpMetrics();

        //gRPC request metrics
        //https://github.com/prometheus-net/prometheus-net#aspnet-core-grpc-request-metrics
        app.UseGrpcMetrics();

        //https://josef.codes/asp-net-core-6-http-logging-log-requests-responses/
        app.UseHttpLogging();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapMetrics("/metrics");

        app.MapHealthChecks("/v1/health-check");

        app.MapControllers();
    }
}
