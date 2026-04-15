using LMS.Domain.Entities;
using LMS.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LMS.Infrastructure.Persistence.Repositories
{
    public class SectionRepository : Repository<Section>, ISectionRepository
    {
        public SectionRepository(LMSDbContext context)
            : base(context)
        {
        }

        public async Task<Section?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await Context.Sections
                .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
        }

        public async Task<List<Section>> GetSectionsByCourseIdAsync(Guid courseId, CancellationToken cancellationToken = default)
        {
            return await Context.Sections
                .Where(s => s.CourseId == courseId)
                .OrderBy(s => s.OrderIndex)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<Section>> GetSectionsWithLessonsByCourseIdAsync(Guid courseId, CancellationToken cancellationToken = default)
        {
            return await Context.Sections
                .Include(s => s.Lessons)
                .Where(s => s.CourseId == courseId && !s.IsDeleted)
                .OrderBy(s => s.OrderIndex)
                .ToListAsync(cancellationToken);
        }

        public async Task AddAsync(Section section, CancellationToken cancellationToken = default)
        {
            await base.AddAsync(section, cancellationToken);
        }

        public void Update(Section section)
        {
            base.Update(section);
        }

        public void Delete(Section section)
        {
            base.Remove(section);
        }
    }
}