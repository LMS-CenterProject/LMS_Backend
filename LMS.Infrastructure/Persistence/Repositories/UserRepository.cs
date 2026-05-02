using LMS.Domain.Entities;
using LMS.Domain.Enums;
using LMS.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Infrastructure.Persistence.Repositories
{
    public sealed class UserRepository(LMSDbContext context)
    : Repository<User>(context), IUserRepository
    {
        public async Task<User?> GetByEmailAsync(string email, CancellationToken ct = default) =>
            await DbSet
                .FirstOrDefaultAsync(u => u.Email == email.ToLowerInvariant(), ct);

        public async Task<User?> GetByGoogleIdAsync(string googleId, CancellationToken ct = default) =>
            await DbSet
                .FirstOrDefaultAsync(u => u.GoogleId == googleId, ct);

        public async Task<bool> EmailExistsAsync(string email, CancellationToken ct = default) =>
            await DbSet
                .AnyAsync(u => u.Email == email.ToLowerInvariant(), ct);

       public async Task<(IEnumerable<User> Users, int TotalCount)> GetAllPaginatedAsync(
                    int page,
                    int pageSize,
                    string? search = null,
                    UserRole? role = null,
                    CancellationToken ct = default)
        {
            var query = DbSet.AsNoTracking().AsQueryable();
            // Filter by role
            if (role.HasValue)
                query = query.Where(u => u.Role == role.Value);

            var normalizedSearch = search?.Trim();
            if (!string.IsNullOrWhiteSpace(normalizedSearch))
            {
                var pattern = $"%{normalizedSearch}%";
                query = query.Where(u =>
                    EF.Functions.Like(u.FullName, pattern) ||
                    EF.Functions.Like(u.Email, pattern));
            }

            var totalCount = await query.CountAsync(ct);
            var users = await query
                .OrderBy(u => u.FullName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);

            return (users, totalCount);


        }
    }
}
