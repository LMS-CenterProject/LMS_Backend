using LMS.Application.DTOs.Course;
using LMS.Domain.Entities;
using LMS.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Infrastructure.Persistence.Repositories
{
    public class CourseRepository : ICourseRepository
    {
        private readonly LMSDbContext _context;
        public CourseRepository(LMSDbContext context)
        {
            _context = context;
        }

        public async Task<Course?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Courses.Where(c => !c.IsDeleted).FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<List<Course>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Courses.Where(c => !c.IsDeleted).OrderByDescending(c => c.CreatedAt).ToListAsync(cancellationToken);

        }

        public async Task<List<Course>> GetByInstructorIdAsync(Guid instructorId, CancellationToken cancellationToken = default)
        {
            return await _context.Courses
                .Where(c => c.InstructorId == instructorId)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task<Course?> GetDetailsByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Courses
                .Include(c => c.Sections)
                    .ThenInclude(s => s.Lessons)
                .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        }


        public async Task AddAsync(Course course, CancellationToken cancellationToken = default)
        {
            await _context.Courses.AddAsync(course, cancellationToken);
        }

        public void Update(Course course)
        {
            _context.Courses.Update(course);
        }

        public void Delete(Course course)
        {
            _context.Courses.Remove(course);
        }
    }
}
