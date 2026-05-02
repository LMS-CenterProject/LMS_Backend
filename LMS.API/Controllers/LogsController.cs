using LMS.API.Extensions;
using LMS.Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LMS.API.Controllers
{
    /// <summary>
    /// Admin-only endpoint to view recent application logs.
    /// Returns the last 500 log entries held in the in-memory buffer.
    /// </summary>
    [ApiController]
    [Route("api/admin/logs")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public sealed class LogsController : ControllerBase
    {
        private const int MaxPageSize = 100;

        /// <summary>
        /// Get recent application logs with filtering and pagination.
        /// </summary>
        /// <param name="page">Page number (default: 1)</param>
        /// <param name="pageSize">Items per page, max 100 (default: 50)</param>
        /// <param name="level">Filter by level: Information, Warning, Error, Fatal</param>
        /// <param name="search">Search in message, path, or user</param>
        /// <param name="from">Filter logs after this UTC datetime (ISO 8601)</param>
        /// <param name="to">Filter logs before this UTC datetime (ISO 8601)</param>
        /// <param name="onlyErrors">Show only Warning, Error, and Fatal entries</param>
        [HttpGet]
        [ProducesResponseType(typeof(LogsResultDto), StatusCodes.Status200OK)]
        public IActionResult GetLogs(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 50,
            [FromQuery] string? level = null,
            [FromQuery] string? search = null,
            [FromQuery] DateTime? from = null,
            [FromQuery] DateTime? to = null,
            [FromQuery] bool onlyErrors = false)
        {
            page = Math.Max(page, 1);
            pageSize = Math.Min(pageSize, MaxPageSize);

            // Pull from the singleton in-memory buffer
            var all = LoggingExtensions.MemorySink.GetEntries();

            // ── Apply filters ─────────────────────────────────────
            var query = all.AsEnumerable();

            if (onlyErrors)
                query = query.Where(e =>
                    e.Level is "Warning" or "Error" or "Fatal");

            if (!string.IsNullOrWhiteSpace(level))
                query = query.Where(e =>
                    e.Level.Equals(level, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrWhiteSpace(search))
            {
                var lower = search.ToLowerInvariant();
                query = query.Where(e =>
                    (e.Message?.ToLowerInvariant().Contains(lower) ?? false) ||
                    (e.RequestPath?.ToLowerInvariant().Contains(lower) ?? false) ||
                    (e.User?.ToLowerInvariant().Contains(lower) ?? false) ||
                    (e.SourceContext?.ToLowerInvariant().Contains(lower) ?? false));
            }

            if (from.HasValue)
                query = query.Where(e => e.Timestamp >= from.Value);

            if (to.HasValue)
                query = query.Where(e => e.Timestamp <= to.Value);

            // ── Newest first ──────────────────────────────────────
            var filtered = query.OrderByDescending(e => e.Timestamp).ToList();
            var filteredCount = filtered.Count;
            var totalPages = filteredCount == 0 ? 0
                : (int)Math.Ceiling((double)filteredCount / pageSize);

            var page_entries = filtered
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(e => new LogEntryDto(
                    Timestamp: e.Timestamp,
                    Level: e.Level,
                    Message: e.Message,
                    Exception: e.Exception,
                    SourceContext: e.SourceContext,
                    CorrelationId: e.CorrelationId,
                    RequestMethod: e.RequestMethod,
                    RequestPath: e.RequestPath,
                    StatusCode: e.StatusCode,
                    ElapsedMs: e.ElapsedMs,
                    User: e.User));

            return Ok(new LogsResultDto(
                Entries: page_entries,
                TotalCount: all.Count,
                FilteredCount: filteredCount,
                Page: page,
                PageSize: pageSize,
                TotalPages: totalPages));
        }

        /// <summary>
        /// Get a summary: counts by level for the buffered entries.
        /// Useful for a dashboard badge showing error counts.
        /// </summary>
        [HttpGet("summary")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetSummary()
        {
            var all = LoggingExtensions.MemorySink.GetEntries();

            var summary = new
            {
                TotalBuffered = all.Count,
                Information = all.Count(e => e.Level == "Information"),
                Warning = all.Count(e => e.Level == "Warning"),
                Error = all.Count(e => e.Level == "Error"),
                Fatal = all.Count(e => e.Level == "Fatal"),
                LastError = all
                    .Where(e => e.Level is "Error" or "Fatal")
                    .OrderByDescending(e => e.Timestamp)
                    .Select(e => new { e.Timestamp, e.Message, e.RequestPath })
                    .FirstOrDefault()
            };

            return Ok(summary);
        }
    }
}
