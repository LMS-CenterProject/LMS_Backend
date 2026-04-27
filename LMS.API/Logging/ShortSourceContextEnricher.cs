using Serilog.Core;
using Serilog.Events;

namespace LMS.API.Logging;

public sealed class ShortSourceContextEnricher : ILogEventEnricher
{
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        if (!logEvent.Properties.TryGetValue("SourceContext", out var sourceContextValue) ||
            sourceContextValue is not ScalarValue { Value: string sourceContext } ||
            string.IsNullOrWhiteSpace(sourceContext))
        {
            return;
        }

        var separatorIndex = sourceContext.LastIndexOf('.');
        var shortSource = separatorIndex >= 0
            ? sourceContext[(separatorIndex + 1)..]
            : sourceContext;

        logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("Source", shortSource));
    }
}
