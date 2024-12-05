using Restaurants.API.Extensions;
using Restaurants.API.Middlewares;
using Restaurants.Application.Extensions;
using Restaurants.Domain.Entities;
using Restaurants.Infrastructure.Extensions;
using Restaurants.Infrastructure.Seeders;
using Serilog;

try
{
    // Configure Serilog early
    Log.Logger = new LoggerConfiguration()
        .WriteTo.Console()
        .WriteTo.File("logs/startup-log.txt", rollingInterval: RollingInterval.Day)
        .CreateLogger();

    var builder = WebApplication.CreateBuilder(args);

    // Detailed configuration logging
    Log.Information($"Content Root Path: {builder.Environment.ContentRootPath}");
    Log.Information($"Environment Name: {builder.Environment.EnvironmentName}");

    // Load configuration with detailed logging
    builder.Configuration
        .SetBasePath(builder.Environment.ContentRootPath)
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
        .AddEnvironmentVariables();

    // Extensive configuration logging
    Log.Information("Configuration Sources:");
    foreach (var source in builder.Configuration.Sources)
    {
        Log.Information($"- {source.GetType().Name}");
    }

    // Log all configuration keys and values
    Log.Information("Configuration Values:");
    foreach (var configItem in builder.Configuration.AsEnumerable())
    {
        Log.Information($"- {configItem.Key}: {configItem.Value}");
    }

    // Detailed connection string logging
    var connectionString = builder.Configuration.GetConnectionString("RestaurantDb");
    Log.Information($"Connection String: {connectionString}");

    // Add services to the container
    builder.AddPresentation();
    builder.Services.AddApplication();

    // Add detailed exception handling to infrastructure registration
    try
    {
        builder.Services.AddInfrastructure(builder.Configuration);
    }
    catch (Exception infraEx)
    {
        Log.Error(infraEx, "Error during infrastructure service registration");
        throw;
    }

    var app = builder.Build();

    // Detailed seeding with error handling
    using (var scope = app.Services.CreateScope())
    {
        try
        {
            var seeder = scope.ServiceProvider.GetRequiredService<IRestaurantSeeder>();
            await seeder.Seed();
        }
        catch (Exception seedEx)
        {
            Log.Error(seedEx, "Error during database seeding");
            throw;
        }
    }

    // Rest of your configuration
    app.UseMiddleware<ErrorHandlingMiddleWare>();
    app.UseMiddleware<RequestTimeLoggingMiddleware>();
    app.UseSerilogRequestLogging();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();
    app.MapGroup("api/identity")
        .WithTags("Identity")
        .MapIdentityApi<User>();
    app.UseAuthorization();
    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application startup failed with a critical error");
    // Ensure log is written before application exits
    Log.CloseAndFlush();
    // Rethrow to ensure the application fails visibly
    throw;
}
finally
{
    Log.CloseAndFlush();
}

// Uncomment if needed for integration testing
public partial class Program { }