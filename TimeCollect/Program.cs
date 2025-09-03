// TimeCollect/Program.cs

using System.CodeDom;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using TimeCollect.Core.Models;
using TimeCollect.Core.Services;

namespace TimeCollect;

/// <summary>
/// This is the main entry point of the application.
/// </summary>

var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((context, configuration) =>
    {
        // --- Configuration Setup ---
        configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
        configuration.AddEnvironmentVariables();
    })
    .ConfigureServices((context, services) =>
    {
       // --- Dependency Injection ---

       services.AddSingleton<GoogleApiService>();
       services.AddSingleton<DataCollectionService>();
       services.AddSingleton<DataTransformationService>();
       services.AddSingleton<ExcelExportService>();
       services.AddSingleton<DateService>();
    })
    .UseSerilog((context, loggerConfig) =>
    {
        // --- Logging Configuration ---

        loggerConfig
            .ReadFrom.Configuration(context.Configuration)
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .WriteTo.File("logs/main_app.log", rollingInterval: RollingInterval.Day);
    })
    .Build();


var logger = host.Services.GetRequiredService<ILogger>();
var config = host.Services.GetRequiredService<IConfiguration>();
var googleApiService = host.Services.GetRequiredService<GoogleApiService>();
var dataCollectionService = host.Services.GetRequiredService<DataCollectionService>();
var dataTransformationService = host.Services.GetRequiredService<DataTransformationService>();
var excelExportService = host.Services.GetRequiredService<ExcelExportService>();
var dateService = host.Services.GetRequiredService<DateService>();

try
{
    logger.Information("--- TimeCollect 2.0 Starting ---");
    
    // --- Google API Connection ---
    var credentials = await googleApiService.GetUserCredentialAsync();
    if (credentials == null)
    {
        logger.Error("Failed to connect to Google API service. Exiting.");
        return;
    }
    logger.Information("🌐 Connected to Google API.");
    
    // --- Date and Project Data Setup ---
    var weekTypeSchedule = dateService.SetWeekTypes(2024, 12, 29);
    var rawProjectData = await dataCollectionService.GetDataAsync(
        credentials,
        config["GoogleSheets:ProjectSpreadsheetId"],
        config["GoogleSheets:ProjectRange"]
    );
    
    
    
    
}
catch (Exception ex)
{
    logger.Fatal(ex.ToString());
}
finally
{
    logger.Information("--- TimeCollect 2.0 Finished ---");
}

await host.RunAsync();
    