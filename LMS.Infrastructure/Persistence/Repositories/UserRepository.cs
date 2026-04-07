using LMS.Domain.Entities;
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
    }
}
