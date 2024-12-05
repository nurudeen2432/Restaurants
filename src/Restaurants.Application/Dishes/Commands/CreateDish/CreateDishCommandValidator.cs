using FluentValidation;

namespace Restaurants.Application.Dishes.Commands.CreateDish;


public class CreateDishCommandValidator: AbstractValidator<CreateDishCommand>
{
    public CreateDishCommandValidator()
    {
        RuleFor(dish => dish.Price)
            .GreaterThanOrEqualTo(0)
            .WithMessage("The price must be a non-negative number");
        RuleFor(dish => dish.KiloCalories)
            .GreaterThanOrEqualTo(0)
            .WithMessage("The calories must be a non-negative number");


    }
}
