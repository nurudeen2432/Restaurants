using Microsoft.OpenApi.Models;
using Restaurants.API.Middlewares;
using Serilog;

namespace Restaurants.API.Extensions;

public static class WebApplicationBuilderExt
{
    //With this we are going to create an extension method on the
    //web application builder type
    public static void AddPresentation(this WebApplicationBuilder builder)
    {
        //with this service added to our webAPI, we should be able to run 
        //the application, get the token for an existing user, and then 
        //use the token in a form of header as a bearer token for any http request.
        builder.Services.AddAuthentication();


        builder.Services.AddControllers();

        //Configuration to add security definition for the bearer token in swagger doc
        //by extending the swagger gen method by providing a config object 

        builder.Services.AddSwaggerGen(
            c =>
            {
                //on the config options object we invoke methods

                c.AddSecurityDefinition("bearerAuth", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer"
                });

                //we need to add a config to allow us pass the bearer token saved on swagger memory to all request made in the 
                //swagger doc interface

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "bearerAuth"
                    }

                },
                []
            }
                });
            }
            );


        //This will include every minimal endpoint from our api through a swagger Interface
        builder.Services.AddEndpointsApiExplorer();

        builder.Services.AddScoped<ErrorHandlingMiddleWare>();

        builder.Services.AddScoped<RequestTimeLoggingMiddleware>();

        builder.Host.UseSerilog((context, configuration) =>
        {
            configuration.ReadFrom.Configuration(context.Configuration);

            //.MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            //.MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Information)
            //.WriteTo.File("Logs/Restaurant-API- .log", rollingInterval: RollingInterval.Day, rollOnFileSizeLimit: true)
            //.WriteTo.Console(outputTemplate: "[{Timestamp:dd-MM HH:mm:ss} {Level:u3}] |{SourceContext}| {NewLine}{Message:lj}{NewLine}{Exception}");


        });

    }

}
