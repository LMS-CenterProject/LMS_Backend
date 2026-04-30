using LMS.Application.Common.Interfaces;
using LMS.Application.Common.Models;
using LMS.Domain.Errors;
using LMS.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Application.Features.Notifications.Commands.MarkRead
{
    public sealed class MarkReadCommandHandler(
        IUnitOfWork uow,
        ICurrentUserService current)
        : IRequestHandler<MarkReadCommand, Result>
    {
        public async Task<Result> Handle(MarkReadCommand cmd, CancellationToken ct)
        {
            // 1. Find the notification
            var notification = await uow.Notifications.GetByIdAsync(cmd.NotificationId, ct);

            if (notification is null)
                return Result.Failure(DomainErrors.Notification.NotFound);

            // 2. Ownership check — users can only mark their own notifications
            if (notification.UserId != current.UserId)
                return Result.Failure(DomainErrors.Notification.Unauthorized);

            // 3. Idempotent — if already read, succeed without a DB write
            if (notification.IsRead)
                return Result.Success();

            // 4. Mark as read and save
            notification.MarkAsRead();
            await uow.SaveChangesAsync(ct);

            return Result.Success();
        }
    }
}
