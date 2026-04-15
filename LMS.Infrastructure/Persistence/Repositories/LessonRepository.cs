using LMS.Domain.Entities;
using LMS.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LMS.Infrastructure.Persistence.Repositories
{
    public class LessonRepository : Repository<Lesson>, ILessonRepository
    {
        public LessonRepository(LMSDbContext context)
            : base(context)
        {
        }

        public async Task<Lesson?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await Context.Lessons
                .FirstOrDefaultAsync(l => l.Id == id, cancellationToken);
        }

        public async Task<List<Lesson>> GetLessonsBySectionIdAsync(Guid sectionId, CancellationToken cancellationToken = default)
        {
            return await Context.Lessons
                .Where(l => l.SectionId == sectionId)
                .OrderBy(l => l.OrderIndex)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<Lesson>> GetActiveLessonsBySectionIdAsync(Guid sectionId, CancellationToken cancellationToken = default)
        {
            return await Context.Lessons
                .Where(l => l.SectionId == sectionId && !l.IsDeleted)
                .OrderBy(l => l.OrderIndex)
                .ToListAsync(cancellationToken);
        }

        public async Task AddAsync(Lesson lesson, CancellationToken cancellationToken = default)
        {
            await base.AddAsync(lesson, cancellationToken);
        }

        public void Update(Lesson lesson)
        {
            base.Update(lesson);
        }

        public void Delete(Lesson lesson)
        {
            base.Remove(lesson);
        }
    }
}