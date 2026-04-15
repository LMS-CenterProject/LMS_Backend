using LMS.Domain.Entities;
using LMS.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Infrastructure.Persistence.Repositories
{
    public sealed class EnrollmentRepository(LMSDbContext context)
    : Repository<Enrollment>(context), IEnrollmentRepository
    {
        public async Task<Enrollment?> GetByStudentAndCourseAsync(
            Guid studentId, Guid courseId, CancellationToken ct = default) =>
            await DbSet
                .FirstOrDefaultAsync(
                    e => e.StudentId == studentId && e.CourseId == courseId, ct);

        public async Task<Enrollment?> GetByIdWithDetailsAsync(
            Guid enrollmentId, CancellationToken ct = default) =>
            await DbSet
                .Include(e => e.Course)
                    .ThenInclude(c => c.Instructor)
                .Include(e => e.Course)
                    .ThenInclude(c => c.Sections.OrderBy(s => s.OrderIndex))
                        .ThenInclude(s => s.Lessons.OrderBy(l => l.OrderIndex))
                .Include(e => e.LessonProgresses)
                .Include(e => e.Certificate)
                .FirstOrDefaultAsync(e => e.Id == enrollmentId, ct);

        public async Task<IEnumerable<Enrollment>> GetByStudentAsync(
            Guid studentId, CancellationToken ct = default) =>
            await DbSet
                .Where(e => e.StudentId == studentId)
                .Include(e => e.Course)
                    .ThenInclude(c => c.Instructor)
                .Include(e => e.Course)
                    .ThenInclude(c => c.Sections)
                        .ThenInclude(s => s.Lessons)
                .Include(e => e.LessonProgresses)
                .Include(e => e.Certificate)
                .OrderByDescending(e => e.EnrolledAt)
                .ToListAsync(ct);

        public async Task<bool> IsEnrolledAsync(
            Guid studentId, Guid courseId, CancellationToken ct = default) =>
            await DbSet
                .AnyAsync(
                    e => e.StudentId == studentId && e.CourseId == courseId, ct);
    }
}
