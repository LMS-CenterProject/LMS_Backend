using Serilog.Core;
using Serilog.Events;
using System.Collections.Concurrent;

namespace LMS.API.Logging
{
    /// <summary>
    /// Thread-safe circular buffer that holds the last N log entries.
    /// Registered as singleton so the admin endpoint can read from it.
    /// </summary>
    public sealed class InMemorySink : ILogEventSink
    {
        private readonly ConcurrentQueue<LogEntry> _entries = new();
        private readonly int _maxEntries;
        private int _count;

        public InMemorySink(int maxEntries = 500)
        {
            _maxEntries = maxEntries;
        }

        public void Emit(LogEvent logEvent)
        {
            var entry = new LogEntry
            {
                Timestamp = logEvent.Timestamp.UtcDateTime,
                Level = logEvent.Level.ToString(),
                Message = logEvent.RenderMessage(),
                Exception = logEvent.Exception?.ToString(),
                SourceContext = GetProperty(logEvent, "Source")
                             ?? GetProperty(logEvent, "SourceContext"),
                CorrelationId = GetProperty(logEvent, "CorrelationId"),
                RequestMethod = GetProperty(logEvent, "Method"),
                RequestPath = GetProperty(logEvent, "Path"),
                StatusCode = GetProperty(logEvent, "StatusCode"),
                ElapsedMs = GetProperty(logEvent, "ElapsedMs"),
                User = GetProperty(logEvent, "User"),
            };

            _entries.Enqueue(entry);
            Interlocked.Increment(ref _count);

            // Keep buffer bounded — drop oldest when full
            while (_count > _maxEntries)
            {
                if (_entries.TryDequeue(out _))
                    Interlocked.Decrement(ref _count);
            }
        }

        public IReadOnlyList<LogEntry> GetEntries() =>
            _entries.ToArray();

        private static string? GetProperty(LogEvent logEvent, string name) =>
            logEvent.Properties.TryGetValue(name, out var val)
                ? val is ScalarValue sv ? sv.Value?.ToString() : val.ToString()
                : null;
    }

    public sealed class LogEntry
    {
        public DateTime Timestamp { get; set; }
        public string Level { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string? Exception { get; set; }
        public string? SourceContext { get; set; }
        public string? CorrelationId { get; set; }
        public string? RequestMethod { get; set; }
        public string? RequestPath { get; set; }
        public string? StatusCode { get; set; }
        public string? ElapsedMs { get; set; }
        public string? User { get; set; }
    }
}
