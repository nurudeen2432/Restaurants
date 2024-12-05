using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Restaurants.Application.Restaurants.Dtos;
using Restaurants.Domain.Entities;
using Restaurants.Domain.Exceptions;
using Restaurants.Domain.Interfaces;
using Restaurants.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurants.Application.Restaurants.Queries.GetRestaurantById
{
    public class GetRestaurantByIdQueryHandler(
        ILogger<GetRestaurantByIdQueryHandler> logger, 
        IMapper mapper,
        IRestaurantsRepository restaurantsRepository,
        IBlobStorageService blobStorageService
        ) : IRequestHandler<GetRestaurantByIdQuery, RestaurantDto>
    {
     
       public async Task<RestaurantDto> Handle(GetRestaurantByIdQuery request, CancellationToken cancellationToken)
        {
            logger.LogInformation(">>>>>> Getting restaurants by Id {RestaurantId}", request.Id);

            var restaurant = await restaurantsRepository.GetByIdAsync(request.Id)
                ?? throw new NotFoundExceptions(nameof(Restaurant), request.Id.ToString());


            var restaurantsDto = mapper.Map<RestaurantDto>(restaurant);

          restaurantsDto.LogoSasUrl = blobStorageService.GetBlobSasUrl(restaurant.LogoUrl);

            //var restaurantsDto = RestaurantDto.FromEntity(restaurant);

            return restaurantsDto;
        }
    }
}
