using AutoMapper;
using FluentAssertions;
using Restaurants.Application.Restaurants.Commands.CreateRestaurant;
using Restaurants.Application.Restaurants.Commands.UpdateRestaurant;
using Restaurants.Domain.Entities;
using Xunit;
namespace Restaurants.Application.Restaurants.Dtos.Tests; // File scoped namespace


/*
 In this class, we would like to somehow create a mapper
 that invokes the map method to assert whether the properties after mapping are exactly as we are 
expecting to.
 */
public class RestaurantsProfileTests
{
    private IMapper _mapper;
    public RestaurantsProfileTests()
    {
        var configuration = new MapperConfiguration(cfg =>
        {
            //To be able to define which profile we will like to use for this configuration 
            //We will have to use a specific syntax in which we will pass as an action 
            //for the Imapper configuration expression to its constructor

            cfg.AddProfile<RestaurantsProfile>();


        });

        //to get the mapper we now invoke configuration

        _mapper = configuration.CreateMapper();

    }


    [Fact()]
    public void CreateMap_ForRestaurantToRestaurantDto_MapsCorrectly()
    {
        //arrange

        var restaurants = new Restaurant() //creating a new instance of the class restaurant
        {
            Id = new Guid(),
            Name = "Test restaurants",
            Description = "Test Description",
            Category = "Test category",
            HasDelivery = true,
            ContactEmail = "test@example.com",
            ContactNumber="123345836",
            Address= new Address

            {
                City = "Test City",
                Street= "Test street",
                PostalCode = "12344"
            }

        }; //Restaurant entity in the Domain Layer

        //act 

        var restaurantDto = _mapper.Map<RestaurantDto>(restaurants);

        //assertions

        restaurantDto.Should().NotBeNull(); //From fluent assertion extension

        restaurantDto.Id.Should().Be(restaurants.Id);
        restaurantDto.Name.Should().Be(restaurants.Name);
        restaurantDto.Description.Should().Be(restaurants.Description);
        restaurantDto.Category.Should().Be(restaurants.Category);
        restaurantDto.HasDelivery.Should().Be(restaurants.HasDelivery);
        restaurantDto.City.Should().Be(restaurants.Address.City);
        restaurantDto.Street.Should().Be(restaurants.Address.Street);
        restaurantDto.PostalCode.Should().Be(restaurants.Address?.PostalCode);  

    }


    [Fact()]
    public void CreateMap_ForCreateRestaurantCommandToRestaurant_MapsCorrectly()
    {
        //arrange
 //We now have the mapper interface which we can use for mapping purposes.

        var command = new CreateRestaurantCommand() 
        {
            
            Name = "Test restaurants",
            Description = "Test Description",
            Category = "Test category",
            HasDelivery = true,
            ContactEmail = "test@example.com",
            ContactNumber = "123345836",
            City = "Test City",
            Street = "Test street",
            PostalCode = "12344"




        }; //Restaurant entity in the Domain Layer

        //act 

        var restaurant = _mapper.Map<Restaurant>(command);


        //assertions
  
        restaurant.Name.Should().Be(command.Name);
        restaurant.Description.Should().Be(command.Description);
        restaurant.Category.Should().Be(command.Category);
        restaurant.HasDelivery.Should().Be(command.HasDelivery);
        restaurant.Address?.City.Should().Be(command.City);
        restaurant.Address.Should().NotBeNull();        //From fluent assertion extension
        restaurant.Address?.Street.Should().Be(command.Street);
        restaurant.Address?.PostalCode.Should().Be(command.PostalCode);
        restaurant.ContactEmail.Should().Be(command.ContactEmail);
        restaurant.ContactNumber.Should().Be(command.ContactNumber);
        
        









    }




    [Fact()]
    public void CreateMap_ForUpdateRestaurantCommandToRestaurant_MapsCorrectly()
    {
        //arrange
        //We now have the mapper interface which we can use for mapping purposes.

        var command = new UpdateRestaurantCommand()
        {
            Id = new Guid(),
            Name = "Update restaurants",
            Description = "Update Description",
            HasDelivery = true,





        }; //Restaurant entity in the Domain Layer

        //act 

        var restaurant = _mapper.Map<Restaurant>(command);


        //assertions

        restaurant.Should().NotBeNull();
        restaurant.Id.Should().Be(command.Id);
        restaurant.Name.Should().Be(command.Name);
        restaurant.Description.Should().Be(command.Description);
        restaurant.HasDelivery.Should().Be(command.HasDelivery);











    }


}