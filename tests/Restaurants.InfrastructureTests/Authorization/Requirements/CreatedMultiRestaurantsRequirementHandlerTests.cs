using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Moq;
using Restaurants.Application.Users;
using Restaurants.Domain.Entities;
using Restaurants.Domain.Repositories;
using Xunit;


namespace Restaurants.Infrastructure.Authorization.Requirements.Tests;

public class CreatedMultiRestaurantsRequirementHandlerTests
{
    [Fact()]
    public async Task HandleRequirementAsync_UserHasCreatedMulitpleRestaurants_ShouldSuccedAsync()
    {
        var currentUser = new CurrentUser("39D311E2-6B5C-4352-8C41-08DD02A5D9OP", "test@pay.co", [], null, null);

        var userContextMock = new Mock<IUserContext>();

        userContextMock.Setup(m => m.GetCurrentUser()).Returns(currentUser);

        var restaurants = new List<Restaurant>()
        {
            new Restaurant()
            {
                OwnerId = currentUser.Id


            },

            new Restaurant()
            {
                OwnerId = currentUser.Id


            },

            new Restaurant()
            {
                OwnerId = "4e8340ac-6430-494f-b325-23f864fbbb78"


            }
        };

        var restaurantRepositoryMock = new Mock<IRestaurantsRepository>();

        restaurantRepositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(restaurants);

        var requirement = new CreateMultiRestaurantsRequirement(2);


        var handler = new CreatedMultiRestaurantsRequirementHandler(userContextMock.Object, restaurantRepositoryMock.Object);

        var context = new AuthorizationHandlerContext([requirement], null, null);

        //act 

        await handler.HandleAsync(context);

        //assert

        context.HasSucceeded.Should().BeTrue();
    }

    [Fact()]

    public async Task HandleRequirementAsync_UserHasNotCreatedMultipleRestaurants_ShouldFail()
    {
        var currentUser = new CurrentUser("39D311E2-6B5C-4352-8C41-08DD02A5D9OP", "test@pay.co", [], null, null);

        var userContextMock = new Mock<IUserContext>();

        userContextMock.Setup(m => m.GetCurrentUser()).Returns(currentUser);

        var restaurants = new List<Restaurant>()
        {
            new Restaurant()
            {
                OwnerId = currentUser.Id


            },

       

            new Restaurant()
            {
                OwnerId = "4e8340ac-6430-494f-b325-23f864fbbb78"


            }
        };

        var restaurantRepositoryMock = new Mock<IRestaurantsRepository>();

        restaurantRepositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(restaurants);

        var requirement = new CreateMultiRestaurantsRequirement(2);


        var handler = new CreatedMultiRestaurantsRequirementHandler(userContextMock.Object, restaurantRepositoryMock.Object);

        var context = new AuthorizationHandlerContext([requirement], null, null);

        //act 

        await handler.HandleAsync(context);

        //assert

        context.HasSucceeded.Should().BeFalse();
        context.HasFailed.Should().BeTrue();
    }
}