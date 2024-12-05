using MediatR;
using Microsoft.Extensions.Logging;
using Restaurants.Domain.Entities;
using Restaurants.Domain.Exceptions;
using Restaurants.Domain.Interfaces;
using Restaurants.Domain.Repositories;

using Restaurants.Domain.Constants;

namespace Restaurants.Application.Restaurants.Commands.UploadRestaurantLogo

{
    public class UploadRestaurantLogoCommandHandler
        (
        ILogger<UploadRestaurantLogoCommandHandler> logger,
        IRestaurantsRepository _restaurantRepository,
        IRestaurantAuthorizationService restaurantAuthorizationService,
        IBlobStorageService blobStorageService

        ) : IRequestHandler<UploadRestaurantLogoCommand>
    {
        public async Task Handle(UploadRestaurantLogoCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation(">>>>>> Uploading restaurant logo for id: {RestaurantId}", request.RestaurantId);
            var restaurant = await _restaurantRepository.GetByIdAsync(request.RestaurantId);

            if (restaurant == null)
                throw new NotFoundExceptions(nameof(Restaurant), request.RestaurantId.ToString());


            if (!restaurantAuthorizationService.Authorize(restaurant, ResourceOperation.Update))
                throw new ForbidException();

            var logoUrl = await blobStorageService.UploadToBlobAsync(request.File, request.FileName);


            //We need to save the logo url inside of our restaurants

            restaurant.LogoUrl = logoUrl;

            await _restaurantRepository.SaveChanges();


        }
    }
}