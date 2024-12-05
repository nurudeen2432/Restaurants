using AutoMapper;
using Castle.Core.Logging;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Restaurants.Application.Users;
using Restaurants.Domain.Constants;
using Restaurants.Domain.Entities;
using Restaurants.Domain.Repositories;
using Xunit;


namespace Restaurants.Application.Restaurants.Commands.CreateRestaurant.Tests;

public class CreateRestaurantCommandHandlerTests
{
    [Fact()]
    public async Task Handle_ForValidCommand_ReturnsCreatedRestaurantId()
    {
        //arrange

        var loggerMock = new Mock<ILogger<CreateRestaurantCommandHandler>>();

        var mapperMock = new Mock<IMapper>();

        var command = new CreateRestaurantCommand();

        var restaurant = new Restaurant();

        mapperMock.Setup(m => m.Map<Restaurant>(command)).Returns(restaurant);

        var restaurantRepositoryMock = new Mock<IRestaurantsRepository>();

        restaurantRepositoryMock.Setup(repo => repo.Create(It.IsAny<Restaurant>()))
                                .ReturnsAsync(new Guid());

        var userContext = new Mock<IUserContext>();

        var currentUser = new CurrentUser("owner-id", "test@test.com", [], null, null);

        userContext.Setup(u => u.GetCurrentUser()).Returns(currentUser);

        var commandHandler = new CreateRestaurantCommandHandler(loggerMock.Object,
            mapperMock.Object,
            restaurantRepositoryMock.Object,
            userContext.Object
            );
           

        //act

        var result = await commandHandler.Handle(command, CancellationToken.None);

        //assertion

        result.Should().Be(new Guid());

        restaurant.OwnerId.Should().Be(currentUser.Id);

        //assert whether the create method has been invoked exactly one time with the restaurant entity as parameter

        restaurantRepositoryMock.Verify(r => r.Create(restaurant), Times.Once);



            
            
        

        

    }

}