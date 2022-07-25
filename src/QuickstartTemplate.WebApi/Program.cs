using QuickstartTemplate.WebApi;
using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);

// Manually create an instance of the Startup class
// https://andrewlock.net/exploring-dotnet-6-part-12-upgrading-a-dotnet-5-startup-based-app-to-dotnet-6/
var startup = new Startup(builder.Configuration);

builder.Host.UseSerilog((context, configuration) =>
{
    if (!Enum.TryParse(context.Configuration["LogEventLevel"], out LogEventLevel logEventLevel))
        logEventLevel = LogEventLevel.Information;

    configuration.Filter.ByExcluding(logEvent =>
        logEvent.Exception != null && logEvent.Exception.GetType() == typeof(OperationCanceledException));

    configuration.MinimumLevel.Is(logEventLevel);

    configuration.Enrich.FromLogContext().WriteTo.Console();
});

// Manually call ConfigureServices()
startup.ConfigureServices(builder.Services);

var app = builder.Build();

// Manually call Configure()
startup.Configure(app);

app.Run();