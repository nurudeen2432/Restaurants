using Restaurants.Domain.Entities;

namespace Restaurants.Domain.Repositories;

public interface IDishRepository
{
    Task<int> Create(Dish entity);

    Task Delete(IEnumerable<Dish> entities);
}
