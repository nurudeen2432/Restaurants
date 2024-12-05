
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Restaurants.Application.Common;
using Restaurants.Application.Restaurants.Dtos;
using Restaurants.Domain.Repositories;

namespace Restaurants.Application.Restaurants.Queries.GetAllRestaurants;

public class GetAllRestaurantsQueryHandler(ILogger<GetAllRestaurantsQueryHandler> logger, IMapper mapper, IRestaurantsRepository restaurantsRepository) 
    : IRequestHandler<GetAllRestaurantQuery, PagedResult<RestaurantDto>>
{
    public async Task<PagedResult<RestaurantDto>> Handle(GetAllRestaurantQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation(" >>>>>>>> Getting all Restaurants");
        var searchPhraseLower = request.searchPhrase?.ToLower();
        /*
         This is a bad method of filtering the result as we are not executing the Where method directly
        on the IQueryable interface but rather on the already materialized restaurants directly in memory


         var restaurants = (await restaurantsRepository.GetAllAsync())
                            .Where(r => r.Name.ToLower().Contains(searchPhraseLower)
                            || r.Description.ToLower().Contains(searchPhraseLower)); 
         
         */
        var (restaurants, totalCount) = await restaurantsRepository.GetAllMatchingAsync(searchPhraseLower,
            request.PageSize,
            request.PageNumber,
            request.SortBy,
            request.SortDirection
            );
                  

        var restaurantsDto = mapper.Map<IEnumerable<RestaurantDto>>(restaurants);

        var result = new PagedResult<RestaurantDto>(restaurantsDto, totalCount, request.PageSize, request.PageNumber);



        return result;
    }
}
