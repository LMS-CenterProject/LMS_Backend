using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
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
            var correlationId = httpContext.Response.Headers
                .TryGetValue("X-Correlation-ID", out var correlationIdHeader)
                    ? correlationIdHeader.ToString()
                    : httpContext.TraceIdentifier;

            logger.LogError(
                exception,
                "Unhandled exception | Method: {Method} | Path: {Path} | CorrelationId: {CorrelationId} | Message: {Message}",
                httpContext.Request.Method,
                httpContext.Request.Path,
                correlationId,
                exception.Message);

            var (statusCode, title) = exception switch
            {
                ValidationException => (400, "Validation failed"),
                UnauthorizedAccessException => (403, "Forbidden"),
                KeyNotFoundException => (404, "Resource not found"),
                InvalidOperationException => (400, "Invalid operation"),
                _ => (500, "An unexpected error occurred")
            };

            var problem = new ProblemDetails
            {
                Status = statusCode,
                Title = title,
                Instance = httpContext.Request.Path,
                Detail = statusCode == 500
                    ? "An unexpected error occurred. Please try again later."
                    : exception.Message
            };

            // Always include traceId and timestamp for debugging
            problem.Extensions["traceId"] = correlationId;
            problem.Extensions["timestamp"] = DateTime.UtcNow.ToString("o");

            // Validation: include field-level errors
            if (exception is ValidationException ve)
            {
                problem.Extensions["errors"] = ve.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.ErrorMessage).ToArray());
            }

            httpContext.Response.StatusCode = statusCode;
            httpContext.Response.ContentType = "application/problem+json";

            await httpContext.Response.WriteAsJsonAsync(problem, ct);
            return true;
        }
    }
}
