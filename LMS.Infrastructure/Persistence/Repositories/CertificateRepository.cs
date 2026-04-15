using LMS.Domain.Entities;
using LMS.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Infrastructure.Persistence.Repositories
{
    public sealed class CertificateRepository(LMSDbContext context)
    : Repository<Certificate>(context), ICertificateRepository
    {
        public async Task<IEnumerable<Certificate>> GetByStudentAsync(
            Guid studentId, CancellationToken ct = default) =>
            await DbSet
                .Include(c => c.Enrollment)
                    .ThenInclude(e => e.Course)
                        .ThenInclude(c => c.Instructor)
                .Where(c => c.Enrollment.StudentId == studentId)
                .OrderByDescending(c => c.IssuedAt)
                .ToListAsync(ct);

        public async Task<bool> ExistsForEnrollmentAsync(
            Guid enrollmentId, CancellationToken ct = default) =>
            await DbSet
                .AnyAsync(c => c.EnrollmentId == enrollmentId, ct);
    }
}
