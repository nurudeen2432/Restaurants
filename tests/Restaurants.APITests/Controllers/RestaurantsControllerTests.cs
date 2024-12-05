using FluentAssertions;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using Restaurants.APITests;
using Restaurants.Application.Restaurants.Dtos;
using Restaurants.Domain.Entities;
using Restaurants.Domain.Repositories;
using System.Net;
using System.Net.Http.Json;
using Xunit;


namespace Restaurants.API.Controllers.Tests;
/*
 in order to actually set up in-memory version of our API from
integrated test, we'll have to use the use the web application factory class from a dedicated package, which is MVC testing
 */
public class RestaurantsControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    private readonly Mock<IRestaurantsRepository> _restaurantsRepositoryMock = new();   //assign to a new object of the same type






    public RestaurantsControllerTests(WebApplicationFactory<Program> factory)

    {
        //_factory obj will allow us to actually create an Http client, with which then we'll be able to
        //send http request against our api in memory
        this._factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                services.AddSingleton<IPolicyEvaluator, FakePolicyEvaluator>();
                //using this as a replacement of the real version that is registered inside of our API

                //This is how we can replace the concrete restaurant repository that is registered within our API
                services.Replace(ServiceDescriptor.Scoped(typeof(IRestaurantsRepository), _ => _restaurantsRepositoryMock.Object));
            });
        });
    }

    [Fact()]

    public async Task GetById_ForNonExisitingId_ShoudldReturn404NotFound()
    {
        //arrange

        var id = Guid.Parse("4e8340ac-6430-494f-b325-23f864fbbc45");

        _restaurantsRepositoryMock.Setup(m => m.GetByIdAsync(id)).ReturnsAsync((Restaurant?)null);

        var client = _factory.CreateClient();


        //act

        var response = await client.GetAsync($"/api/restaurants/{id}");


        //assert

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

    }



    [Fact()]

    public async Task GetById_ForExisitingId_ShoudldReturn200Ok()
    {
        //arrange

        var id = Guid.Parse("5e8340ac-6430-494f-b325-23f864fbbc45");

        var restaurant = new Restaurant()
        {
            Id = id,
            Name = "Test",
            Description = "Test Description"
        };

        _restaurantsRepositoryMock.Setup(m => m.GetByIdAsync(id)).ReturnsAsync((restaurant));

        var client = _factory.CreateClient();


        //act

        var response = await client.GetAsync($"/api/restaurants/{id}");
        var restaurantDto = await response.Content.ReadFromJsonAsync<RestaurantDto>();


        //assert

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        restaurantDto.Should().NotBeNull();
        restaurantDto!.Name.Should().Be("Test");
        restaurantDto!.Description.Should().Be("Test Description");



    }


    [Fact()]
    public async Task GetAll_ForValidRequest_Returns200Ok()
    {
        var client = _factory.CreateClient();

        //act

        var result = await client.GetAsync("/api/restaurants?searchPhrase=Random&PageNumber=1&PageSize=10");


        //assert

        result.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
    }


    [Fact()]
    public async Task GetAll_ForInValidRequest_Returns400BadRequest()
    {
        var client = _factory.CreateClient();

        //act

        var result = await client.GetAsync("/api/restaurants");


        //assert

        result.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
    }
}