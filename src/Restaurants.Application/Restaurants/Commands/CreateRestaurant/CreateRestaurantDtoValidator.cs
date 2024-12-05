using FluentValidation;
using Restaurants.Application.Restaurants.Dtos;

namespace Restaurants.Application.Restaurants.Commands.CreateRestaurant;

public class CreateRestaurantCommandValidator : AbstractValidator<CreateRestaurantCommand>
{
    //This to create a custom validation on our category property
    private readonly List<string> validCategories = ["Italian", "Mexican", "Japanese", "American", "Indian", "Chinese", "French"];
    public CreateRestaurantCommandValidator()
    {
        RuleFor(dto => dto.Name)
            .Length(3, 100);
        RuleFor(dto => dto.Description)
            .NotEmpty().WithMessage("Description is require");

        RuleFor(dto => dto.Category)
            .Must(validCategories.Contains)
            .WithMessage("Choose from the valid categories")
            .Custom((value, context) =>
            {
                var isValidCategory = validCategories.Contains(value);

                if (!isValidCategory)
                {
                    context.AddFailure("Category", "Invalid Category, Please choose from the valid categories");
                }

            }
            );

        RuleFor(dto => dto.ContactEmail)
            .EmailAddress()
            .WithMessage("Please provide valid Email Address");

        RuleFor(dto => dto.PostalCode)
            .Matches(@"^\d{2}-\d{3}$")
            .WithMessage("Please provide a valid Postal code in the format (XX-XXX)");
    }

}
