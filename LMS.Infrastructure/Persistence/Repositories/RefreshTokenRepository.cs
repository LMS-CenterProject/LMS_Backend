using LMS.Domain.Entities;
using LMS.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Infrastructure.Persistence.Repositories
{
    public sealed class RefreshTokenRepository(LMSDbContext context)
    : Repository<RefreshToken>(context), IRefreshTokenRepository
    {
        public async Task<RefreshToken?> GetByTokenAsync(
            string token, CancellationToken ct = default) =>
            await DbSet
                .Include(rt => rt.User)       // needed so handlers can access rt.User
                .FirstOrDefaultAsync(rt => rt.Token == token, ct);

        public async Task RevokeAllUserTokensAsync(
            Guid userId, CancellationToken ct = default)
        {
            var tokens = await DbSet
                .Where(rt => rt.UserId == userId && !rt.IsRevoked)
                .ToListAsync(ct);

            foreach (var t in tokens)
                t.Revoke();
        }
    }
}
