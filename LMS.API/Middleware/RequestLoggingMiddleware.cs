using Serilog.Context;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace LMS.API.Middleware;

public class RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
{
    private static readonly HashSet<string> ExcludedPaths = new()
    {
        "/health",
        "/swagger",
        "/favicon.ico"
    };

    public async Task InvokeAsync(HttpContext context)
    {
        if (ExcludedPaths.Any(path => context.Request.Path.StartsWithSegments(path, StringComparison.OrdinalIgnoreCase)))
        {
            await next(context);
            return;
        }

        var correlationId = context.Request.Headers.TryGetValue("X-Correlation-ID", out var correlationIdHeader)
            ? correlationIdHeader.ToString()
            : context.TraceIdentifier;

        context.TraceIdentifier = correlationId;
        context.Response.Headers["X-Correlation-ID"] = correlationId;

        var requestPath = $"{context.Request.Path}{context.Request.QueryString}";

        using (LogContext.PushProperty("CorrelationId", correlationId))
        {
            var stopwatch = Stopwatch.StartNew();
            await next(context);
            stopwatch.Stop();

            logger.Log(
                GetLogLevel(context.Response.StatusCode),
                "HTTP {Method} {Path} -> {StatusCode} in {ElapsedMs:0.000} ms [user:{User}] [corr:{CorrelationId}]",
                context.Request.Method,
                requestPath,
                context.Response.StatusCode,
                stopwatch.Elapsed.TotalMilliseconds,
                GetUserLabel(context.User),
                correlationId);
        }
    }

    private static LogLevel GetLogLevel(int statusCode) =>
        statusCode >= 500 ? LogLevel.Error :
        statusCode >= 400 ? LogLevel.Warning :
        LogLevel.Information;

    private static string GetUserLabel(ClaimsPrincipal user)
    {
        if (user.Identity?.IsAuthenticated != true)
        {
            return "anonymous";
        }

        var userId = user.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? user.FindFirstValue(JwtRegisteredClaimNames.Sub)
            ?? "unknown";

        var name = user.FindFirstValue(ClaimTypes.Name)
            ?? user.FindFirstValue(JwtRegisteredClaimNames.Name)
            ?? user.FindFirstValue("unique_name")
            ?? user.Identity?.Name
            ?? user.FindFirstValue(ClaimTypes.Email)
            ?? user.FindFirstValue(JwtRegisteredClaimNames.Email);

        return string.IsNullOrWhiteSpace(name)
            ? userId
            : $"{name} ({userId})";
    }
}
