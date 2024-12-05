
using Microsoft.AspNetCore.Http.HttpResults;
using Restaurants.Domain.Exceptions;

namespace Restaurants.API.Middlewares;

public class ErrorHandlingMiddleWare(ILogger<ErrorHandlingMiddleWare> _logger) : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next.Invoke(context);

        }
        catch (NotFoundExceptions notFound)
        {
            context.Response.StatusCode = 404;
            await context.Response.WriteAsync(notFound.Message);
            _logger.LogWarning(notFound.Message);

        }
        catch(ForbidException) 
        {
            context.Response.StatusCode = 403;
            await context.Response.WriteAsync("Access forbidden");
        
        }

        catch (Exception ex) 
        {
            _logger.LogError(ex, ex.Message);

            context.Response.StatusCode = 500;
           await context.Response.WriteAsync("Something went wrong");
        }
    }
}
