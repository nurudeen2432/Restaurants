using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using Restaurants.Domain.Constants;
using System.Security.Claims;
using Xunit;


namespace Restaurants.Application.Users.Tests;

public class UserContextTests
{
    [Fact()]
    public void GetCurrentUserTest_WithAuthenticatedUser_ShouldReturnCurrentUser()
    {
        //arrange
        var httpContextAccessorMock = new Mock<IHttpContextAccessor>();

        var dateOfBirth = new DateOnly(1990, 1, 1);

        var claims = new List<Claim>()
        {
            new(ClaimTypes.NameIdentifier, "1"),
            new(ClaimTypes.Email, "test@test.com"),
            new(ClaimTypes.Role, UserRoles.Admin),
            new(ClaimTypes.Role, UserRoles.User),
            new("Nationality", "German"),
            new("DateOfBirth", dateOfBirth.ToString("yyyy-MM-dd"))


        };
        //user object of the claims value by assigning a new claims principal object
        //passing the claimsIdentity and the and claims as it's constructor parameter

        var user = new ClaimsPrincipal(new ClaimsIdentity(claims, "Test"));


        // our mock in the memory of the test will create a default httpcontext
        httpContextAccessorMock.Setup(x => x.HttpContext).Returns(new DefaultHttpContext()
        {
            //and provides our custom user definition principle to the user property of our http context
            User = user,
        });

        var userContext = new UserContext(httpContextAccessorMock.Object);



        //act

        var currentUser = userContext.GetCurrentUser();


        //assertion

        currentUser.Should().NotBeNull();

        currentUser?.Id.Should().Be("1");

        currentUser?.Email.Should().Be("test@test.com");

        currentUser?.Roles.Should().ContainInOrder(UserRoles.Admin, UserRoles.User );

        currentUser?.Nationality.Should().Be("German");

        currentUser?.DateOfBirth.Should().Be(dateOfBirth);


    }

    [Fact]
    public void GetCurrentUser_WithUserContextNotPresent_ThrowsInvalidOperationException()
    {
        //Arrange 
        //we create an empty mock data that returns a null value for the Httpcontext object

        var httpContextAccessorMock = new Mock<IHttpContextAccessor>();

        httpContextAccessorMock.Setup(x => x.HttpContext).Returns((HttpContext)null);

        //we then create a new user context class with the mock object as it's parameter

        var userContext = new UserContext(httpContextAccessorMock.Object);


        //act

        //we can specify an action  that will invoke the getcurrentUser method from the UserContext


        Action action = () => userContext.GetCurrentUser();


        //Assert

        action.Should().Throw<InvalidOperationException>()
                       .WithMessage("User context is not present");

    }
}