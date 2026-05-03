using LMS.Domain.Entities;
using LMS.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LMS.Infrastructure.Persistence.Repositories
{
    public sealed class CourseRepository(LMSDbContext context)
        : Repository<Course>(context), ICourseRepository
    {
        public async Task<Course?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await DbSet
                .Where(c => !c.IsDeleted && c.Id == id)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<List<Course>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await DbSet
                .Where(c => !c.IsDeleted)
                .Include(c => c.Instructor)
                .Include(c => c.Category).Include(s=> s.Sections)
                    .ThenInclude(s => s.Lessons)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<Course>> GetByInstructorIdAsync(Guid instructorId, CancellationToken cancellationToken = default)
        {
            return await DbSet
                .Where(c => c.InstructorId == instructorId && !c.IsDeleted)
                .Include(c => c.Category)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task<Course?> GetDetailsByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await DbSet
                .Include(c => c.Instructor)
                .Include(c => c.Category)
                .Include(c => c.Sections)
                    .ThenInclude(s => s.Lessons)
                .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted, cancellationToken);
        }

        public async Task<Course?> GetByIdWithSectionsAsync(
            Guid courseId, CancellationToken ct = default)
        {
            return await DbSet
                .Include(c => c.Instructor)
                .Include(c => c.Category)
                .Include(c => c.Sections.OrderBy(s => s.OrderIndex))
                    .ThenInclude(s => s.Lessons.OrderBy(l => l.OrderIndex))
                .Include(c => c.CourseTags)
                    .ThenInclude(ct2 => ct2.Tag)
                .FirstOrDefaultAsync(c => c.Id == courseId, ct);
        }

        public async Task AddAsync(Course course, CancellationToken cancellationToken = default)
        {
            await DbSet.AddAsync(course, cancellationToken);
        }

        public void Update(Course course)
        {
            DbSet.Update(course);
        }

        public void Delete(Course course)
        {
            DbSet.Remove(course);
        }
    }
}