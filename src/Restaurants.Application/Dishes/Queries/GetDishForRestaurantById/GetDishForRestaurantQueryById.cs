using MediatR;
using Restaurants.Application.Dishes.Dtos;

namespace Restaurants.Application.Dishes.Queries.GetDishForRestaurantById;

public class GetDishForRestaurantQueryById(Guid restaurantId, int dishId): IRequest<DishDto>
{
    public Guid RestaurantId { get; } = restaurantId;

    public int DishId { get; } = dishId;

}
