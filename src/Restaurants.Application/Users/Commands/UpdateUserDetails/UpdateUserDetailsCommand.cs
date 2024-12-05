using MediatR;

namespace Restaurants.Application.Users.Commands.UpdateUserDetails;

public class UpdateUserDetailsCommand : IRequest
{
    //we need the UserId to know the particular user we want to apply the property changes
    //we can get it from the Http Context of the send request by the API client 
    //instead of hardcoding the Id into the request path, we will get it from the Http request object...
    public DateOnly? DateOfBirth { get; set; }
    public string? Nationality { get; set; }



}
