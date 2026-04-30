using LMS.Application.Common.Interfaces;
using LMS.Application.Common.Models;
using LMS.Application.DTOs;
using LMS.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Application.Features.Notifications.Queries.GetNotifications
{
    public sealed class GetNotificationsQueryHandler(
        IUnitOfWork uow,
        ICurrentUserService current)
        : IRequestHandler<GetNotificationsQuery, Result<NotificationsResultDto>>
    {
        private const int MaxPageSize = 50;

        public async Task<Result<NotificationsResultDto>> Handle(
            GetNotificationsQuery query, CancellationToken ct)
        {
            var userId = current.UserId!.Value;
            var page = Math.Max(query.Page, 1);
            var pageSize = Math.Min(query.PageSize, MaxPageSize);

            // Two separate queries — paginated items + total unread count
            var (items, totalCount) = await uow.Notifications.GetByUserAsync(
                userId, page, pageSize, query.OnlyUnread, ct);

            var unreadCount = await uow.Notifications.CountUnreadAsync(userId, ct);

            var totalPages = totalCount == 0 ? 0
                : (int)Math.Ceiling((double)totalCount / pageSize);

            var dtos = items.Select(n => new NotificationDto(
                Id: n.Id,
                Title: n.Title,
                Message: n.Message,
                IsRead: n.IsRead,
                CreatedAt: n.CreatedAt));

            return Result.Success(new NotificationsResultDto(
                Items: dtos,
                TotalCount: totalCount,
                UnreadCount: unreadCount,
                Page: page,
                PageSize: pageSize,
                TotalPages: totalPages));
        }
    }
}
