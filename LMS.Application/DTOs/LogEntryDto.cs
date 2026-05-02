using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Application.DTOs
{
    public sealed record LogEntryDto(
        DateTime Timestamp,
        string Level,
        string Message,
        string? Exception,
        string? SourceContext,
        string? CorrelationId,
        string? RequestMethod,
        string? RequestPath,
        string? StatusCode,
        string? ElapsedMs,
        string? User);

    public sealed record LogsResultDto(
        IEnumerable<LogEntryDto> Entries,
        int TotalCount,
        int FilteredCount,
        int Page,
        int PageSize,
        int TotalPages);
}
