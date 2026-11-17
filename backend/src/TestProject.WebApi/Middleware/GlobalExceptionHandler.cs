using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using TestProject.Application.Common.Exceptions;
using TestProject.Domain.Exceptions;

namespace TestProject.WebApi.Middleware;

public sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        switch (exception)
        {
            case AppValidationException validationException:
            {
                logger.LogWarning("Validation failed for {Path}", httpContext.Request.Path);

                var problemDetails = new ValidationProblemDetails(
                    new Dictionary<string, string[]>(validationException.Errors))
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = "Validation Failed",
                    Detail = validationException.Message,
                    Instance = httpContext.Request.Path
                };

                httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
                return true;
            }

            case DomainException domainException:
            {
                logger.LogWarning(domainException, "Domain rule violation on {Path}", httpContext.Request.Path);

                var problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status422UnprocessableEntity,
                    Title = "Domain Rule Violation",
                    Detail = domainException.Message,
                    Instance = httpContext.Request.Path
                };

                httpContext.Response.StatusCode = StatusCodes.Status422UnprocessableEntity;
                await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
                return true;
            }

            default:
            {
                logger.LogError(exception, "Unhandled exception while processing {Path}", httpContext.Request.Path);

                var problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Title = "Internal Server Error",
                    Detail = "An unexpected error occurred.",
                    Instance = httpContext.Request.Path
                };

                httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
                return true;
            }
        }
    }
}
