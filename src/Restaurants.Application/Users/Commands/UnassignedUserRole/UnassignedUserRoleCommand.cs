using MediatR;

namespace Restaurants.Application.Users.Commands.UnassignedUserRole;

public class UnassignedUserRoleCommand : IRequest
{
    public string UserEmail { get; set; } = default!;

    public string RoleName { get; set; } = default!;

}
