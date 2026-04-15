using LMS.Domain.Entities;
using LMS.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Infrastructure.Persistence.Repositories
{
    public sealed class LessonProgressRepository(LMSDbContext context)
    : Repository<LessonProgress>(context), ILessonProgressRepository
    {
        public async Task<LessonProgress?> GetByEnrollmentAndLessonAsync(
            Guid enrollmentId,
            Guid lessonId,
            CancellationToken ct = default)=>
            await DbSet.FirstOrDefaultAsync(lp => lp.EnrollmentId == enrollmentId && lp.LessonId == lessonId, ct);

        public async Task<int> CountCompletedAsync(
            Guid enrollmentId,
            CancellationToken ct = default) =>
            await DbSet.CountAsync(lp => lp.EnrollmentId == enrollmentId && lp.IsCompleted, ct);

    }
}
