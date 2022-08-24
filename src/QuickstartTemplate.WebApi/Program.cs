using QuickstartTemplate.WebApi;
using Sentry;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
// NOTE: Default application configuration sources can be found at 
// https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/?view=aspnetcore-6.0#default-application-configuration-sources

builder.WebHost.UseSentry(sentryOptions => sentryOptions.AddExceptionFilterForType<OperationCanceledException>());

builder.Host.UseSerilog((context, configuration) => configuration.ReadFrom.Configuration(context.Configuration));

// Manually create an instance of the Startup class
// https://andrewlock.net/exploring-dotnet-6-part-12-upgrading-a-dotnet-5-startup-based-app-to-dotnet-6/
var startup = new Startup(builder.Configuration);

// Manually call ConfigureServices()
startup.ConfigureServices(builder.Services);

var app = builder.Build();

// Manually call Configure()
startup.Configure(app);

app.Run();


#pragma warning disable CA1050 // Declare types in namespaces
// ReSharper disable once ClassNeverInstantiated.Global
public partial class Program { } //needed for WebApplicationFactory on IntegrationTests
#pragma warning restore CA1050 // Declare types in namespaces
