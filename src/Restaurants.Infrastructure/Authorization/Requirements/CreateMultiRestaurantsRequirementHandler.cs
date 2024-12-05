using Microsoft.AspNetCore.Authorization;
using Restaurants.Application.Users;
using Restaurants.Domain.Repositories;

namespace Restaurants.Infrastructure.Authorization.Requirements
{
    internal class CreatedMultiRestaurantsRequirementHandler(
        IUserContext userContext,
        IRestaurantsRepository restaurantsRepository
        ) : AuthorizationHandler<CreateMultiRestaurantsRequirement>
    {
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, CreateMultiRestaurantsRequirement requirement)
        {
            var currentUser = userContext.GetCurrentUser();

            var restaurants = await  restaurantsRepository.GetAllAsync();

            var userRestaurantsCreated = restaurants.Count(r => r.OwnerId == currentUser.Id);

            if( userRestaurantsCreated >= requirement.MinimumRestaurantsCreated)
            {
                context.Succeed(requirement);
            }
            else
            {
                context.Fail();
            }
                
        }
    }
}
