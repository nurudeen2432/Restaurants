using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Restaurants.Application.Dishes.Dtos;
using Restaurants.Domain.Entities;
using Restaurants.Domain.Exceptions;
using Restaurants.Domain.Repositories;

namespace Restaurants.Application.Dishes.Queries.GetDishForRestaurantById;

public class GetDishForRestaurantQueryByIdHandler(
    ILogger<GetDishForRestaurantQueryByIdHandler> logger,
    IRestaurantsRepository restaurantsRepository,
    IMapper mapper
    
    ) : IRequestHandler<GetDishForRestaurantQueryById, DishDto>
{
    public async Task<DishDto> Handle(GetDishForRestaurantQueryById request, CancellationToken cancellationToken)
    {
        logger.LogInformation(">>> Retrieving dish: {DishId}, for Restaurant with Id: {RestaurantId}", request.DishId, request.RestaurantId);

        var restaurant = await restaurantsRepository.GetByIdAsync(request.RestaurantId);

        if (restaurant == null) throw new NotFoundExceptions(nameof(Restaurant), request.RestaurantId.ToString());

        var dish = restaurant.Dishes.FirstOrDefault(d => d.Id == request.DishId);

        if (dish == null) throw new NotFoundExceptions(nameof(Dish), request.DishId.ToString());

        var result = mapper.Map<DishDto>(dish);

        return result;

    }
}
