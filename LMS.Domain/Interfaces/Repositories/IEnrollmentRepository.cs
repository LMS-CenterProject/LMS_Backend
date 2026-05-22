using LMS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Domain.Interfaces.Repositories
{
    public interface IEnrollmentRepository : IRepository<Enrollment>
    {
        Task<Enrollment?> GetByStudentAndCourseAsync(
        Guid studentId,
        Guid courseId,
        CancellationToken ct = default);

        Task<Enrollment?> GetByIdWithDetailsAsync(
            Guid enrollmentId,
            CancellationToken ct = default);

        Task<IEnumerable<Enrollment>> GetByStudentAsync(
            Guid studentId,
            CancellationToken ct = default);

        Task<bool> IsEnrolledAsync(
            Guid studentId,
            Guid courseId,
            CancellationToken ct = default);

        Task<IEnumerable<Enrollment>> GetByCourseAsync(
            Guid courseId,
            CancellationToken ct = default);

    }
}
