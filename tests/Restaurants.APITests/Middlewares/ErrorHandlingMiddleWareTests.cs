using Castle.Core.Logging;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Restaurants.Domain.Entities;
using Restaurants.Domain.Exceptions;
using Xunit;


namespace Restaurants.API.Middlewares.Tests
{
    public class ErrorHandlingMiddleWareTests
    {
        [Fact()]
        public async Task InvokeAsync_WhenNoExceptionThrown_ShouldCallNextDelegate()
        {
            //arrange

            var loggerMock = new Mock<ILogger<ErrorHandlingMiddleWare>>();

            //with this mock we should be able to create that middleware as a new error handling middleware
           // and passing the loggerMock object as a parameter 

            var middleware = new ErrorHandlingMiddleWare(loggerMock.Object);

            var context = new DefaultHttpContext();

            var nextDelegateMock = new Mock<RequestDelegate>();

            //we also have to create the httpcontext

            //act
           await  middleware.InvokeAsync(context, nextDelegateMock.Object);// pass an empty request delegate as a lamda expression

            //assert

            nextDelegateMock.Verify(next => next.Invoke(context), Times.Once);

        }

        [Fact()]

        public async Task InvokeAsync_WhenNotFoundExceptionThrown_ShouldSetStatusCode404()
        {
            //Arrange

            var context = new DefaultHttpContext();
            var loggerMock = new  Mock<ILogger<ErrorHandlingMiddleWare>>();
            var middleware = new ErrorHandlingMiddleWare(loggerMock.Object);
            var notFoundException = new NotFoundExceptions(nameof(Restaurant), "4e8340ac-6430-494f-b325-23f864fbbb90");

            //act 

            await middleware.InvokeAsync(context, _ => throw  notFoundException);

            //assert

            context.Response.StatusCode.Should().Be(404);
        }

        [Fact()]
        public async Task InvokeAsync_WhenForbidExceptionThrown_ShouldSetStatusCode403()
        {
            //Arrange

            var context = new DefaultHttpContext();
            var loggerMock = new  Mock<ILogger<ErrorHandlingMiddleWare>>();
            var middleware = new ErrorHandlingMiddleWare(loggerMock.Object);
            var execption = new ForbidException();

            //act 

            await middleware.InvokeAsync(context, _ => throw  execption);

            //assert

            context.Response.StatusCode.Should().Be(403);
        }

        [Fact()]
        public async Task InvokeAsync_WhenGenericExceptionThrown_ShouldSetStatusCode500()
        {
            //Arrange

            var context = new DefaultHttpContext();
            var loggerMock = new Mock<ILogger<ErrorHandlingMiddleWare>>();
            var middleware = new ErrorHandlingMiddleWare(loggerMock.Object);
            var exception = new Exception();

            //act 

            await middleware.InvokeAsync(context, _ => throw exception);

            //assert

            context.Response.StatusCode.Should().Be(500);
        }

    }
}