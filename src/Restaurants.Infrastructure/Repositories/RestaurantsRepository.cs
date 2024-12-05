using Microsoft.EntityFrameworkCore;
using Restaurants.Domain.Constants;
using Restaurants.Domain.Entities;
using Restaurants.Domain.Repositories;
using Restaurants.Infrastructure.Persistence;
using System.Linq.Expressions;

namespace Restaurants.Infrastructure.Repositories;

internal class RestaurantsRepository(RestaurantsDbContext dbContext): IRestaurantsRepository
{
    public async Task<Guid> Create(Restaurant entity)
    {
        dbContext.Restaurants.Add(entity);
        await dbContext.SaveChangesAsync();
        return entity.Id;
    }

    public async Task Delete(Restaurant entity)
    {
        dbContext.Remove(entity);

      await  dbContext.SaveChangesAsync();
    }

    public async Task<IEnumerable<Restaurant>> GetAllAsync()
    {
        var restaurants = await dbContext.Restaurants.ToListAsync();

        return restaurants;

    }

    public async Task<(IEnumerable<Restaurant>, int)> GetAllMatchingAsync(string? searchPhrase, 
        int PageSize, int PageNumber, string? sortBy, SortDirection sortDirection   )
    {
        var searchPhraseLower = searchPhrase?.ToLower();


        var baseQuery = dbContext
                            .Restaurants!
                            .Where(r => searchPhraseLower == null || (r.Name.Contains(searchPhraseLower)
            || r.Description.Contains(searchPhraseLower)));

        var totalCount = await baseQuery.CountAsync();

        if (sortBy != null)
        {
            //type for the EF parameter expression of func of a restaurant, the
            //pass the object as the property we will like to sort on
            var columnsSelector = new Dictionary<string, Expression<Func<Restaurant, object>>>
            {
                {nameof(Restaurant.Name), r => r.Name },
                {nameof(Restaurant.Description), r => r.Description },
                {nameof(Restaurant.Category), r => r.Category }


            };

            var selectedColumn = columnsSelector[sortBy];
                
            baseQuery = sortDirection == SortDirection.Ascending ?
                
                baseQuery.OrderBy(selectedColumn) : baseQuery.OrderByDescending(selectedColumn); 

        }

        //Total count based on the current page size and number

        var restaurants = await baseQuery
            .Skip(PageSize * (PageNumber - 1))
            .Take(PageSize)
            .ToListAsync();


        return (restaurants, totalCount);

    }


    public async Task <Restaurant?> GetByIdAsync(Guid id)
    {
        var restaurant = await dbContext.Restaurants
            .Include(r => r.Dishes)
            .FirstOrDefaultAsync(x => x.Id == id);

        return restaurant;
    }

    public Task SaveChanges()
    
       => dbContext.SaveChangesAsync ();
    
}
