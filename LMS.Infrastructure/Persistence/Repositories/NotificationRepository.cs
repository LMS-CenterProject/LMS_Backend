using LMS.Domain.Entities;
using LMS.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LMS.Infrastructure.Persistence.Repositories
{
    public sealed class NotificationRepository(LMSDbContext context)
           : Repository<Notification>(context), INotificationRepository
    {

        public async Task<Notification?> GetByIdAsync(Guid notificationId, CancellationToken ct = default) =>
            await DbSet.FirstOrDefaultAsync(n => n.Id == notificationId, ct);

        public async Task<(IEnumerable<Notification> Items, int TotalCount)> GetByUserAsync(
            Guid userId,
            int page,
            int pageSize,
            bool onlyUnread = false,
            CancellationToken ct = default)
        {
            if (page < 1) page = 1;
            var query = DbSet.AsQueryable().Where(n => n.UserId == userId);
            if (onlyUnread) query = query.Where(n => !n.IsRead);

            var total = await query.CountAsync(ct);
            var items = await query
                .OrderByDescending(n => n.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);

            return (items, total);
        }

        public Task<int> CountUnreadAsync(Guid userId, CancellationToken ct = default) =>
            DbSet.CountAsync(n => n.UserId == userId && !n.IsRead, ct);

        public async Task<IEnumerable<Notification>> GetAllUnreadByUserAsync(
            Guid userId, CancellationToken ct = default) =>
            await DbSet
                .Where(n => n.UserId == userId && !n.IsRead)
                .ToListAsync(ct);
    }
}
