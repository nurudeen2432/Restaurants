using AutoMapper;
using Castle.Core.Logging;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Restaurants.Domain.Constants;
using Restaurants.Domain.Entities;
using Restaurants.Domain.Exceptions;
using Restaurants.Domain.Interfaces;
using Restaurants.Domain.Repositories;
using System.Xml.Serialization;
using Xunit;
using Assert = Xunit.Assert;



namespace Restaurants.Application.Restaurants.Commands.UpdateRestaurant.Tests;

public class UpdateRestaurantCommandHandlerTests
{
    private readonly Mock<ILogger<UpdateRestaurantCommandHandler>> _loggerMock;
    private readonly Mock<IRestaurantsRepository> _restaurantsRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IRestaurantAuthorizationService> _restaurantauthorizationServiceMock;

    private readonly UpdateRestaurantCommandHandler _handler;

    public UpdateRestaurantCommandHandlerTests()
    {
        _loggerMock = new Mock<ILogger<UpdateRestaurantCommandHandler>>();
        _restaurantsRepositoryMock = new Mock<IRestaurantsRepository>();
        _mapperMock = new Mock<IMapper>();
        _restaurantauthorizationServiceMock = new Mock<IRestaurantAuthorizationService>();

        //created a handler with references to the Mock Object
        _handler = new UpdateRestaurantCommandHandler(
            _loggerMock.Object,
            _restaurantsRepositoryMock.Object,
            _mapperMock.Object,
            _restaurantauthorizationServiceMock.Object
            );




    }


    [Fact()]
    public async Task Handle_WithValidRequest_ShouldpdateRestaurants()
    {
        //arrange
        var restaurantId = Guid.NewGuid();

        var command = new UpdateRestaurantCommand()
        {
            Id = restaurantId,
            Name = "Update Test",
            Description = "Update Description",
            HasDelivery = true
        };
        //needed to actually set up the interfaces used in our command handlers

        var restaurant = new Restaurant()
        {
            Id = restaurantId,
            Name = "Test",
            Description = "Test for all"
        };

         _restaurantsRepositoryMock.Setup(r => r.GetByIdAsync(restaurantId)).ReturnsAsync(restaurant);

        _restaurantauthorizationServiceMock.Setup(m => m.Authorize(restaurant, Domain.Constants.ResourceOperation.Update))
                                            .Returns(true);

        //act

        await _handler.Handle(command, CancellationToken.None);

        //assert

        _restaurantsRepositoryMock.Verify(r => r.SaveChanges(), Times.Once);
        //verify the saveChanges has been invoked once on our restaurant repository
        _mapperMock.Verify(m => m.Map(command, restaurant), Times.Once);

    }

    [Fact()]

    public async Task Handle_WithNonExistingRestaurant_ShouldThrowNotFoundException()
    {
        //arrange
        // var restaurantId = Guid.Parse("cb83ecf2-8bb2-4105-8ed8-28ea6e8827b8");

        var restaurantId = new Guid("cb83ecf2-8bb2-4105-8ed8-28ea6e8827b8");


        //var restaurantId = Guid.Parse("cb83ecf2-8bb2-4105-8ed8-28ea6e8827b8");


        var request = new UpdateRestaurantCommand
        {
            Id = restaurantId

        };

        _restaurantsRepositoryMock.Setup(r => r.GetByIdAsync(restaurantId)).ReturnsAsync((Restaurant?)null);



        //act
        /*In order to properly invoke the async method
         we'll have to delegate of a type func of a task
         */
        Func<Task> act = async () => await _handler.Handle(request, CancellationToken.None);

        //assertions

        await act.Should().ThrowAsync<NotFoundExceptions>()
                    .WithMessage($"Restaurant with id: {restaurantId} doesn't exist".TrimEnd());




    }


    [Fact()]

    public async Task Handle_WithUnauthorizedUser_ShouldThrowForbidException()
    {
        // Arrange

        var restaurantId = new Guid("39D311E2-6B5C-4352-8C41-08DD02A5D9BF");

        var request = new UpdateRestaurantCommand
        {
            Id = restaurantId
        };

        var existingRestaurant = new Restaurant
        {
            Id = restaurantId
        };

        _restaurantsRepositoryMock.Setup(r => r.GetByIdAsync(restaurantId)).ReturnsAsync(existingRestaurant);

        _restaurantauthorizationServiceMock.Setup(a => a.Authorize(existingRestaurant, ResourceOperation.Update)).Returns(false);


        //act

        Func<Task> act = async () => await _handler.Handle(request, CancellationToken.None);


        //assert

        await act.Should().ThrowAsync<ForbidException>();

    }
}