using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Restaurants.Application.Dishes.Commands.CreateDish;
using Restaurants.Application.Dishes.Commands.DeleteDishes;
using Restaurants.Application.Dishes.Dtos;
using Restaurants.Application.Dishes.Queries.GetDishesForRestaurant;
using Restaurants.Application.Dishes.Queries.GetDishForRestaurantById;
using Restaurants.Infrastructure.Authorization;

namespace Restaurants.API.Controllers;

[Route("api/restaurant/{restaurantId}/dishes")]

[ApiController]
[Authorize]
public class DishesController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateDish([FromRoute]Guid restaurantId, CreateDishCommand command)
    {
        command.RestaurantId = restaurantId;

       var dishId =  await mediator.Send(command);

        return CreatedAtAction(nameof(GetDishForRestaurantById), new { restaurantId, dishId }, null);

    }


    [HttpGet]
    [Authorize(Policy = PolicyNames.Atleast20)]
    public async Task<ActionResult<IEnumerable<DishDto>>> GetAllDishesForRestaurant([FromRoute] Guid restaurantId)
    {
       var dishes = await mediator.Send(new GetDishesForRestaurantQuery(restaurantId));

        return Ok(dishes);
    }


    [HttpGet("{dishId}")]

    public async Task<ActionResult<DishDto>> GetDishForRestaurantById([FromRoute] Guid restaurantId, [FromRoute] int dishId)
    {
        var dish = await mediator.Send(new GetDishForRestaurantQueryById(restaurantId, dishId));

        return Ok(dish);
    }


    [HttpDelete]

    public async Task<IActionResult> DeleteDishesForRestaurant([FromRoute] Guid restaurantId)
    {
        await mediator.Send(new DeleteDishesForRestaurantCommand(restaurantId));

        return NoContent();
    }


}
