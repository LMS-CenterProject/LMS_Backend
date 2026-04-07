using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
//using System.ComponentModel.DataAnnotations;
using FluentValidation;

namespace LMS.API.Middleware
{
    public sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken ct)
        {
            logger.LogError(exception, "Unhandled exception: {Message}", exception.Message);

            var (statusCode, title) = exception switch
            {
                ValidationException => (400, "Validation failed"),
                UnauthorizedAccessException => (403, "Forbidden"),
                _ => (500, "An unexpected error occurred")
            };

            var problem = new ProblemDetails
            {
                Status = statusCode,
                Title = title
            };

            if (exception is ValidationException ve)
            {
                problem.Extensions["errors"] = ve.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.ErrorMessage).ToArray());
            }

            httpContext.Response.StatusCode = statusCode;
            await httpContext.Response.WriteAsJsonAsync(problem, ct);
            return true;
        }
    }
}
