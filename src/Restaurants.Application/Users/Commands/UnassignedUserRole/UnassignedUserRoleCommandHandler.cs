using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Restaurants.Domain.Entities;
using Restaurants.Domain.Exceptions;

namespace Restaurants.Application.Users.Commands.UnassignedUserRole;

public class UnassignedUserRoleCommandHandler(

    ILogger<UnassignedUserRoleCommandHandler> logger,
    UserManager<User> userManager,
    RoleManager<IdentityRole> roleManager
    ) : IRequestHandler<UnassignedUserRoleCommand>
{
    public async Task Handle(UnassignedUserRoleCommand request, CancellationToken cancellationToken)
    {
        logger.LogWarning(">>> Unassigning user role : {request} ", request);

        var user = await userManager.FindByEmailAsync(request.UserEmail) 
            ?? throw new NotFoundExceptions(nameof(User), request.UserEmail);

        
        var role = await roleManager.FindByNameAsync(request.RoleName)
            ?? throw new NotFoundExceptions(nameof(IdentityRole), request.RoleName);


        await userManager.RemoveFromRoleAsync(user, role.Name!);


    }
}
