
using MediatR;
using Restaurants.Application.Restaurants.Dtos;
using System.Security.Cryptography.X509Certificates;

namespace Restaurants.Application.Restaurants.Queries.GetRestaurantById;

public class GetRestaurantByIdQuery(Guid id): IRequest<RestaurantDto>
{
 
       public Guid Id { get; } = id;
     

        
    
    
}
