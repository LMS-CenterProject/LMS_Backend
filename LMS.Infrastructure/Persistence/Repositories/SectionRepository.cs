using LMS.Domain.Entities;
using LMS.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Infrastructure.Persistence.Repositories
{
    public class SectionRepository : ISectionRepository
    {
        private readonly LMSDbContext _context;
        public SectionRepository(LMSDbContext context)
        {
            _context = context;
        }


        public async Task<Section?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Sections
                .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
        }
        public async Task<List<Section>> GetSectionsByCourseIdAsync(Guid courseId, CancellationToken cancellationToken = default)
        {
            return await _context.Sections
                .Where(s => s.CourseId == courseId)
                .OrderBy(s => s.OrderIndex)
                .ToListAsync(cancellationToken);
        }
        public async Task<List<Section>> GetSectionsWithLessonsByCourseIdAsync(Guid courseId, CancellationToken cancellationToken = default)
        {
            return await _context.Sections
                .Include(s => s.Lessons)
                .Where(s => s.CourseId == courseId && !s.IsDeleted)
                .OrderBy(s => s.OrderIndex)
                .ToListAsync(cancellationToken);
        }
        public async Task AddAsync(Section section, CancellationToken cancellationToken = default)
        {
            await _context.Sections.AddAsync(section, cancellationToken);
        }

        public void Update(Section section)
        {
            _context.Sections.Update(section);
        }

        public void Delete(Section section)
        {
            _context.Sections.Remove(section);
        }



    }
}
