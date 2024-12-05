using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.DependencyInjection;
using Restaurants.Application.Restaurants;
using Restaurants.Application.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurants.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddApplication(this IServiceCollection services)
    {

        var applicationAssembly = typeof(ServiceCollectionExtensions).Assembly;
        
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(applicationAssembly));

        services.AddAutoMapper(applicationAssembly);

        services.AddValidatorsFromAssembly(applicationAssembly)
                .AddFluentValidationAutoValidation();

        //we need to register the service GetCurrentUser in our DI container,
        //so that in any handler in our application module, we'll be able to actually
        //refer to the current user context and get the UserId or the User Email

        services.AddScoped<IUserContext , UserContext>();
        //in order to get the proper implementation of Ihttpcontext accessor object, I will invoke the method
        //to allow us inject to context accessor to our User context service class.

        services.AddHttpContextAccessor();
    }
}
