using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
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

        services.AddApiVersioning(o =>
        {
            o.AssumeDefaultVersionWhenUnspecified = false;
            o.DefaultApiVersion = new ApiVersion(1, 0);
            o.ReportApiVersions = true;
            o.ApiVersionReader = new UrlSegmentApiVersionReader();
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