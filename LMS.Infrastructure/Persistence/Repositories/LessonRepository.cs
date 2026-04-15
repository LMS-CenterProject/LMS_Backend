using LMS.Domain.Entities;
using LMS.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Infrastructure.Persistence.Repositories
{
    public class LessonRepository:ILessonRepository
    {
        private readonly LMSDbContext _context;

        public LessonRepository(LMSDbContext context)
        {
            _context = context;
        }

        public async Task<Lesson?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Lessons
                .FirstOrDefaultAsync(l => l.Id == id, cancellationToken);
        }

        public async Task<List<Lesson>> GetLessonsBySectionIdAsync(Guid sectionId, CancellationToken cancellationToken = default)
        {
            return await _context.Lessons
                .Where(l => l.SectionId == sectionId)
                .OrderBy(l => l.OrderIndex)
                .ToListAsync(cancellationToken);
        }
        public async Task<List<Lesson>> GetActiveLessonsBySectionIdAsync(Guid sectionId, CancellationToken cancellationToken = default)
        {
            return await _context.Lessons
                .Where(l => l.SectionId == sectionId && !l.IsDeleted)
                .OrderBy(l => l.OrderIndex)
                .ToListAsync(cancellationToken);
        }
        public async Task AddAsync(Lesson lesson, CancellationToken cancellationToken = default)
        {
            await _context.Lessons.AddAsync(lesson, cancellationToken);
        }

        public void Update(Lesson lesson)
        {
            _context.Lessons.Update(lesson);
        }

        public void Delete(Lesson lesson)
        {
            _context.Lessons.Remove(lesson);
        }
    }
}
