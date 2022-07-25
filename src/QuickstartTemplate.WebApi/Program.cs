using QuickstartTemplate.WebApi;

var builder = WebApplication.CreateBuilder(args);

// Manually create an instance of the Startup class
// https://andrewlock.net/exploring-dotnet-6-part-12-upgrading-a-dotnet-5-startup-based-app-to-dotnet-6/
var startup = new Startup(builder.Configuration);

// Manually call ConfigureServices()
startup.ConfigureServices(builder.Services);

var app = builder.Build();

// Manually call Configure()
startup.Configure(app);

app.Run();