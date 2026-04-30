using LMS.Application.Common.Interfaces;
using LMS.Application.Common.Models;
using LMS.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Application.Features.Notifications.Commands.MarkRead
{
    public sealed class MarkAllReadCommandHandler(
       IUnitOfWork uow,
       ICurrentUserService current)
       : IRequestHandler<MarkAllReadCommand, Result<int>>
    {
        public async Task<Result<int>> Handle(MarkAllReadCommand cmd, CancellationToken ct)
        {
            var userId = current.UserId!.Value;

            // Fetch only unread — no point loading read ones
            var unread = (await uow.Notifications
                .GetAllUnreadByUserAsync(userId, ct)).ToList();

            // Nothing to do — succeed immediately
            if (!unread.Any())
                return Result.Success(0);

            // Mark all — single SaveChangesAsync at the end
            foreach (var notification in unread)
                notification.MarkAsRead();

            await uow.SaveChangesAsync(ct);

            return Result.Success(unread.Count);
        }
    }
}
