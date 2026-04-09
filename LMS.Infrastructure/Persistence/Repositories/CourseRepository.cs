using LMS.Domain.Entities;
using LMS.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Infrastructure.Persistence.Repositories
{
    public sealed class CourseRepository(LMSDbContext context)
    : Repository<Course>(context), ICourseRepository
    {
        public async Task<Course?> GetByIdWithSectionsAsync(
            Guid courseId, CancellationToken ct = default) =>
            await DbSet
                .Include(c => c.Instructor)
                .Include(c => c.Category)
                .Include(c => c.Sections.OrderBy(s => s.OrderIndex))
                    .ThenInclude(s => s.Lessons.OrderBy(l => l.OrderIndex))
                .Include(c => c.CourseTags)
                    .ThenInclude(ct2 => ct2.Tag)
                .FirstOrDefaultAsync(c => c.Id == courseId, ct);
    }
}
