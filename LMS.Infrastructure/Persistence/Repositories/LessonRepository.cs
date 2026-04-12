using LMS.Domain.Entities;
using LMS.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Infrastructure.Persistence.Repositories
{
    public sealed class LessonRepository(LMSDbContext context)
    : Repository<Lesson>(context), ILessonRepository
    {
        public async Task<int> CountByCourseAsync(
            Guid courseId, CancellationToken ct = default) =>
            await DbSet
                .Where(l => l.Section.CourseId == courseId)
                .CountAsync(ct);

        public async Task<Lesson?> GetByIdWithSectionAsync(
            Guid lessonId, CancellationToken ct = default) =>
            await DbSet
                .Include(l => l.Section)
                .FirstOrDefaultAsync(l => l.Id == lessonId, ct);
    }
}
