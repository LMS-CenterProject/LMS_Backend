using LMS.Domain.Entities;
using LMS.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LMS.Infrastructure.Persistence.Repositories
{
    public class CourseRepository : Repository<Course>, ICourseRepository
    {
        public CourseRepository(LMSDbContext context)
            : base(context)
        {
        }

        public async Task<Course?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await Context.Courses
                .Where(c => !c.IsDeleted && c.Id == id)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<List<Course>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await Context.Courses
                .Where(c => !c.IsDeleted)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<Course>> GetByInstructorIdAsync(Guid instructorId, CancellationToken cancellationToken = default)
        {
            return await Context.Courses
                .Where(c => c.InstructorId == instructorId && !c.IsDeleted)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task<Course?> GetDetailsByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await Context.Courses
                .Include(c => c.Sections)
                    .ThenInclude(s => s.Lessons)
                .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted, cancellationToken);
        }

        public async Task AddAsync(Course course, CancellationToken cancellationToken = default)
        {
            await base.AddAsync(course, cancellationToken);
        }

        public void Update(Course course)
        {
            base.Update(course);
        }

        public void Remove(Course course)
        {
            base.Remove(course);
        }
    }
}