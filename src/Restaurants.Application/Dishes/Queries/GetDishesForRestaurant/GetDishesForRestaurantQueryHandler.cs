using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Restaurants.Application.Dishes.Dtos;
using Restaurants.Domain.Entities;
using Restaurants.Domain.Exceptions;
using Restaurants.Domain.Repositories;

namespace Restaurants.Application.Dishes.Queries.GetDishesForRestaurant;

public class GetDishesForRestaurantQueryHandler(
    
    ILogger<GetDishesForRestaurantQueryHandler> logger,

    IRestaurantsRepository restaurantsRepository,

    IMapper mapper

    
    ) : IRequestHandler<GetDishesForRestaurantQuery, IEnumerable<DishDto>>
{
    public async Task<IEnumerable<DishDto>> Handle(GetDishesForRestaurantQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation(">>>> Retrieving Dishes for restaurant with:  {RestaurantId}", request.RestaurantId);

        var restaurant = await restaurantsRepository.GetByIdAsync( request.RestaurantId );

        if (restaurant == null) throw new NotFoundExceptions(nameof(Restaurant), request.RestaurantId.ToString() );

        var result = mapper.Map<IEnumerable<DishDto>>(restaurant.Dishes);


        return result;
    }
}
