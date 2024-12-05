using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Restaurants.Domain.Entities;
using Restaurants.Domain.Interfaces;
using Restaurants.Domain.Repositories;
using Restaurants.Infrastructure.Authorization;
using Restaurants.Infrastructure.Authorization.Requirements;
using Restaurants.Infrastructure.Authorization.Services;
using Restaurants.Infrastructure.Configuration;
using Restaurants.Infrastructure.Persistence;
using Restaurants.Infrastructure.Repositories;
using Restaurants.Infrastructure.Seeders;
using Restaurants.Infrastructure.Storage;
namespace Restaurants.Infrastructure.Extensions;


public static class ServiceCollectionExtensions
{

    public static void AddInfrastructure(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("RestaurantDb");

        serviceCollection.AddDbContext<RestaurantsDbContext>(options => options
        .UseSqlServer(connectionString,
                     sqlServerOptionsAction: sqlOptions =>
                     {
                         sqlOptions.EnableRetryOnFailure(
                             maxRetryCount: 5,
                             maxRetryDelay: TimeSpan.FromSeconds(30),
                             errorNumbersToAdd: null);
                     }
        )
        .EnableSensitiveDataLogging()
        );

        //Asp.netcore Identity package is  going to register the EF stores 
        //which are basically a repositories for user roles and also some other types
        //AddIdentityApi Endpoint we are going to register some concrete classes that are necessary for the identity 
        //endpoints

        serviceCollection.AddIdentityApiEndpoints<User>()
                            .AddRoles<IdentityRole>()
                            .AddClaimsPrincipalFactory<RestaurantsUserClaimsPrincipalFactory>()
                         .AddEntityFrameworkStores<RestaurantsDbContext>();
                        

        serviceCollection.AddScoped<IRestaurantSeeder, RestaurantSeeder>();

        serviceCollection.AddScoped<IRestaurantsRepository, RestaurantsRepository>();

        serviceCollection.AddScoped<IDishRepository, DishesRepository>();

        serviceCollection.AddAuthorizationBuilder()
                         .AddPolicy(PolicyNames.HasNationality,
                         builder => builder.RequireClaim(AppClaimTypes.Nationality, "German", "Polish"))
                         .AddPolicy(PolicyNames.Atleast20,
                            builder => builder.AddRequirements(new MinimumAgeRequirement(20)))
                         .AddPolicy(PolicyNames.CreatedAtleast2Restaurants,
                         builder => builder.AddRequirements(new CreateMultiRestaurantsRequirement(2)));


        serviceCollection.AddScoped<IAuthorizationHandler, MinimumAgeRequirementHandler>();
        serviceCollection.AddScoped<IAuthorizationHandler, CreatedMultiRestaurantsRequirementHandler>();
                         
        serviceCollection.AddScoped<IRestaurantAuthorizationService, RestaurantAuthorizationService>();


        serviceCollection.Configure<BlobStorageSettings>(configuration.GetSection("BlobStorage"));

        serviceCollection.AddScoped<IBlobStorageService, BlobStorageService>();


    }

}
