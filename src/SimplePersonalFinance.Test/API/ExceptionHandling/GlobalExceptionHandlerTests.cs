using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
using SimplePersonalFinance.API.ExceptionHandling;
using SimplePersonalFinance.Core.Domain.Exceptions;

namespace SimplePersonalFinance.Test.API.ExceptionHandling;

public class GlobalExceptionHandlerTests
{
    [Fact]
    public async Task TryHandleAsync_WhenEntityIsMissing_ShouldReturn404ProblemDetails()
    {
        var entityId = Guid.NewGuid();
        var exception = new EntityNotFoundException("Account", entityId);
        var (handler, problemDetailsService, capturedProblem) = CreateHandler(Environments.Production);
        var httpContext = CreateHttpContext();

        var handled = await handler.TryHandleAsync(httpContext, exception, CancellationToken.None);

        Assert.True(handled);
        Assert.Equal(StatusCodes.Status404NotFound, httpContext.Response.StatusCode);
        Assert.NotNull(capturedProblem.Value);
        Assert.Equal("Resource Not Found", capturedProblem.Value.Title);
        Assert.Equal("Account", capturedProblem.Value.Extensions["entityName"]);
        Assert.Equal(entityId, capturedProblem.Value.Extensions["entityId"]);
        problemDetailsService.Verify(service => service.WriteAsync(It.IsAny<ProblemDetailsContext>()), Times.Once);
    }

    [Fact]
    public async Task TryHandleAsync_WhenBusinessRuleIsViolated_ShouldReturn422ProblemDetails()
    {
        var exception = new BusinessRuleViolationException("Duplicated Email", "Email already exists");
        var (handler, _, capturedProblem) = CreateHandler(Environments.Production);
        var httpContext = CreateHttpContext();

        var handled = await handler.TryHandleAsync(httpContext, exception, CancellationToken.None);

        Assert.True(handled);
        Assert.Equal(StatusCodes.Status422UnprocessableEntity, httpContext.Response.StatusCode);
        Assert.NotNull(capturedProblem.Value);
        Assert.Equal("Business Rule Violation", capturedProblem.Value.Title);
        Assert.Equal("Duplicated Email", capturedProblem.Value.Extensions["ruleName"]);
    }

    [Fact]
    public async Task TryHandleAsync_WhenValidationFails_ShouldReturn400WithValidationErrors()
    {
        var errors = new Dictionary<string, string[]>
        {
            ["Email"] = ["Email is required"]
        };
        var exception = new ValidationException(errors);
        var (handler, _, capturedProblem) = CreateHandler(Environments.Production);
        var httpContext = CreateHttpContext();

        var handled = await handler.TryHandleAsync(httpContext, exception, CancellationToken.None);

        Assert.True(handled);
        Assert.Equal(StatusCodes.Status400BadRequest, httpContext.Response.StatusCode);
        Assert.NotNull(capturedProblem.Value);
        Assert.Equal("Validation Error", capturedProblem.Value.Title);
        Assert.Same(errors, capturedProblem.Value.Extensions["errors"]);
    }

    [Fact]
    public async Task TryHandleAsync_WhenUnexpectedExceptionOccursInProduction_ShouldHideInternalMessage()
    {
        var exception = new InvalidOperationException("internal database detail");
        var (handler, _, capturedProblem) = CreateHandler(Environments.Production);
        var httpContext = CreateHttpContext();

        var handled = await handler.TryHandleAsync(httpContext, exception, CancellationToken.None);

        Assert.True(handled);
        Assert.Equal(StatusCodes.Status500InternalServerError, httpContext.Response.StatusCode);
        Assert.NotNull(capturedProblem.Value);
        Assert.Equal("Server Error", capturedProblem.Value.Title);
        Assert.Equal("An unexpected error occurred while processing your request.", capturedProblem.Value.Detail);
        Assert.DoesNotContain("internal database detail", capturedProblem.Value.Detail, StringComparison.Ordinal);
        Assert.True(capturedProblem.Value.Extensions.ContainsKey("traceId"));
    }

    private static (GlobalExceptionHandler Handler, Mock<IProblemDetailsService> ProblemDetailsService, ProblemDetailsCapture CapturedProblem)
        CreateHandler(string environmentName)
    {
        var logger = new Mock<ILogger<GlobalExceptionHandler>>();
        var environment = new Mock<IHostEnvironment>();
        var problemDetailsService = new Mock<IProblemDetailsService>();
        var capturedProblem = new ProblemDetailsCapture();

        environment.SetupGet(value => value.EnvironmentName).Returns(environmentName);
        problemDetailsService
            .Setup(service => service.WriteAsync(It.IsAny<ProblemDetailsContext>()))
            .Callback<ProblemDetailsContext>(context => capturedProblem.Value = context.ProblemDetails)
            .Returns(ValueTask.CompletedTask);

        return (
            new GlobalExceptionHandler(logger.Object, environment.Object, problemDetailsService.Object),
            problemDetailsService,
            capturedProblem);
    }

    private static DefaultHttpContext CreateHttpContext()
    {
        var context = new DefaultHttpContext();
        context.Request.Path = "/api/test";
        context.TraceIdentifier = "test-trace-id";
        return context;
    }

    private sealed class ProblemDetailsCapture
    {
        public Microsoft.AspNetCore.Mvc.ProblemDetails? Value { get; set; }
    }
}
