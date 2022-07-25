using Microsoft.AspNetCore.HttpLogging;
using QuickstartTemplate.ApplicationCore;
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
        services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        services.AddInfrastructure();
        services.AddApplication();
        
        //https://josef.codes/asp-net-core-6-http-logging-log-requests-responses/
        services.AddHttpLogging(options =>
        {
            //dont log Response if grpc is added it will break; track bug in below issue
            //https://github.com/dotnet/aspnetcore/issues/39317

            HttpLoggingFields httpLoggingFields;
            //https://docs.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.httplogging.httploggingfields?view=aspnetcore-6.0
            if (!Enum.TryParse(_configuration["HttpLoggingFields"],
                    out httpLoggingFields))
                httpLoggingFields = HttpLoggingFields.None;
            options.LoggingFields = httpLoggingFields;
        });
    }

    public void Configure(WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        app.UseRouting();
        
        app.UseSerilogRequestLogging();
        
        //https://josef.codes/asp-net-core-6-http-logging-log-requests-responses/
        app.UseHttpLogging();
        
        app.UseAuthorization();

        app.MapControllers();
    }
}