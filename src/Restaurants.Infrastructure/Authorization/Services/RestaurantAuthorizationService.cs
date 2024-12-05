using Microsoft.Extensions.Logging;
using Restaurants.Application.Users;
using Restaurants.Domain.Constants;
using Restaurants.Domain.Entities;
using Restaurants.Domain.Interfaces;

namespace Restaurants.Infrastructure.Authorization.Services;

public class RestaurantAuthorizationService(

    ILogger<RestaurantAuthorizationService> logger,

    IUserContext userContext
    ) : IRestaurantAuthorizationService
{
    public bool Authorize(Restaurant restaurant, ResourceOperation resourceOperation)
    {
        var user = userContext.GetCurrentUser();

        logger.LogInformation(">>>> Authorizing User {UserEmail}, to {Operation} for restaurant {RestaurantName}",
            user.Email,

            resourceOperation,

            restaurant.Name
            );
        //custom logic to authorize user to a given restaurant in the context of a given operation

        if (resourceOperation == ResourceOperation.Read || resourceOperation == ResourceOperation.Create)
        {
            logger.LogInformation("Create/read operation - successful authorization");

            return true;
        }

        if (resourceOperation == ResourceOperation.Delete && user.IsInRole(UserRoles.Admin))
        {
            logger.LogInformation("admin user, delete operation - successful authorization");

            return true;
        }

        if ((resourceOperation == ResourceOperation.Update || resourceOperation == ResourceOperation.Delete)

            &&

            user.Id == restaurant.OwnerId

            )
        {
            logger.LogInformation("Restaurant owner -  successful authorization");

            return true;

        }

        return false;
    }
}
