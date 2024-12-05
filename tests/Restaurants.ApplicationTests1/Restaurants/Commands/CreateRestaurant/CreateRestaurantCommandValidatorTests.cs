using FluentValidation.TestHelper;
using Xunit;


namespace Restaurants.Application.Restaurants.Commands.CreateRestaurant.Tests;

public class CreateRestaurantCommandValidatorTests
{
    [Fact()]
    public void Validator_ForValidCommand_ShouldNotHaveValidationError()
    {
        //arrange

        //the part we arrange and initialize our object

        var command = new CreateRestaurantCommand()
        {
            Name = "Test",
            Category = "Italian",
            ContactEmail = "test@test.com",
            Description = "New Foodie for pastries",
            PostalCode = "12-345"
        };

        var validator = new CreateRestaurantCommandValidator();


        //act

        //the action we performed section


        var result = validator.TestValidate(command);

        //assert

        // the part to confirm if the rest result was successful

        result.ShouldNotHaveAnyValidationErrors();//This typically means that there are no errors in the context of the validation of any of those properties



    }
    //Negative scenario to create a command that should fail the validation with some specific validation error

    [Fact()]
    public void Validator_ForInValidCommand_ShouldHaveValidationError()
    {
        //arrange

        //the part we arrange and initialize our object

        var command = new CreateRestaurantCommand()
        {
            Name = "Te",
            Category = "Ita",
            ContactEmail = "@test.com",
            Description = "New Foodie for pastries",
            PostalCode = "12345"
        };

        var validator = new CreateRestaurantCommandValidator();


        //act

        //the action we performed section


        var result = validator.TestValidate(command);

        //assert

        // the part to confirm if the rest result was successful

        result.ShouldHaveValidationErrorFor(c => c.Name);
        result.ShouldHaveValidationErrorFor(c => c.Category);
        result.ShouldHaveValidationErrorFor(c => c.ContactEmail);
        result.ShouldHaveValidationErrorFor(c => c.PostalCode);
        //This typically means that there are no errors in the context of the validation of any of those properties



    }

    [Theory()]
    [InlineData("French")]
    [InlineData("Chinese")]
    [InlineData("Indian")]
    [InlineData("Italian")]

    public void Validator_ForValidCategory_ShouldNotHaveValidationErrorsForCategoryProperty(string category)
    {

        //arrange

        var validator = new CreateRestaurantCommandValidator();

        var command = new CreateRestaurantCommand { Category = category };


        //act

        var result = validator.TestValidate(command);


        //assertion

        result.ShouldNotHaveValidationErrorFor(c => c.Category);

    }// as we are passing the inline data we need to also pass it as parameter

    [Theory()]
    [InlineData("1002-00")]
    [InlineData("1003-00")]
    [InlineData("1004-00")]

    public void Validator_ForInvalidPostalCode_ShouldHaveFailedValidationErrorsForPostalCodeProperties(string postalCode)
    {
        //arrange

        var validator = new CreateRestaurantCommandValidator();

        var command = new CreateRestaurantCommand { PostalCode = postalCode };

        //act

        var result = validator.TestValidate(command);

        //assertion

        result.ShouldHaveValidationErrorFor(c => c.PostalCode);
    }





}