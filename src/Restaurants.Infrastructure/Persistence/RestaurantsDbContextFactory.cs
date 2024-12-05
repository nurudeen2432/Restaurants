using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Restaurants.Infrastructure.Persistence;

internal class RestaurantsDbContextFactory : IDesignTimeDbContextFactory<RestaurantsDbContext>
{
    public RestaurantsDbContext CreateDbContext(string[] args)
    {
        // Build configuration
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.Development.json")
            .Build();

        // Get connection string
        var connectionString = configuration.GetConnectionString("RestaurantDb");

        // Configure DbContext options
        var optionsBuilder = new DbContextOptionsBuilder<RestaurantsDbContext>();
        optionsBuilder.UseSqlServer(connectionString);

        return new RestaurantsDbContext(optionsBuilder.Options);
    }
}