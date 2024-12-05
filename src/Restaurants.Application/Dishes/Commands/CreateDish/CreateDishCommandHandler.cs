

using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Restaurants.Domain.Constants;
using Restaurants.Domain.Entities;
using Restaurants.Domain.Exceptions;
using Restaurants.Domain.Interfaces;
using Restaurants.Domain.Repositories;

namespace Restaurants.Application.Dishes.Commands.CreateDish;

public class CreateDishCommandHandler(ILogger<CreateDishCommandHandler> logger,
    IRestaurantsRepository restaurantsRepository,
    IDishRepository dishesRepository,
    IMapper mapper,
    IRestaurantAuthorizationService restaurantAuthorizationService 

    ) : IRequestHandler<CreateDishCommand, int>
{
    public async Task<int> Handle(CreateDishCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation(">>>>>> Creating new Dish {@DishRequest}", request);

        //To create a new dish for a restaurant we need to check if the restaurant exist in our restaurant entity in the Database

        var restaurant = await restaurantsRepository.GetByIdAsync(request.RestaurantId);

        if (restaurant == null) throw new NotFoundExceptions(nameof(Restaurant), request.RestaurantId.ToString());


        if (!restaurantAuthorizationService.Authorize(restaurant, ResourceOperation.Update))

            throw new ForbidException();

        var dish = mapper.Map<Dish>(request);



       return  await dishesRepository.Create(dish);
    }
}
