using LMS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Domain.Interfaces.Repositories
{
    public interface INotificationRepository:IRepository<Notification>
    {
        Task<Notification?> GetByIdAsync(
            Guid notificationId,
            CancellationToken ct = default);
        Task<(IEnumerable<Notification> Items, int TotalCount)> GetByUserAsync(
            Guid userId,
            int page,
            int pageSize,
            bool onlyUnread = false,
            CancellationToken ct = default);

        Task<int> CountUnreadAsync(
            Guid userId,
            CancellationToken ct = default);

        Task<IEnumerable<Notification>> GetAllUnreadByUserAsync(
            Guid userId,
            CancellationToken ct = default);

    }
}
