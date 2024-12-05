﻿using MediatR;
using Microsoft.Extensions.Logging;
using Restaurants.Domain;
using Restaurants.Domain.Entities;
using Restaurants.Domain.Exceptions;
using Restaurants.Domain.Interfaces;
using Restaurants.Domain.Repositories;
using Restaurants.Domain.Constants;

namespace Restaurants.Application.Restaurants.Commands.DeleteRestaurant;
public class DeleteRestaurantCommandHandler(
    ILogger<DeleteRestaurantCommandHandler> logger, 
    IRestaurantsRepository restaurantsRepository,
    IRestaurantAuthorizationService restaurantAuthorizationService
    
    ) : IRequestHandler<DeleteRestaurantCommand>
{
    public async Task Handle(DeleteRestaurantCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation(">>>>>>> Deleting Restaurant with Id: {@RestaurantId}", request.Id);

        var restaurant = await restaurantsRepository.GetByIdAsync( request.Id );

        if (restaurant is null)
            throw new NotFoundExceptions(nameof(Restaurant), request.Id.ToString());


        if (!restaurantAuthorizationService.Authorize(restaurant, ResourceOperation.Delete))

            throw new ForbidException();

       await restaurantsRepository.Delete( restaurant );

       
    }
}
