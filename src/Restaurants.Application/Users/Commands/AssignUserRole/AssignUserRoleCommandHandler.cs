using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Restaurants.Domain.Entities;
using Restaurants.Domain.Exceptions;

namespace Restaurants.Application.Users.Commands.AssignUserRole;

public class AssignUserRoleCommandHandler(
    ILogger<AssemblyLoadEventHandler> logger,
    UserManager<User> userManager,
    //takes identity role as its generic parameter
    RoleManager<IdentityRole> roleManager
    
    ) : IRequestHandler<AssignUserRoleCommand>
{
    public async Task Handle(AssignUserRoleCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation(">>>> Assigning user role: {@Request}", request);

        var user = await userManager.FindByEmailAsync(request.UserEmail)
            //null coalescing operator
            ?? throw new NotFoundExceptions(nameof(User), request.UserEmail);

        var role = await roleManager.FindByNameAsync(request.RoleName) ?? throw new NotFoundExceptions(nameof(IdentityRole), request.RoleName);


        await userManager.AddToRoleAsync(user, role.Name!);


       


        
    }
}
