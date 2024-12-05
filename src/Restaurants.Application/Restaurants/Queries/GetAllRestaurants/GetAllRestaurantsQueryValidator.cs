using FluentValidation;
using Restaurants.Application.Restaurants.Dtos;

namespace Restaurants.Application.Restaurants.Queries.GetAllRestaurants;

public class GetAllRestaurantsQueryValidator : AbstractValidator<GetAllRestaurantQuery>
{
    private readonly int[] allowedPageSizes = [5, 10, 15, 20];

    private readonly string[] allowedSortByColumns = [
        nameof(RestaurantDto.Name),
        nameof(RestaurantDto.Description),
        nameof(RestaurantDto.Category)];

    public GetAllRestaurantsQueryValidator()
    {
        RuleFor(r => r.PageNumber)
            .GreaterThanOrEqualTo(1);

        RuleFor(r => r.PageSize)
            .Must(value => allowedPageSizes.Contains(value))
            .WithMessage($"Page size must be in a [{string.Join(", ", allowedPageSizes)}]");

        RuleFor(r => r.SortBy)
                .Must(value => allowedSortByColumns.Contains(value))
                .When(q => q.SortBy != null)
                .WithMessage($"Sort by is optional but must be in  [{string.Join(", ", allowedSortByColumns)}]");



    }
}
