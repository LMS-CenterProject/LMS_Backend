using LMS.Domain.Entities;
using LMS.Domain.Enums;
using LMS.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Infrastructure.Persistence.Repositories
{
    public sealed class ReviewRepository(LMSDbContext context)
       : Repository<Review>(context), IReviewRepository
    {
        public async Task<Review?> GetByIdAsync(
            Guid reviewId, CancellationToken ct = default) =>
            await DbSet
                .Include(r => r.Student)
                .FirstOrDefaultAsync(r => r.Id == reviewId, ct);

        public async Task<bool> ExistsAsync(
            Guid studentId,
            Guid targetId,
            ReviewTargetType targetType,
            CancellationToken ct = default) =>
            await DbSet.AnyAsync(
                r => r.StudentId == studentId &&
                     r.TargetId == targetId &&
                     r.TargetType == targetType, ct);

        public async Task<IEnumerable<Review>> GetByTargetAsync(
            Guid targetId,
            ReviewTargetType targetType,
            CancellationToken ct = default) =>
            await DbSet
                .Where(r => r.TargetId == targetId && r.TargetType == targetType)
                .Include(r => r.Student)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync(ct);

        public async Task<double> GetAverageRatingAsync(
            Guid targetId,
            ReviewTargetType targetType,
            CancellationToken ct = default)
        {
            var hasReviews = await DbSet.AnyAsync(
                r => r.TargetId == targetId && r.TargetType == targetType, ct);

            if (!hasReviews) return 0;

            return await DbSet
                .Where(r => r.TargetId == targetId && r.TargetType == targetType)
                .AverageAsync(r => (double)r.Rating, ct);
        }
    }
}
