namespace Restaurants.Application.Users;

public record CurrentUser(string Id, string Email, IEnumerable<string> Roles, string? Nationality, DateOnly? DateOfBirth)
{
    //checking if a user is in a concrete role or not
    //all we have to do is just check whether our property rules contains the role parameter passed down

    public bool IsInRole(string RoleName) => Roles.Contains(RoleName);
}
