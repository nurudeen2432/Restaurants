using Restaurants.Application.Restaurants.Dtos;

namespace Restaurants.Application.Common;

public class PagedResult<T>
{
    /*
     Container we would to return to our API client, containing 
    the information about the result and the properties
     */

    public PagedResult(
        IEnumerable<T> items,
        int totalCount,
        int pageSize,
        int pageNumber
        )
    {
        //initialize our collections with list of items coming through the constructor.
        this.Items = items;
        this.TotalItemsCount = totalCount;
        this.TotalPages = (int)Math.Ceiling( totalCount / (double)pageSize);
        this.ItemsFrom = pageSize * (pageNumber -1) + 1;
        this.ItemsTo = ItemsFrom + pageSize - 1;

        //page size = 5, pageNumber=2
        //skip: pageSize * (pageNumber - 1) => 5
        // itemsFrom: 5 + 1 => 6
        //itemsTo: 6 + 5 - 1 => 10


    }

    public IEnumerable<T> Items { get; set; }

    public int TotalPages { get; set; }

    public int TotalItemsCount { get; set; }

    public int ItemsFrom {  get; set; }

    public int ItemsTo { get; set; }
}
