using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Application.DTOs
{
    public sealed record NotificationDto(
        Guid Id,
        string Title,
        string Message,
        bool IsRead,
        DateTime CreatedAt);

    public sealed record NotificationsResultDto(
        IEnumerable<NotificationDto> Items,
        int TotalCount,
        int UnreadCount,
        int Page,
        int PageSize,
        int TotalPages);
}
