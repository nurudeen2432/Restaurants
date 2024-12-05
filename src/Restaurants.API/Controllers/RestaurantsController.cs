using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Restaurants.Application.Restaurants;
using Restaurants.Application.Restaurants.Commands.CreateRestaurant;
using Restaurants.Application.Restaurants.Commands.DeleteRestaurant;
using Restaurants.Application.Restaurants.Commands.UpdateRestaurant;
using Restaurants.Application.Restaurants.Commands.UploadRestaurantLogo;
using Restaurants.Application.Restaurants.Dtos;
using Restaurants.Application.Restaurants.Queries.GetAllRestaurants;
using Restaurants.Application.Restaurants.Queries.GetRestaurantById;
using Restaurants.Domain.Constants;
using Restaurants.Domain.Entities;
using Restaurants.Infrastructure.Authorization;
using System.Reflection.Metadata.Ecma335;
namespace Restaurants.API.Controllers;

[ApiController]
[Route("api/restaurants")]
[Authorize]
public class RestaurantsController(IMediator mediator): ControllerBase
{
    
    // Get All Request
    
    [HttpGet]
    //all endpoint requires authorization before access except this one
    [AllowAnonymous]
   // [Authorize(Policy = PolicyNames.CreatedAtleast2Restaurants)]
    //[ProducesResponseType(StatusCodes.Status200OK, Type =typeof(IEnumerable<RestaurantDto>))]
    public async Task<ActionResult<IEnumerable<RestaurantDto>>> GetAll([FromQuery] GetAllRestaurantQuery query)
    {
        var restaurants = await mediator.Send(query);
            return Ok(restaurants);
    }

    //Get By Id request
    [HttpGet("{id}")]
    //[Authorize(Policy= PolicyNames.HasNationality)]
    public async Task<ActionResult<IEnumerable<RestaurantDto?>>> GetById([FromRoute] Guid id)
    {
        //declare a lambda expression that will take the value of a claim that contains the information
        //about the userId, so I can distinguish the claims by their types.

        
        var restaurant = await mediator.Send(new GetRestaurantByIdQuery(id));
       

        if (restaurant == null) 
        
            return NotFound();
       
      return Ok(restaurant);
    }

    //Delete Request
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteRestaurant([FromRoute] Guid id)
    {
        await mediator.Send(new DeleteRestaurantCommand(id));


  

       return NoContent();

        
    }

//Update Request

    [HttpPatch("{id}")]
    [Authorize(Roles = UserRoles.Owner)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateRestaurant(
        [FromRoute] Guid id, 
        UpdateRestaurantCommand command,
        ILogger<UpdateRestaurantCommand> _logger
        
        )
    {
        command.Id = id;

        var restaurant = await mediator.Send(new GetRestaurantByIdQuery(id));

        if (restaurant == null)
        {
            _logger.LogWarning($"No restaurant found with Id: {id}");
            return NotFound();
        }

       await mediator.Send(command);


  

        return NotFound();
    }

    // Create Request
    [HttpPost]
    [Authorize(Roles = UserRoles.Owner)]
    public async Task<IActionResult> CreateRestaurant([FromBody]CreateRestaurantCommand command )
    {
       
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

      Guid id = await  mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id }, null);


    }


    /*This endpoint is to allow blob storage upload using the Iformfile*/

    [HttpPost("{id}/logo")]
    [Authorize(Roles = UserRoles.Owner)]
    public async Task<IActionResult> UploadLogo([FromRoute]Guid id, IFormFile file)
    {
        using var stream = file.OpenReadStream();

        var command = new UploadRestaurantLogoCommand()
        {
            RestaurantId = id,
            FileName = $"{id}-{file.FileName}",
            File = stream
        };
        await mediator.Send(command);

        return NoContent();



    }


}
