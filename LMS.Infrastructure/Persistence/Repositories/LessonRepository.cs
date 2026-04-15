using LMS.Domain.Entities;
using LMS.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LMS.Infrastructure.Persistence.Repositories
{
    public sealed class LessonRepository(LMSDbContext context)
        : Repository<Lesson>(context), ILessonRepository
    {
        public async Task<Lesson?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await DbSet
                .FirstOrDefaultAsync(l => l.Id == id, cancellationToken);
        }

        public async Task<List<Lesson>> GetLessonsBySectionIdAsync(Guid sectionId, CancellationToken cancellationToken = default)
        {
            return await DbSet
                .Where(l => l.SectionId == sectionId)
                .OrderBy(l => l.OrderIndex)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<Lesson>> GetActiveLessonsBySectionIdAsync(Guid sectionId, CancellationToken cancellationToken = default)
        {
            return await DbSet
                .Where(l => l.SectionId == sectionId && !l.IsDeleted)
                .OrderBy(l => l.OrderIndex)
                .ToListAsync(cancellationToken);
        }

        public async Task<int> CountByCourseAsync(Guid courseId, CancellationToken ct = default)
        {
            return await DbSet
                .Where(l => l.Section.CourseId == courseId)
                .CountAsync(ct);
        }

        public async Task<Lesson?> GetByIdWithSectionAsync(Guid lessonId, CancellationToken ct = default)
        {
            return await DbSet
                .Include(l => l.Section)
                .FirstOrDefaultAsync(l => l.Id == lessonId, ct);
        }

        public async Task AddAsync(Lesson lesson, CancellationToken cancellationToken = default)
        {
            await DbSet.AddAsync(lesson, cancellationToken);
        }

        public void Update(Lesson lesson)
        {
            DbSet.Update(lesson);
        }

        public void Delete(Lesson lesson)
        {
            DbSet.Remove(lesson);
        }
    }
}