using LMS.API.Logging;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;
using Serilog.Sinks.SystemConsole.Themes;

namespace LMS.API.Extensions;

public static class LoggingExtensions
{
    private const string ConsoleOutputTemplate =
        "[{Timestamp:HH:mm:ss} {Level:u3}] [{Source,-28}] {Message:lj}{NewLine}{Exception}";

    private const string FileOutputTemplate =
        "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} {Level:u3}] [{Source}] {Message:lj}{NewLine}{Exception}";

    public static WebApplicationBuilder AddSerilogLogging(
        this WebApplicationBuilder builder,
        string applicationName = "LMS.API")
    {
        var environment = builder.Environment.EnvironmentName;
        var logsPath = Path.Combine("logs", $"{applicationName}-{environment}");
        Directory.CreateDirectory(logsPath);

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
            .MinimumLevel.Override("System", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .Enrich.With(new ShortSourceContextEnricher())
            .Enrich.WithProperty("Application", applicationName)
            .Enrich.WithProperty("Environment", environment)
            .Enrich.WithMachineName()
            .Enrich.WithProcessId()
            .Enrich.WithThreadId()
            .WriteTo.Console(
                theme: AnsiConsoleTheme.Code,
                outputTemplate: ConsoleOutputTemplate)
            .WriteTo.File(
                path: Path.Combine(logsPath, "logs-.txt"),
                rollingInterval: RollingInterval.Day,
                rollOnFileSizeLimit: true,
                fileSizeLimitBytes: 10_000_000,
                retainedFileCountLimit: 30,
                shared: true,
                outputTemplate: FileOutputTemplate)
            .WriteTo.File(
                formatter: new CompactJsonFormatter(),
                path: Path.Combine(logsPath, "logs-.json"),
                rollingInterval: RollingInterval.Day,
                rollOnFileSizeLimit: true,
                fileSizeLimitBytes: 10_000_000,
                retainedFileCountLimit: 30,
                shared: true)
            .CreateLogger();

        builder.Host.UseSerilog(Log.Logger, dispose: true);
        return builder;
    }
}
